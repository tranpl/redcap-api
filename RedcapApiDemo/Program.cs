using System;
using System.Collections.Generic;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        /*
         * Change this token for your demo project
         * Using one created from a local dev instance
         */
        private const string _token = "A8E6949EF4380F1111C66D5374E1AE6C";
        /*
         * Change this token for your demo project
         * Using one created from a local dev instance
        */
        private const string _superToken = "92F719F0EC97783D06B0E0FF49DC42DDA2247BFDC6759F324EE0D710FCA87C33";
        /*
         * Using a local redcap development instsance
         */
        private const string _uri = "http://localhost/redcap/api/";
        static void Main(string[] args)
        {
            /*
             * This is a demo. This program provides a demonstration of potential calls using the API library.
             * 
             * This program sequently runs through all the APIs methods.
             * 
             * Directions:
             * 
             * 1. Go into Redcap and create a new project with demographics.
             * 2. Turn on longitudinal and add two additional event. Event name should be "Event 1, Event 2, Event 3"
             *  Important, make sure you designate the instrument to atleast one event
             * 3. Create a folder in C: , name it redcap_download_files
             * 4. Create a text file in that folder, save it as test.txt
             * 5. Add a field, field type file upload to the project, name it "protocol_upload"
             * This allows the upload file method to upload files
             * 
             */

            /*
             * Output to console
             */ 
            Console.WriteLine("Starting Redcap Api Demo..");
            Console.WriteLine("Please make sure you include a working redcap api token.");

            /*
             * Start a new instance of Redcap APi
             */ 
            //var redcapApi = new RedcapApi(_token, _uri);

            //Console.WriteLine("Calling API Methods < 1.0.0");
            //Console.WriteLine("Calling GetRecordAsync() . . .");
            //var GetRecordAsync = redcapApi.GetRecordAsync("1", InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            //var GetRecordAsyncData = JsonConvert.DeserializeObject(GetRecordAsync);
            //Console.WriteLine($"GetRecordAsync Result: {GetRecordAsyncData}");

            //Console.WriteLine("Calling ExportEventsAsync() . . .");
            //var exportEvents = redcapApi.ExportEventsAsync(InputFormat.json, ReturnFormat.json).Result;
            //var exportEventsAsync = JsonConvert.DeserializeObject(exportEvents);
            //Console.WriteLine($"ExportEventsAsync Result: {exportEventsAsync}");

            //Console.WriteLine("Calling GetRecordsAsync() . . .");
            //var GetRecordsAsync = redcapApi.GetRecordsAsync(InputFormat.json, ReturnFormat.json, RedcapDataType.flat).Result;
            //var GetRecordsAsyncData = JsonConvert.DeserializeObject(GetRecordsAsync);
            //Console.WriteLine($"GetRecordsAsync Result: {GetRecordsAsyncData}");

            //Console.WriteLine("Calling GetRedcapVersionAsync() . . .");
            //var GetRedcapVersionAsync = redcapApi.GetRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            //Console.WriteLine($"GetRedcapVersionAsync Result: {GetRedcapVersionAsync}");


            //var saveRecordsAsyncObject = new
            //{
            //    record_id = "1",
            //    redcap_event_name = "event_1_arm_1",
            //    first_name = "John",
            //    last_name = "Doe"
            //};

            //Console.WriteLine("Calling SaveRecordsAsync() . . .");
            //var SaveRecordsAsync = redcapApi.SaveRecordsAsync(saveRecordsAsyncObject, ReturnContent.ids, OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json).Result;
            //var SaveRecordsAsyncData = JsonConvert.DeserializeObject(SaveRecordsAsync);
            //Console.WriteLine($"SaveRecordsAsync Result: {SaveRecordsAsyncData}");


            //Console.WriteLine("Calling ExportRecordsAsync() . . .");
            //var ExportRecordsAsync = redcapApi.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat).Result;
            //var ExportRecordsAsyncData = JsonConvert.DeserializeObject(ExportRecordsAsync);
            //Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncData}");

            //Console.WriteLine("Calling ExportArmsAsync() . . .");
            //var ExportArmsAsync = redcapApi.ExportArmsAsync(InputFormat.json, ReturnFormat.json).Result;
            //var ExportArmsAsyncData = JsonConvert.DeserializeObject(ExportArmsAsync);
            //Console.WriteLine($"ExportArmsAsync Result: {ExportArmsAsyncData}");

            //Console.WriteLine("Calling ExportRecordsAsync() . . .");
            //var ExportRecordsAsync2 = redcapApi.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, "research_opportunities", "event1_arm1", "cda_check,info_check,protocol_check,synopsis_check,feasquestion_check").Result;
            //var ExportRecordsAsyncdata = JsonConvert.DeserializeObject(ExportRecordsAsync2);
            //Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncdata}");

            //var listOfEvents = new List<RedcapEvent>() {
            //    new RedcapEvent{arm_num = "1", custom_event_label = null, event_name = "Event 1", day_offset = "1", offset_min = "0", offset_max = "0", unique_event_name = "event_1_arm_1" }
            //};
            //Console.WriteLine("Calling ImportEventsAsync() . . .");
            //var ImportEventsAsync = redcapApi.ImportEventsAsync(listOfEvents, Override.False, InputFormat.json, ReturnFormat.json).Result;
            //var ImportEventsAsyncData = JsonConvert.DeserializeObject(ImportEventsAsync);
            //Console.WriteLine($"ImportEventsAsync Result: {ImportEventsAsyncData}");

            //var pathImport = "C:\\redcap_download_files";
            //string importFileName = "test.txt";
            //Console.WriteLine("Calling ImportFile() . . .");
            //var ImportFile = redcapApi.ImportFileAsync("1", "protocol_upload", "event_1_arm_1", "", importFileName, pathImport, ReturnFormat.json).Result;
            //Console.WriteLine($"File has been imported! To verify, field history!");

            //var pathExport = "C:\\redcap_download_files";
            //Console.WriteLine("Calling ExportFile() . . .");
            //var ExportFile = redcapApi.ExportFileAsync("1", "protocol_upload", "event_1_arm_1", "", pathExport, ReturnFormat.json).Result;
            //Console.WriteLine($"ExportFile Result: {ExportFile} to : {pathExport}");

            //Console.WriteLine("Calling DeleteFile() . . .");
            //var DeleteFile = redcapApi.DeleteFileAsync("1", "protocol_upload", "event_1_arm_1", "", ReturnFormat.json).Result;
            //Console.WriteLine($"File has been deleted! To verify, field history!" );

            //Console.WriteLine("Calls to < 1.0.0 completed...");
            //// Make a sound!
            //Console.Beep();

            Console.WriteLine("Starting demo for API Version 1.0.0");

            Console.WriteLine("Creating a new instance of RedcapApi");
            var redcap_api_1_0_0 = new RedcapApi(_uri);

            #region ExportArmsAsync()
            var arms = new string[] {};
            Console.WriteLine("Calling ExportArmsAsync()");
            var ExportArmsAsyncResult = redcap_api_1_0_0.ExportArmsAsync(_token, "arm", ReturnFormat.json, arms, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportArmsAsyncResult: {ExportArmsAsyncResult}");
            #endregion ExportArmsAsync()

            #region ImportArmsAsync()
            var ImportArmsAsyncData = new List<RedcapArm>{ new RedcapArm {arm_num = "1", name = "hooo" }, new RedcapArm { arm_num = "2", name = "heee" }, new RedcapArm { arm_num = "3", name = "hawww" } };
            Console.WriteLine("Calling ImportArmsAsync()");
            var ImportArmsAsyncResult = redcap_api_1_0_0.ImportArmsAsync(_token, "arm", Override.False, null, ReturnFormat.json, ImportArmsAsyncData, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportArmsAsyncResult: {ImportArmsAsyncResult}");
            #endregion ImportArmsAsync()


            #region DeleteArmsAsync()
            var DeleteArmsAsyncData = new string[] {"3"};            
            Console.WriteLine("Calling DeleteArmsAsync()");
            var DeleteArmsAsyncResult = redcap_api_1_0_0.DeleteArmsAsync(_token, "arm", "delete", DeleteArmsAsyncData).Result;
            Console.WriteLine($"DeleteArmsAsyncResult: {DeleteArmsAsyncResult}");
            #endregion DeleteArmsAsync()

            #region ExportEventsAsync()
            var ExportEventsAsyncData = new string[] { "1" };
            Console.WriteLine("Calling ExportEventsAsync()");
            var ExportEventsAsyncResult = redcap_api_1_0_0.ExportEventsAsync(_token, "event", ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportEventsAsyncResult: {ExportEventsAsyncResult}");
            #endregion ExportEventsAsync()

            #region ImportEventsAsync()
            Console.WriteLine("Calling ExportEventsAsync()");
            var eventList = new List<RedcapEvent> {
                new RedcapEvent {
                    event_name = "baseline",
                    arm_num = "1",
                    day_offset = "1",
                    offset_min = "0",
                    offset_max = "0",
                    unique_event_name = "baseline_arm_1",
                    custom_event_label = "hello baseline"
                },
                new RedcapEvent {
                    event_name = "clinical",
                    arm_num = "1",
                    day_offset = "1",
                    offset_min = "0",
                    offset_max = "0",
                    unique_event_name = "clinical_arm_1",
                    custom_event_label = "hello clinical"
                }
            };
            var ImportEventsAsyncResult = redcap_api_1_0_0.ImportEventsAsync(_token, "event", "import", Override.False, ReturnFormat.json, eventList, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportEventsAsyncResult: {ImportEventsAsyncResult}");
            #endregion ImportEventsAsync()

            #region DeleteEventsAsync()
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };
            Console.WriteLine("Calling DeleteEventsAsync()");
            var DeleteEventsAsyncResult = redcap_api_1_0_0.DeleteEventsAsync(_token, "event", "delete", DeleteEventsAsyncData ).Result;
            Console.WriteLine($"DeleteEventsAsyncResult: {DeleteEventsAsyncResult}");
            #endregion DeleteEventsAsync()

            #region ExportFieldNamesAsync()
            Console.WriteLine("Calling ExportFieldNamesAsync(), first_name");
            var ExportFieldNamesAsyncResult = redcap_api_1_0_0.ExportFieldNamesAsync(_token, "exportFieldNames", ReturnFormat.json, "first_name", OnErrorFormat.json).Result;
            Console.WriteLine($"ExportFieldNamesAsyncResult: {ExportFieldNamesAsyncResult}");
            #endregion ExportFieldNamesAsync()

            #region ImportFileAsync()
            var recordId = "1";
            var fieldName = "protocol_upload";
            var fileName = "test.txt";
            var fileUploadPath = @"C:\redcap_upload_files";
            Console.WriteLine($"Calling ImportFileAsync(), {fileName}");
            var ImportFileAsyncResult = redcap_api_1_0_0.ImportFileAsync(_token, "file", "import", recordId, fieldName, "clinical_arm_1", null, fileName, fileUploadPath, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportFileAsyncResult: {ImportFileAsyncResult}");
            #endregion ImportFileAsync()


            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, does not save the file.");
            var ExportFileAsyncResult = redcap_api_1_0_0.ExportFileAsync(_token, "file", "export", recordId, fieldName, "clinical_arm_1", null, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportFileAsyncResult: {ExportFileAsyncResult}");
            #endregion ExportFileAsync()


            #region ExportFileAsync()
            var filedDownloadPath = @"C:\redcap_download_files";
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, saving the file.");
            var ExportFileAsyncResult2 = redcap_api_1_0_0.ExportFileAsync(_token, "file", "export", recordId, fieldName, "clinical_arm_1", null, OnErrorFormat.json, filedDownloadPath).Result;
            Console.WriteLine($"ExportFileAsyncResult2: {ExportFileAsyncResult2}");
            #endregion ExportFileAsync()


            Console.ReadLine();

        }
    }
}
