using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Redcap.Mcp;

/// <summary>Maps one public REDCap operation to a fixed route and a closed MCP input schema.</summary>
internal sealed record Operation(string Name, string Content, string? Action, string[] Required, string[] Optional)
{
    public bool ReadOnly => Name.StartsWith("redcap_export_", StringComparison.Ordinal);
    public bool Binary => ReadOnly && (Content is "file" or "pdf" || Content == "fileRepository" && Action == "export");

    /// <summary>Maps a REDCap argument name to its MCP JSON Schema type.</summary>
    /// <param name="field">The argument name in the operation catalog.</param>
    /// <returns>The JSON Schema type name used to validate the argument.</returns>
    public static string TypeOf(string field) => field switch
    {
        "fields" or "forms" or "records" or "events" or "arms" or "dags" or "users" or "roles" => "array",
        "delete_logging" or "returnAlt" or "override" or "forceAutoNumber" or "backgroundProcess" or
        "exportCheckboxLabel" or "exportSurveyFields" or "exportDataAccessGroups" or "exportBlankForGrayFormStatus" or
        "combineCheckboxOptions" or "allRecords" or "compactDisplay" or "returnMetadataOnly" or "exportFiles" => "boolean",
        _ => "string"
    };

    /// <summary>Builds the discovery entry for this operation, including its schema and access hints.</summary>
    /// <returns>The tool advertised to MCP clients.</returns>
    public Tool Describe()
    {
        var properties = new Dictionary<string, object>();
        foreach(var field in Required.Concat(Optional))
        {
            var type = TypeOf(field);
            properties[field] = type == "array"
                ? new { type, items = new { type = "string" }, minItems = 1 }
                : (object)new { type };
        }
        return new Tool
        {
            Name = Name,
            Description = Name[7..].Replace('_', ' ') + " in the configured REDCap project." +
                (Required.Contains("data") ? " Supply data as a string serialized in the selected REDCap format." : "") +
                (Binary ? " Returns base64-encoded file contents." : ""),
            Annotations = new ToolAnnotations { ReadOnlyHint = ReadOnly, DestructiveHint = !ReadOnly },
            InputSchema = JsonSerializer.SerializeToElement(new { type = "object", properties, required = Required, additionalProperties = false })
        };
    }
}
