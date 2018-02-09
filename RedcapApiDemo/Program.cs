using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            /**
             * This is a demo.
             * This program sequently runs through all the APIs.
             * Go into Redcap and create a new project with demographics.
             * Turn on longitudinal and add two additional event.
             * Event name should be "Event 1, Event 2, Event 3"
             * Create a folder in C: , name it redcap_download_files
             * Create a text file in that folder, save it as test.txt
             * Add a field, field type file upload to the project, name it "protocol_upload"
             * This allows the upload file method to upload files
             * 
             * */

            Console.WriteLine("Redcap Api Demo Started!");
            // change api token for your demo project
            var _apiToken = "ED2D0A2E34D9693DCA7E9E6BD5F0941C";

            var redcapApi = new RedcapApi(_apiToken, "http://localhost/redcap/api/");

            Console.WriteLine("Calling GetRecordAsync() . . .");
            var GetRecordAsync = redcapApi.GetRecordAsync("1", InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            var GetRecordAsyncData = JsonConvert.DeserializeObject(GetRecordAsync);
            Console.WriteLine($"GetRecordAsync Result: {GetRecordAsyncData}");

            Console.WriteLine("Calling ExportEventsAsync() . . .");
            var exportEvents = redcapApi.ExportEventsAsync(InputFormat.json, ReturnFormat.json).Result;
            var exportEventsAsync = JsonConvert.DeserializeObject(exportEvents);
            Console.WriteLine($"ExportEventsAsync Result: {exportEventsAsync}");

            Console.WriteLine("Calling GetRecordsAsync() . . .");
            var GetRecordsAsync = redcapApi.GetRecordsAsync(InputFormat.json, ReturnFormat.json, RedcapDataType.flat).Result;
            var GetRecordsAsyncData = JsonConvert.DeserializeObject(GetRecordsAsync);
            Console.WriteLine($"GetRecordsAsync Result: {GetRecordsAsyncData}");

            Console.WriteLine("Calling GetRedcapVersionAsync() . . .");
            var GetRedcapVersionAsync = redcapApi.GetRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            Console.WriteLine($"GetRedcapVersionAsync Result: {GetRedcapVersionAsync}");


            var saveRecordsAsyncObject = new
            {
                record_id = "1",
                redcap_event_name = "event_1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };

            Console.WriteLine("Calling SaveRecordsAsync() . . .");
            var SaveRecordsAsync = redcapApi.SaveRecordsAsync(saveRecordsAsyncObject, ReturnContent.ids, OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json).Result;
            var SaveRecordsAsyncData = JsonConvert.DeserializeObject(SaveRecordsAsync);
            Console.WriteLine($"SaveRecordsAsync Result: {SaveRecordsAsyncData}");


            Console.WriteLine("Calling ExportRecordsAsync() . . .");
            var ExportRecordsAsync = redcapApi.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat).Result;
            var ExportRecordsAsyncData = JsonConvert.DeserializeObject(ExportRecordsAsync);
            Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncData}");

            Console.WriteLine("Calling ExportArmsAsync() . . .");
            var ExportArmsAsync = redcapApi.ExportArmsAsync(InputFormat.json, ReturnFormat.json).Result;
            var ExportArmsAsyncData = JsonConvert.DeserializeObject(ExportArmsAsync);
            Console.WriteLine($"ExportArmsAsync Result: {ExportArmsAsyncData}");

            Console.WriteLine("Calling ExportRecordsAsync() . . .");
            var ExportRecordsAsync2 = redcapApi.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, "research_opportunities", "event1_arm1", "cda_check,info_check,protocol_check,synopsis_check,feasquestion_check").Result;
            var ExportRecordsAsyncdata = JsonConvert.DeserializeObject(ExportRecordsAsync2);
            Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncdata}");

            var listOfEvents = new List<RedcapEvent>() {
                new RedcapEvent{arm_num = "1", custom_event_label = null, event_name = "Event 1", day_offset = "1", offset_min = "0", offset_max = "0", unique_event_name = "event_1_arm_1" }
            };
            Console.WriteLine("Calling ImportEventsAsync() . . .");
            var ImportEventsAsync = redcapApi.ImportEventsAsync(listOfEvents, Override.False, InputFormat.json, ReturnFormat.json).Result;
            var ImportEventsAsyncData = JsonConvert.DeserializeObject(ImportEventsAsync);
            Console.WriteLine($"ImportEventsAsync Result: {ImportEventsAsyncData}");

            var pathImport = "C:\\redcap_download_files";
            string importFileName = "test.txt";
            Console.WriteLine("Calling ImportportFile() . . .");
            var ImportFile = redcapApi.ImportFileAsync("1", "protocol_upload", "Event_1_arm_1", "", importFileName, pathImport, ReturnFormat.json).Result;
            Console.WriteLine($"File has been imported!");

            var pathExport = "C:\\redcap_download_files";
            Console.WriteLine("Calling ExportFile() . . .");
            var ExportFile = redcapApi.ExportFileAsync("1", "protocol_upload", "Event_1_arm_1", "", pathExport, ReturnFormat.json).Result;
            Console.WriteLine($"ExportFile Result: {ExportFile} to : {pathExport}");

            Console.WriteLine("Calling DeleteFile() . . .");
            var DeleteFile = redcapApi.DeleteFileAsync("1", "protocol_upload", "Event_1_arm_1", "", ReturnFormat.json).Result;
            Console.WriteLine($"File has been deleted!");

            Console.ReadLine();

        }
    }
}
