using System;
using System.IO;
using System.Text.Json;

namespace RedcapApiDemo
{
    partial class Program
    {
        sealed class DevelopmentSettings
        {
            public string BaseUri { get; set; }
            public string ProjectToken { get; set; }
            public string SuperToken { get; set; }
        }

        sealed class DevelopmentSettingsRoot
        {
            public DevelopmentSettings RedcapDemo { get; set; }
        }

        static DevelopmentSettings LoadDevelopmentSettings()
        {
            var settings = new DevelopmentSettings();
            var filePath = FindDevelopmentSettingsFile();
            if(!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var parsed = JsonSerializer.Deserialize<DevelopmentSettingsRoot>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if(parsed?.RedcapDemo != null)
                    {
                        settings = parsed.RedcapDemo;
                    }
                }
                catch
                {
                    // Keep defaults if local config is malformed.
                }
            }

            ApplyEnvironmentOverrides(settings);

            return settings;
        }

        static void ApplyEnvironmentOverrides(DevelopmentSettings settings)
        {
            if(settings == null)
            {
                return;
            }

            var baseUri = FirstNonEmptyEnvironmentVariable("REDCAP_DEMO_BASE_URI", "RedcapDemo__BaseUri");
            if(!string.IsNullOrWhiteSpace(baseUri))
            {
                settings.BaseUri = baseUri;
            }

            var projectToken = FirstNonEmptyEnvironmentVariable("REDCAP_DEMO_PROJECT_TOKEN", "RedcapDemo__ProjectToken");
            if(!string.IsNullOrWhiteSpace(projectToken))
            {
                settings.ProjectToken = projectToken;
            }

            var superToken = FirstNonEmptyEnvironmentVariable("REDCAP_DEMO_SUPER_TOKEN", "RedcapDemo__SuperToken");
            if(!string.IsNullOrWhiteSpace(superToken))
            {
                settings.SuperToken = superToken;
            }
        }

        static string FirstNonEmptyEnvironmentVariable(params string[] names)
        {
            foreach(var name in names)
            {
                var value = Environment.GetEnvironmentVariable(name);
                if(!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return null;
        }

        static string FindDevelopmentSettingsFile()
        {
            var current = new DirectoryInfo(Environment.CurrentDirectory);
            while(current != null)
            {
                var candidate = Path.Combine(current.FullName, "appsettings.Development.json");
                if(File.Exists(candidate))
                {
                    return candidate;
                }

                current = current.Parent;
            }

            return null;
        }
    }
}
