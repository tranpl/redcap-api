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

        /// <summary>Loads local demo settings and applies environment variable overrides.</summary>
        /// <returns>The merged demo settings.</returns>
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

        /// <summary>Applies the first configured environment value for each demo setting.</summary>
        /// <param name="settings">The demo configuration to update from the environment.</param>
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

        /// <summary>Looks up environment variables in the supplied order.</summary>
        /// <param name="names">Environment variable names in order of precedence.</param>
        /// <returns>The trimmed value, or null if none is set.</returns>
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

        /// <summary>Searches for the local demo settings file.</summary>
        /// <returns>The settings file path, or null if it cannot be found.</returns>
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
