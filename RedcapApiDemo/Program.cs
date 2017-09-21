using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Redcap Api Demo Started!");
            var _apiToken = "ED2D0A2E34D9693DCA7E9E6BD5F0941C";
            if (false) _apiToken = "BCC9D1F214B8BE2AA4F24C56ED7674E4";
            var redcap_api = new RedcapApi(_apiToken, "http://localhost/redcap/api/");

            Console.WriteLine("Calling GetRecordAsync() . . .");
            var GetRecordAsync = redcap_api.GetRecordAsync("1", InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            var GetRecordAsyncData = JsonConvert.DeserializeObject(GetRecordAsync);
            Console.WriteLine($"GetRecordAsync Result: {GetRecordAsyncData}");

            Console.WriteLine("Calling ExportEventsAsync() . . .");
            var exportEvents = redcap_api.ExportEventsAsync(InputFormat.json, ReturnFormat.json).Result;
            var exportEventsAsync = JsonConvert.DeserializeObject(exportEvents);
            Console.WriteLine($"ExportEventsAsync Result: {exportEventsAsync}");

            Console.WriteLine("Calling GetRecordsAsync() . . .");
            var GetRecordsAsync = redcap_api.GetRecordsAsync(InputFormat.json,  ReturnFormat.json, RedcapDataType.flat).Result;
            var GetRecordsAsyncData = JsonConvert.DeserializeObject(GetRecordsAsync);
            Console.WriteLine($"GetRecordsAsync Result: {GetRecordsAsyncData}");

            Console.WriteLine("Calling GetRedcapVersionAsync() . . .");
            var GetRedcapVersionAsync = redcap_api.GetRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            Console.WriteLine($"GetRedcapVersionAsync Result: {GetRedcapVersionAsync}");


            var saveRecordsAsyncObject = new {
                record_id = "1",
                redcap_event_name = "event_1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };

            Console.WriteLine("Calling SaveRecordsAsync() . . .");
            var SaveRecordsAsync = redcap_api.SaveRecordsAsync(saveRecordsAsyncObject, ReturnContent.ids,OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json).Result;
            var SaveRecordsAsyncData = JsonConvert.DeserializeObject(SaveRecordsAsync);
            Console.WriteLine($"SaveRecordsAsync Result: {SaveRecordsAsyncData}");


            Console.WriteLine("Calling ExportRecordsAsync() . . .");
            var ExportRecordsAsync = redcap_api.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat).Result;
            var ExportRecordsAsyncData = JsonConvert.DeserializeObject(ExportRecordsAsync);
            Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncData}");

            Console.WriteLine("Calling ExportArmsAsync() . . .");
            var ExportArmsAsync = redcap_api.ExportArmsAsync(InputFormat.json, ReturnFormat.json).Result;
            var ExportArmsAsyncData = JsonConvert.DeserializeObject(ExportArmsAsync);
            Console.WriteLine($"ExportArmsAsync Result: {ExportArmsAsyncData}");

            Console.WriteLine("Calling ExportRecordsAsync() . . .");
            var ExportRecordsAsync2 = redcap_api.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, "research_opportunities", "event1_arm1", "cda_check,info_check,protocol_check,synopsis_check,feasquestion_check").Result;
            var ExportRecordsAsyncdata = JsonConvert.DeserializeObject(ExportRecordsAsync2);
            Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncdata}");

            var listOfEvents = new List<RedcapEvent>() {
                new RedcapEvent{arm_num = "1", custom_event_label = null, event_name = "Event 1", day_offset = "1", offset_min = "0", offset_max = "0", unique_event_name = "event_1_arm_1" }
            };
            Console.WriteLine("Calling ImportEventsAsync() . . .");
            var ImportEventsAsync = redcap_api.ImportEventsAsync(listOfEvents, Override.False, InputFormat.json, ReturnFormat.json).Result;
            var ImportEventsAsyncData = JsonConvert.DeserializeObject(ImportEventsAsync);
            Console.WriteLine($"ImportEventsAsync Result: {ImportEventsAsyncData}");

            var pathExport = "C:\\redcap_download_files";
            Console.WriteLine("Calling ExportFile() . . .");
            var ExportFile = redcap_api.ExportFileAsync("1","protocol_upload", "1_arm_1", "", pathExport, ReturnFormat.json).Result;
            Console.WriteLine($"ExportFile Result: {ExportFile} to : {pathExport}");

            var pathImport = "C:\\redcap_download_files";
            string importFileName = "REDCapTraining2";
            Console.WriteLine("Calling ImportportFile() . . .");
            var ImportFile = redcap_api.ImportFileAsync("1", "protocol_upload", "1_arm_1", "", importFileName, pathImport, ReturnFormat.json).Result;
            Console.WriteLine($"ImportportFile Result: {ImportFile} to : {pathImport}");

            Console.ReadLine();

        }
    }
}
