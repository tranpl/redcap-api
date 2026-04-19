using System;
using System.IO;

namespace RedcapApiDemo
{
    partial class Program
    {
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
