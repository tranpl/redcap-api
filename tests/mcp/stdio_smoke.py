"""Verify the Release MCP executable with synthetic credentials and a loopback REDCap stub."""
import base64
import json
import os
from pathlib import Path
import subprocess
import threading
from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import parse_qs

ROOT = Path(__file__).resolve().parents[2]
DLL = ROOT / "src/RedcapApi.Mcp/bin/Release/net11.0/RedcapApi.Mcp.dll"


class Endpoint(BaseHTTPRequestHandler):
    def do_POST(self):
        body = self.rfile.read(int(self.headers["Content-Length"]))
        self.server.requests.append(body)
        self.send_response(self.server.status)
        self.end_headers()
        self.wfile.write(self.server.reply)

    def log_message(self, *args):
        pass


def exercise(endpoint, writes):
    env = dict(os.environ, REDCAP_MCP_URL=f"http://127.0.0.1:{endpoint.server_port}/api/",
               REDCAP_MCP_TOKEN="project-test", REDCAP_MCP_SUPER_TOKEN="super-test",
               REDCAP_MCP_ENABLE_WRITES=str(writes))
    with subprocess.Popen([os.environ.get("DOTNET_HOST_PATH", "dotnet"), str(DLL)],
                          stdin=subprocess.PIPE, stdout=subprocess.PIPE, text=True, env=env) as process:
        sequence = 0

        def send(message):
            process.stdin.write(json.dumps(dict(jsonrpc="2.0", **message)) + "\n")
            process.stdin.flush()

        def rpc(method, params):
            nonlocal sequence
            sequence += 1
            send(dict(id=sequence, method=method, params=params))
            response = json.loads(process.stdout.readline())
            assert response["id"] == sequence and "result" in response, response
            return response["result"]

        def call(tool_name, **arguments):
            return rpc("tools/call", dict(name=tool_name, arguments=arguments))

        try:
            rpc("initialize", dict(protocolVersion="2025-11-25", capabilities={},
                                   clientInfo=dict(name="smoke", version="1")))
            send(dict(method="notifications/initialized"))
            catalog = {t["name"]: t for t in rpc("tools/list", {})["tools"]}
            count = len(endpoint.requests)
            if not writes:
                assert "redcap_export_records" in catalog and "redcap_delete_records" not in catalog
                assert call("redcap_delete_records", records=["1"])["isError"]
                assert len(endpoint.requests) == count
                return

            assert len(catalog) == 55
            for name, tool in catalog.items():
                schema = tool["inputSchema"]
                args = {}
                for field in schema["required"]:
                    kind = schema["properties"][field]["type"]
                    args[field] = ["1"] if kind == "array" else False if kind == "boolean" else "1"
                if "fileBase64" in args:
                    args.update(fileName="upload.bin", fileBase64="AP8=")
                endpoint.reply = b"success"
                if name == "redcap_export_records":
                    args.update(records=["A", "B"])
                if name == "redcap_export_repository_file":
                    endpoint.reply = bytes([0, 255])
                if name == "redcap_create_project":
                    endpoint.reply = b"A" * 32
                result = call(name, **args)
                assert not result.get("isError"), (name, result)
                body = endpoint.requests[-1]
                if "fileBase64" in args:
                    assert b"upload.bin" in body and bytes([0, 255]) in body
                elif name == "redcap_export_records":
                    assert parse_qs(body)[b"records[1]"] == [b"B"]
                elif name == "redcap_create_project":
                    assert parse_qs(body)[b"token"] == [b"super-test"]
                    assert "A" * 32 not in str(result)
                elif name == "redcap_export_repository_file":
                    assert base64.b64decode(json.loads(result["content"][0]["text"])["base64"]) == endpoint.reply
            assert len(endpoint.requests) == count + 55
            assert call("redcap_export_version", format="json", token="unexpected")["isError"]
            assert len(endpoint.requests) == count + 55
            # Run the documented tools/call examples through the same stdio session.
            for sample in ("export-records", "import-records"):
                params = json.loads((ROOT / "demo/mcp" / f"{sample}.json").read_text())
                endpoint.reply = b'[{"record_id":"1"}]' if sample == "export-records" else b'{"count":1}'
                result = rpc("tools/call", params)
                assert not result.get("isError") and result["content"][0]["text"] == endpoint.reply.decode()
                form = parse_qs(endpoint.requests[-1])
                if sample == "export-records":
                    assert form[b"records[0]"] == [b"1"] and form[b"fields[0]"] == [b"record_id"]
                else:
                    assert json.loads(form[b"data"][0]) == [{"record_id": "1", "visit_status": "scheduled"}]
                print(f"{params['name']} -> {result['content'][0]['text']}")
            endpoint.status = 403
            assert call("redcap_export_version", format="json")["isError"]
        finally:
            process.stdin.close()
            process.wait(timeout=10)


if __name__ == "__main__":
    with HTTPServer(("127.0.0.1", 0), Endpoint) as endpoint:
        endpoint.requests = []
        endpoint.status = 200
        threading.Thread(target=endpoint.serve_forever, daemon=True).start()
        exercise(endpoint, False)
        exercise(endpoint, True)
        endpoint.shutdown()
    print("Verified MCP discovery, 55 tool calls, credentials, binary transfers and errors.")
