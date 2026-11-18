using Newtonsoft.Json;

using System;

namespace RedcapApiDemo
{
    partial class Program
    {
        /// <summary>Formats JSON for display and shortens markup responses for console output.</summary>
        /// <param name="response">The raw response body returned by REDCap.</param>
        /// <returns>A console-friendly representation of the response.</returns>
        static string FormatApiResponse(string response)
        {
            if(string.IsNullOrWhiteSpace(response))
            {
                return "<empty response>";
            }

            var trimmed = response.TrimStart();
            if(trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                try
                {
                    var parsed = JsonConvert.DeserializeObject(response);
                    return parsed == null ? response : JsonConvert.SerializeObject(parsed, Formatting.Indented);
                }
                catch
                {
                    return response;
                }
            }

            if(trimmed.StartsWith("<"))
            {
                var previewLength = Math.Min(300, response.Length);
                return $"Non-JSON response from REDCap (likely HTML/error page): {response.Substring(0, previewLength)}";
            }

            return response;
        }
    }
}
