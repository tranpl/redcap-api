using System;
using System.IO;

namespace RedcapApiDemo
{
    partial class Program
    {
        /// <summary>Prints the project rights, fields and files required for the live demo.</summary>
        /// <param name="uri">The URL of the REDCap endpoint.</param>
        /// <param name="eventName">The unique event name in a longitudinal project.</param>
        /// <param name="fieldName">The file upload field used by the demo.</param>
        /// <param name="fileName">The file name within the selected directory.</param>
        /// <param name="downloadPath">The directory for downloaded demo files.</param>
        static void PrintDemoRequirements(string uri, string eventName, string fieldName, string fileName, string downloadPath)
        {
            Console.WriteLine("================ Demo Prerequisites ================");
            Console.WriteLine($"Endpoint base URI: {uri}");
            Console.WriteLine("Expected API endpoint: <uri>/api/");
            Console.WriteLine("Required REDCap setup:");
            Console.WriteLine(" - Project has demographics instrument");
            Console.WriteLine($" - Unique event exists: {eventName}");
            Console.WriteLine($" - File upload field exists: {fieldName}");
            Console.WriteLine(" - API token user has import/export/delete rights for records/files");
            Console.WriteLine($"Required local file: {fileName}");
            Console.WriteLine($"Download folder: {downloadPath}");
            Console.WriteLine("====================================================");
        }

        /// <summary>Checks for the source fixture and creates the download directory if needed.</summary>
        /// <param name="fileName">The file name within the selected directory.</param>
        /// <param name="downloadPath">The directory for downloaded demo files.</param>
        /// <param name="docsPath">Receives the directory containing the demo fixtures.</param>
        /// <returns>True if the local demo fixture exists.</returns>
        static bool EnsureDemoLocalArtifacts(string fileName, string downloadPath, out string docsPath)
        {
            docsPath = ResolveDemoDocsPath();
            var sourceFile = Path.Combine(docsPath, fileName);

            var ok = true;
            if(!Directory.Exists(docsPath))
            {
                Console.WriteLine($"Missing docs folder: {docsPath}");
                ok = false;
            }
            else if(!File.Exists(sourceFile))
            {
                Console.WriteLine($"Missing required file: {sourceFile}");
                ok = false;
            }

            if(!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
                Console.WriteLine($"Created download folder: {downloadPath}");
            }

            return ok;
        }

        /// <summary>Locates the demo fixture directory relative to the executable.</summary>
        /// <returns>The fixture directory or the historical relative fallback.</returns>
        static string ResolveDemoDocsPath()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while(current != null)
            {
                var candidate = Path.Combine(current.FullName, "demo", "RedcapApiDemo", "Docs");
                if(Directory.Exists(candidate))
                {
                    return candidate + Path.DirectorySeparatorChar;
                }

                if(string.Equals(current.Name, "RedcapApiDemo", StringComparison.OrdinalIgnoreCase))
                {
                    var localCandidate = Path.Combine(current.FullName, "Docs");
                    if(Directory.Exists(localCandidate))
                    {
                        return localCandidate + Path.DirectorySeparatorChar;
                    }
                }

                current = current.Parent;
            }

            // Fall back to the historical relative expectation used by the original demo.
            return Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Docs") + Path.DirectorySeparatorChar;
        }
    }
}
