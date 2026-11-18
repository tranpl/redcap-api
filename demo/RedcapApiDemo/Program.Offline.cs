using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Newtonsoft.Json;
using Redcap;
using Redcap.Interfaces;
using Redcap.Models;

namespace RedcapApiDemo
{
    partial class Program
    {
        static async Task RunOfflineDemoAsync()
        {
            Console.WriteLine("Offline REDCap demo using synthetic records.");
            var api = new RedcapApi("https://offline.invalid/api/", new OfflineRecordTransport());
            var sample = Path.Combine(AppContext.BaseDirectory, "Docs", "SampleRecords.xml");
            var imported = await api.ImportRecordsFromOdmFileAsync("demo", sample);
            Console.WriteLine($"ODM import result: {imported}");
            var exported = await api.ExportRecordsAsync("demo", Content.Record);
            Console.WriteLine($"Exported records: {exported}");
            if(Newtonsoft.Json.Linq.JArray.Parse(exported).Count != 2)
            {
                throw new InvalidOperationException("Expected both sample records to be imported.");
            }
        }

        // Implements only the record operations exercised by the offline example.
        private sealed class OfflineRecordTransport : IRedcapTransport
        {
            private readonly Dictionary<string, Dictionary<string, string>> records = new();

            public Task<string> SendPostRequestAsync(RedcapApi api, Dictionary<string, string> payload,
                Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if(payload["content"] != "record")
                {
                    throw new NotSupportedException();
                }
                if(payload.TryGetValue("data", out var data))
                {
                    XNamespace odm = "http://www.cdisc.org/ns/odm/v1.3";
                    var subjects = XDocument.Parse(data).Descendants(odm + "SubjectData").ToList();
                    foreach(var subject in subjects)
                    {
                        var id = (string)subject.Attribute("SubjectKey");
                        var row = subject.Descendants(odm + "ItemData").ToDictionary(
                            item => (string)item.Attribute("ItemOID"), item => (string)item.Attribute("Value"));
                        row["record_id"] = id;
                        records[id] = row;
                    }
                    return Task.FromResult(JsonConvert.SerializeObject(new { count = subjects.Count }));
                }
                return Task.FromResult(JsonConvert.SerializeObject(records.Values));
            }

            public Task<string> SendPostRequestAsync(RedcapApi api, MultipartFormDataContent payload,
                Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
                => throw new NotSupportedException();

            public Task<Stream> GetStreamContentAsync(RedcapApi api, Dictionary<string, string> payload,
                Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
                => throw new NotSupportedException();
        }
    }
}
