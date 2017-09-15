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
            
            Console.WriteLine("Redcap Api Demo Started!");
            var _apiToken = "3D57A7FA57C8A43F6C8803A84BB3957B";
            if (true) _apiToken = "BCC9D1F214B8BE2AA4F24C56ED7674E4";
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
            var ExportArmsAsync = redcap_api.ExportArmsAsync(InputFormat.json, ReturnFormat.json, new List<string> { }).Result;
            var ExportArmsAsyncData = JsonConvert.DeserializeObject(ExportArmsAsync);
            Console.WriteLine($"ExportArmsAsync Result: {ExportArmsAsyncData}");


            Console.ReadLine();

        }
    }
}
