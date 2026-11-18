using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Redcap.Http;
using Redcap.Utilities;

namespace Redcap.Mcp;

/// <summary>Serves the enabled REDCap operations over MCP standard input and output.</summary>
/// <param name="available">Operations enabled by the server configuration.</param>
/// <param name="http">The HTTP client used for REDCap requests.</param>
internal sealed class Program(Dictionary<string, Operation> available, HttpClient http)
{
    /// <summary>Loads the catalog and listens for tool requests until the stdio session ends.</summary>
    /// <returns>A task that completes when the server stops.</returns>
    private static async Task Main()
    {
        using var catalog = typeof(Operation).Assembly.GetManifestResourceStream("RedcapApi.Mcp.operations.json")!;
        var operations = JsonSerializer.Deserialize<Operation[]>(catalog)!;
        var writes = bool.TryParse(Environment.GetEnvironmentVariable("REDCAP_MCP_ENABLE_WRITES"), out var enabled) && enabled;
        var available = operations.Where(op => writes || op.ReadOnly).ToDictionary(op => op.Name);
        using var http = new HttpClient(Utils.GetHttpHandler()) { MaxResponseContentBufferSize = 32 * 1024 * 1024 };
        var options = new McpServerOptions
        {
            ServerInfo = new Implementation { Name = "redcap", Version = typeof(Operation).Assembly.GetName().Version!.ToString() },
            Handlers = new McpServerHandlers
            {
                ListToolsHandler = (_, _) => ValueTask.FromResult(new ListToolsResult
                {
                    Tools = available.Values.Select(op => op.Describe()).ToList()
                }),
                CallToolHandler = new Program(available, http).CallAsync
            }
        };
        await using var server = McpServer.Create(new StdioServerTransport(options), options);
        await server.RunAsync();
    }

    /// <summary>Validates and dispatches an enabled MCP tool with its server-configured credential.</summary>
    /// <param name="context">The incoming MCP request with its tool name and arguments.</param>
    /// <param name="cancellation">A token that cancels the in-flight tool call.</param>
    /// <returns>The REDCap response or an MCP tool error.</returns>
    async ValueTask<CallToolResult> CallAsync(RequestContext<CallToolRequestParams> context, CancellationToken cancellation)
    {
        var call = context.Params;
        if(call == null || !available.TryGetValue(call.Name, out var op))
            return Error("Unknown or disabled REDCap tool.");
        var tokenName = op.Name == "redcap_create_project" ? "REDCAP_MCP_SUPER_TOKEN" : "REDCAP_MCP_TOKEN";
        var token = Environment.GetEnvironmentVariable(tokenName);
        if(string.IsNullOrWhiteSpace(token) || !Uri.TryCreate(Environment.GetEnvironmentVariable("REDCAP_MCP_URL"), UriKind.Absolute, out var uri) ||
            (uri.Scheme != "https" && !(uri.IsLoopback && uri.Scheme == "http")) || uri.UserInfo.Length != 0 || uri.Query.Length != 0)
            return Error("Configure the server endpoint and the credential for this operation.");

        var args = call.Arguments ?? new Dictionary<string, JsonElement>();
        if(op.Required.Any(key => !args.ContainsKey(key)))
            return Error("Missing required tool argument.");
        var fields = new Dictionary<string, string> { ["token"] = token, ["content"] = op.Content };
        if(op.Action != null) fields.Add("action", op.Action);
        foreach(var (key, value) in args)
        {
            if(!op.Required.Contains(key) && !op.Optional.Contains(key)) return Error("Unexpected tool argument.");
            switch(Operation.TypeOf(key), value.ValueKind)
            {
                case ("array", JsonValueKind.Array):
                    if(value.GetArrayLength() == 0) return Error($"Supply at least one value for {key}.");
                    var index = 0;
                    foreach(var item in value.EnumerateArray())
                    {
                        if(item.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(item.GetString())) return Error($"Invalid item in {key}.");
                        fields.Add($"{key}[{index++}]", item.GetString()!);
                    }
                    break;
                case ("boolean", JsonValueKind.True or JsonValueKind.False):
                    fields.Add(key, key == "override" ? (value.GetBoolean() ? "1" : "0") : value.GetRawText());
                    break;
                case ("string", JsonValueKind.String):
                    if(string.IsNullOrWhiteSpace(value.GetString()) && key is not ("dag" or "fileBase64"))
                        return Error($"Provide a value for {key}, or omit it if optional.");
                    fields.Add(key, value.GetString()!);
                    break;
                default:
                    return Error($"Incorrect type for {key}.");
            }
        }
        try
        {
            using var body = MakeContent(fields);
            using var response = await http.PostAsync(uri, body, cancellation);
            if(!response.IsSuccessStatusCode) return Error($"REDCap returned HTTP {(int)response.StatusCode}.");
            if(op.Binary)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellation);
                return Text(JsonSerializer.Serialize(new { mimeType = response.Content.Headers.ContentType?.MediaType, base64 = Convert.ToBase64String(bytes) }));
            }
            var result = await response.Content.ReadAsStringAsync(cancellation);
            if(result.TrimStart().StartsWith('{'))
            {
                try
                {
                    using var json = JsonDocument.Parse(result);
                    if(json.RootElement.TryGetProperty("error", out _)) return Error("REDCap rejected the request.");
                }
                catch(JsonException) { /* REDCap may return plain text. */ }
            }
            if(op.Name == "redcap_create_project" && result.Trim().Trim('"') is { Length: 32 } projectToken && projectToken.All(Uri.IsHexDigit))
                return Text("Project created. Retrieve its new API token through REDCap.");
            return Text(result);
        }
        catch(FormatException) { return Error("Invalid base64 file content."); }
        catch(ArgumentException) { return Error("Invalid file name or request data."); }
        catch(HttpRequestException) { return Error("Unable to complete the REDCap HTTP request."); }
        catch(OperationCanceledException) when(!cancellation.IsCancellationRequested) { return Error("REDCap request timed out."); }
    }

    /// <summary>Encodes REDCap fields as a URL-encoded form or a multipart file upload.</summary>
    /// <param name="fields">Validated form fields, including fileName and fileBase64 for uploads.</param>
    /// <returns>The HTTP content for the caller to dispose after the request.</returns>
    static HttpContent MakeContent(Dictionary<string, string> fields)
    {
        if(!fields.Remove("fileBase64", out var encoded)) return new CustomFormUrlEncodedContent(fields);
        var name = fields["fileName"];
        if(string.IsNullOrWhiteSpace(name) || name.Contains('/') || name.Contains('\\') || name.Contains('"') || name.Any(char.IsControl))
            throw new ArgumentException("Invalid upload name.");
        fields.Remove("fileName");
        var bytes = Convert.FromBase64String(encoded);
        var parts = new MultipartFormDataContent();
        foreach(var (key, value) in fields) parts.Add(new StringContent(value), key);
        parts.Add(new ByteArrayContent(bytes), "file", name);
        return parts;
    }

    /// <summary>Wraps a successful response in an MCP text content block.</summary>
    /// <param name="value">The text returned to the MCP client.</param>
    /// <returns>A successful tool result containing one text block.</returns>
    static CallToolResult Text(string value) => new() { Content = [new TextContentBlock { Text = value }] };
    /// <summary>Creates an MCP tool failure containing the supplied diagnostic.</summary>
    /// <param name="value">The text returned to the MCP client.</param>
    /// <returns>A tool result marked as an error.</returns>
    static CallToolResult Error(string value) => new() { IsError = true, Content = [new TextContentBlock { Text = value }] };
}
