using Redcap;
using Redcap.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace RedcapApiDemo
{
    partial class Program
    {
        /// <summary>Reads the method selector from the command line.</summary>
        /// <param name="args">The command-line arguments supplied to the demo.</param>
        /// <returns>The method name or alias, or null if no selector was supplied.</returns>
        static string GetMethodArgument(string[] args)
        {
            if(args == null || args.Length == 0)
            {
                return null;
            }

            for(var i = 0; i < args.Length; i++)
            {
                var current = args[i];
                if(string.Equals(current, "-m", StringComparison.OrdinalIgnoreCase) || string.Equals(current, "--method", StringComparison.OrdinalIgnoreCase))
                {
                    if(i + 1 < args.Length && !string.IsNullOrWhiteSpace(args[i + 1]))
                    {
                        return args[i + 1].Trim();
                    }
                }

                if(current.StartsWith("-m=", StringComparison.OrdinalIgnoreCase))
                {
                    return current.Substring(3).Trim();
                }

                if(current.StartsWith("--method=", StringComparison.OrdinalIgnoreCase))
                {
                    return current.Substring(9).Trim();
                }
            }

            return null;
        }

        /// <summary>Dispatches a selected live demo operation by name or alias.</summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="token">The project API token for this operation.</param>
        /// <param name="method">The selected demo method name or alias.</param>
        /// <returns>A task that completes when the demo operation finishes.</returns>
        static async Task RunSingleMethodAsync(RedcapApi redcapApi, string token, string method)
        {
            if(string.IsNullOrWhiteSpace(method))
            {
                return;
            }

            var normalized = method.Trim();
            if(normalized.Equals("import", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("ImportRecordsAsync", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("importrecords", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Running ImportRecordsAsync only...");
                await RunImportRecordsOnlyAsync(redcapApi, token);
                return;
            }

            if(normalized.Equals("export", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("ExportRecordsAsync", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("exportrecords", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Running ExportRecordsAsync only...");
                await RunExportRecordsOnlyAsync(redcapApi, token);
                return;
            }

            Console.WriteLine($"Unknown method selector '{method}'. Supported selectors: import, ImportRecordsAsync, importrecords, export, ExportRecordsAsync, exportrecords");
        }

        /// <summary>Imports sample demographics into the configured project.</summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="token">The project API token for this operation.</param>
        /// <returns>A task that completes when the demo operation finishes.</returns>
        static async Task RunImportRecordsOnlyAsync(RedcapApi redcapApi, string token)
        {
            Console.WriteLine("How many records to import? (default: 1)");
            var countInput = Console.ReadLine();
            var count = 1;
            if(!string.IsNullOrWhiteSpace(countInput) && int.TryParse(countInput, out var parsed) && parsed > 0)
            {
                count = parsed;
            }

            var importDemographicsData = CreateDemographics(includeBio: false, count: count);
            Console.WriteLine($"Importing record ids: {string.Join(",", importDemographicsData.Select(x => x.RecordId))}");

            var result = await redcapApi.ImportRecordsAsync(
                token,
                Content.Record,
                RedcapFormat.json,
                RedcapDataType.flat,
                OverwriteBehavior.normal,
                forceAutoNumber: true,
                backgroundProcess: false,
                importDemographicsData,
                "MDY",
                CsvDelimiter.tab,
                ReturnContent.ids,
                RedcapReturnFormat.json);

            Console.WriteLine($"ImportRecordsAsync Result: {FormatApiResponse(result)}");
        }

        /// <summary>Exports records from the configured project and displays the response.</summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="token">The project API token for this operation.</param>
        /// <returns>A task that completes when the demo operation finishes.</returns>
        static async Task RunExportRecordsOnlyAsync(RedcapApi redcapApi, string token)
        {
            Console.WriteLine("Enter record IDs to export (comma-delimited), or press Enter to export all:");
            var recordInput = Console.ReadLine();

            string[] records = null;
            if(!string.IsNullOrWhiteSpace(recordInput))
            {
                records = recordInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
            }

            var result = await redcapApi.ExportRecordsAsync(
                token,
                RedcapFormat.json,
                RedcapDataType.flat,
                records,
                forms: new[] { "demographics" });

            Console.WriteLine($"ExportRecordsAsync Result: {FormatApiResponse(result)}");
        }
    }
}
