[![Build Status](https://dev.azure.com/cctrbic/redcap-api/_apis/build/status/redcap-api?branchName=master)](https://dev.azure.com/cctrbic/redcap-api/_build/latest?definitionId=122&branchName=master)

[![NuGet](https://img.shields.io/nuget/dt/RedcapApi.svg?style=for-the-badge)](https://www.nuget.org/packages/RedcapAPI) 
[![license](https://img.shields.io/github/license/tranpl/redcap-api.svg?style=for-the-badge)](https://github.com/tranpl/redcap-api/blob/master/LICENSE.md)

# REDCap API Library for .NET

Use the .NET client from an application or launch the MCP server to give an agent access to REDCap's public API. This source tree targets .NET 11.

## What's New in 2.0.x

- Updated the client against the public API reference shipped with REDCap 17.5.10.
- Added bulk ODM record import into an existing project and a credential-free demo that imports and exports synthetic records.
- Added a stdio MCP server with 55 tools. It exposes 28 export tools by default and supports all 55 operations when writes are enabled.
- Moved the library, demo and tests to .NET 11, refreshed dependencies and added offline MCP coverage.
- Each library build increments the patch version. See [Build versions](#build-versions) for CI numbering.

## Use the .NET client

Install the .NET 11 SDK to build this checkout. Applications referencing this build must target a compatible framework. From your application, add a project reference to the checkout:

```bash
dotnet add reference /path/to/redcap-api/src/RedcapApi/RedcapApi.csproj
```

For a published release, install the [RedcapAPI NuGet package](https://www.nuget.org/packages/RedcapAPI). To export one record, supply your project's HTTPS API URL and token from your application's configuration:

```csharp
using Redcap;

var api = new RedcapApi(Environment.GetEnvironmentVariable("REDCAP_API_URL")
    ?? throw new InvalidOperationException("Set REDCAP_API_URL to the project's API URL."));
var token = Environment.GetEnvironmentVariable("REDCAP_API_TOKEN")
    ?? throw new InvalidOperationException("Set REDCAP_API_TOKEN."));
var result = await api.ExportRecordsAsync(token, records: ["1"], fields: ["record_id"]);
```

The equivalent REDCap API call is a form-encoded HTTP POST. This Bash example reads the token from the environment and sends it through standard input:

```bash
printf '%s' "$REDCAP_API_TOKEN" | curl --fail-with-body --silent --show-error \
  --data-urlencode 'token@-' --data 'content=record&format=json&type=flat' \
  --data-urlencode 'records[0]=1' --data-urlencode 'fields[0]=record_id' \
  "$REDCAP_API_URL"
```

With access to record 1 in a project whose ID field is `record_id`, the JSON response looks like `[{"record_id":"1"}]`. The [offline C# sample](demo/RedcapApiDemo/Program.Offline.cs) shows both an import and an export with an in-memory transport.

## Use the MCP server

Build the solution from the repository root:

```bash
dotnet build RedcapApi.sln -c Release
```

Check `dotnet --version` if the build reports NETSDK1045. On macOS or Linux with .NET 11 installed in `$HOME/.dotnet`, run `export PATH="$HOME/.dotnet:$PATH"` in the current shell, then repeat the build. A GUI MCP client may need the absolute path to that same `dotnet` executable in its `command` setting.

Register the executable with a stdio-capable MCP client. Replace the absolute path below with your checkout's path:

```json
{
  "mcpServers": {
    "redcap": {
      "command": "dotnet",
      "args": ["/absolute/path/to/redcap-api/src/RedcapApi.Mcp/bin/Release/net11.0/RedcapApi.Mcp.dll"]
    }
  }
}
```

Configure the following variables in the server process environment. Store tokens in the client's secret configuration. The agent can receive any data the supplied credential is allowed to export.

| Variable | Purpose |
| --- | --- |
| `REDCAP_MCP_URL` | Complete REDCap API URL, such as `https://redcap.example.edu/api/`. HTTP is accepted only on loopback. |
| `REDCAP_MCP_TOKEN` | Project API token for ordinary calls. Its REDCap permissions apply to every request. |
| `REDCAP_MCP_ENABLE_WRITES` | Set to `true` and restart the process to expose imports, deletions, randomization and other changes. |
| `REDCAP_MCP_SUPER_TOKEN` | Separate credential used only by `redcap_create_project`, which also requires writes enabled. |

Use `tools/list` to discover each enabled tool and its required and optional arguments. The [operation catalog](src/RedcapApi.Mcp/operations.json) covers records, metadata, projects, reports, surveys, instruments, events, arms, users, roles, data access groups, logging, file fields and the file repository.

### Discover and call tools

After connecting, ask your agent to “export only the record ID for record 1.” An MCP client initializes the stdio session, sends `notifications/initialized`, and requests `tools/list`. Each tool's `inputSchema` describes its arguments. For that request, the client sends:

```json
{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"redcap_export_records","arguments":{"format":"json","type":"flat","records":["1"],"fields":["record_id"]}}}
```

Check `result.isError` before reading `result.content[0].text`. On success, that text contains the REDCap response in the requested format. Parse it as JSON when requesting JSON.

| Sample `tools/call` parameters | What it does |
| --- | --- |
| [Export a record](demo/mcp/export-records.json) | Returns only the ID field for record 1. Works with the default export tools. |
| [Import a record](demo/mcp/import-records.json) | Creates or updates record 1 with a synthetic visit status and requests a count. A live project needs a matching field and import rights, and the server needs writes enabled. |

To run both sample calls offline, build the solution in Release and run `python3 tests/mcp/stdio_smoke.py` from the repository root. It starts the MCP process, sends the sample files over stdio, and prints the responses from a loopback REDCap stub. The [Python client example](tests/mcp/stdio_smoke.py) includes session initialization, discovery and error handling.

Supply identifiers as strings and switches as JSON booleans. For imports, pass `data` as a string serialized in the selected REDCap format. Upload tools accept a plain `fileName` and base64-encoded `fileBase64`. File and PDF exports return a text block containing JSON with `mimeType` and `base64`. The server buffers up to 32 MiB per HTTP response.

## Offline demo and bulk XML import

Run the demo without arguments to import two synthetic records from a bundled ODM file and export them again through an in-memory transport. It runs without credentials, prompts, or a REDCap server.

```bash
dotnet run --project demo/RedcapApiDemo/RedcapApiDemo.csproj
```

To import all records in a REDCap project XML export into an existing project, use its record-import token and supply the file once. The destination needs matching metadata. The client reads the document into memory and submits it as one ODM record import.

```csharp
var result = await api.ImportRecordsFromOdmFileAsync(token, "project-export.xml");
```

## Live demo prerequisites

Before running the demo app in `demo/RedcapApiDemo`, verify all of the following:

1. REDCap instance is reachable and API endpoint resolves.
    - Example base URI: `https://localhost`
    - Effective API URL used by demo: `<base-uri>/api/`
2. You have a valid project API token with rights for records, metadata, files, users, and project setup actions used in the demo.
3. REDCap project setup exists:
    - Demographics instrument exists.
    - Longitudinal event `event_1_arm_1` exists.
    - File upload field `file_upload` exists.
4. Local demo file exists:
    - `demo/RedcapApiDemo/Docs/Demographics_TestProject_DataDictionary.csv`
5. Local download folder is writable:
    - `C:\redcap_download_files`

Run the live demo:

```bash
dotnet run --project demo/RedcapApiDemo/RedcapApiDemo.csproj -- --live
```

The demo now prints a startup checklist and validates local artifacts before API calls begin.

### Developer Local Settings (Tokens)

For local testing, you can provide defaults using a local config file in `demo/RedcapApiDemo`.

1. Copy `demo/RedcapApiDemo/appsettings.Development.example.json` to `demo/RedcapApiDemo/appsettings.Development.json`.
2. Fill in your local values:

```json
{
    "RedcapDemo": {
        "BaseUri": "http://localhost",
        "ProjectToken": "YOUR_PROJECT_TOKEN",
        "SuperToken": "YOUR_SUPER_TOKEN"
    }
}
```

`appsettings.Development.json` is git-ignored in the demo project so local secrets are not committed.

If values are present in this file, pressing Enter at demo prompts will use them.

### Developer Environment Variables

You can configure demo defaults via environment variables instead of a local settings file.

Supported variables:

- `REDCAP_DEMO_BASE_URI`
- `REDCAP_DEMO_PROJECT_TOKEN`
- `REDCAP_DEMO_SUPER_TOKEN`

Alternative .NET-style names are also supported:

- `RedcapDemo__BaseUri`
- `RedcapDemo__ProjectToken`
- `RedcapDemo__SuperToken`

PowerShell example:

```powershell
$env:REDCAP_DEMO_BASE_URI = "http://localhost"
$env:REDCAP_DEMO_PROJECT_TOKEN = "YOUR_PROJECT_TOKEN"
$env:REDCAP_DEMO_SUPER_TOKEN = "YOUR_SUPER_TOKEN"
dotnet run --project demo/RedcapApiDemo/RedcapApiDemo.csproj -- -m ImportRecordsAsync
```

Configuration precedence is:

1. Environment variables
2. `appsettings.Development.json`
3. In-code defaults

### Run One Method Only

You can run a single demo method by name:

```bash
dotnet run --project demo/RedcapApiDemo/RedcapApiDemo.csproj -- -m ImportRecordsAsync
dotnet run --project demo/RedcapApiDemo/RedcapApiDemo.csproj -- -m ExportRecordsAsync
```

Supported selectors include method names and short aliases, e.g. `ImportRecordsAsync`/`import`, `ExportRecordsAsync`/`export`.

## Build versions

Each local library build increments the patch number for the current major/minor line. The first build in a fresh checkout produces 2.0.1. Debug and Release share a checkout-local counter that survives `dotnet clean`. GitHub Actions uses the workflow run number, Bitbucket uses its build number, and Azure Pipelines uses a pipeline counter. CI retries of the same numbered run reuse its version. Tagged releases use the patch number in their 2.0.x tag.

Use `dotnet pack --no-build -c Release` to package the version already compiled in Release.

## Testing and Coverage

The repository includes transport-focused unit tests that validate payload composition for API methods and overload variants without requiring a live REDCap instance.

Run tests:

```bash
dotnet test tests/RedcapApi.Tests/RedcapApi.Tests.csproj --verbosity minimal
```

Run tests with coverage:

```bash
dotnet test tests/RedcapApi.Tests/RedcapApi.Tests.csproj --collect:"XPlat Code Coverage" --verbosity minimal
```

After building the solution in Release, exercise discovery and calls for all 55 MCP tools with a local HTTP stub and Python 3:

```bash
python3 tests/mcp/stdio_smoke.py
```

The unit tests, MCP smoke test and default demo run with synthetic data. Live interoperability has not been verified because no disposable project was available. See [issue #142](https://github.com/tranpl/redcap-api/issues/142).
