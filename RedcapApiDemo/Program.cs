using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedcapApiDemo
{
    public class Demographic
    {
        [JsonRequired]
        [JsonProperty("record_id")]
        public string RecordId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
    class Program
    {
        /*
         * Change this token for your demo project
         * Using one created from a local dev instance
         */
        private const string _token = "4AAE216218B33700456A30898F2D6417";
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
            var redcapApi = new RedcapApi(_token, _uri);

            Console.WriteLine("Calling API Methods < 1.0.0");

            Console.WriteLine("Calling GetRecordAsync() . . .");
            var GetRecordAsync = redcapApi.GetRecordAsync("1", ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, null, null, null, null).Result;
            var GetRecordAsyncData = JsonConvert.DeserializeObject(GetRecordAsync);
            Console.WriteLine($"GetRecordAsync Result: {GetRecordAsyncData}");

            Console.WriteLine("Calling ExportEventsAsync() . . .");
            var exportEvents = redcapApi.ExportEventsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            var exportEventsAsync = JsonConvert.DeserializeObject(exportEvents);
            Console.WriteLine($"ExportEventsAsync Result: {exportEventsAsync}");

            Console.WriteLine("Calling GetRecordsAsync() . . .");
            var GetRecordsAsync = redcapApi.GetRecordsAsync(ReturnFormat.json, OnErrorFormat.json, RedcapDataType.flat).Result;
            var GetRecordsAsyncData = JsonConvert.DeserializeObject(GetRecordsAsync);
            Console.WriteLine($"GetRecordsAsync Result: {GetRecordsAsyncData}");

            Console.WriteLine("Calling GetRedcapVersionAsync() . . .");
            var GetRedcapVersionAsync = redcapApi.GetRedcapVersionAsync(ReturnFormat.json, RedcapDataType.flat).Result;
            Console.WriteLine($"GetRedcapVersionAsync Result: {GetRedcapVersionAsync}");


            var saveRecordsAsyncObject = new
            {
                record_id = "1",
                redcap_event_name = "event_1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };

            Console.WriteLine("Calling SaveRecordsAsync() . . .");
            var SaveRecordsAsync = redcapApi.SaveRecordsAsync(saveRecordsAsyncObject, ReturnContent.ids, OverwriteBehavior.overwrite, ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json).Result;
            var SaveRecordsAsyncData = JsonConvert.DeserializeObject(SaveRecordsAsync);
            Console.WriteLine($"SaveRecordsAsync Result: {SaveRecordsAsyncData}");


            //Console.WriteLine("Calling ExportRecordsAsync() . . .");
            //var ExportRecordsAsync = redcapApi.ExportRecordsAsync(ReturnFormat.json, RedcapDataType.flat).Result;
            //var ExportRecordsAsyncData = JsonConvert.DeserializeObject(ExportRecordsAsync);
            //Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncData}");

            Console.WriteLine("Calling ExportArmsAsync() . . .");
            var ExportArmsAsync = redcapApi.ExportArmsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            var ExportArmsAsyncData = JsonConvert.DeserializeObject(ExportArmsAsync);
            Console.WriteLine($"ExportArmsAsync Result: {ExportArmsAsyncData}");

            //Console.WriteLine("Calling ExportRecordsAsync() . . .");
            //var ExportRecordsAsync2 = redcapApi.ExportRecordsAsync(ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, null, "research_opportunities", "event1_arm1", "cda_check,info_check,protocol_check,synopsis_check,feasquestion_check").Result;
            //var ExportRecordsAsyncdata = JsonConvert.DeserializeObject(ExportRecordsAsync2);
            //Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncdata}");

            var listOfEvents = new List<RedcapEvent>() {
                new RedcapEvent{arm_num = "1", custom_event_label = null, event_name = "Event 1", day_offset = "1", offset_min = "0", offset_max = "0", unique_event_name = "event_1_arm_1" }
            };
            Console.WriteLine("Calling ImportEventsAsync() . . .");
            var ImportEventsAsync = redcapApi.ImportEventsAsync(listOfEvents, Override.False, ReturnFormat.json, OnErrorFormat.json).Result;
            var ImportEventsAsyncData = JsonConvert.DeserializeObject(ImportEventsAsync);
            Console.WriteLine($"ImportEventsAsync Result: {ImportEventsAsyncData}");

            var pathImport = "C:\\redcap_download_files";
            string importFileName = "test.txt";
            var pathExport = "C:\\redcap_download_files";
            var record = "1";
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";
            var repeatingInstrument = "1";

            Console.WriteLine("Calling ImportFile() . . .");
            var ImportFile = redcapApi.ImportFileAsync(_uri, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, importFileName, pathImport, OnErrorFormat.json).Result;
            Console.WriteLine($"File has been imported! To verify, field history!");

            Console.WriteLine("Calling ExportFile() . . .");
            var ExportFile = redcapApi.ExportFileAsync(_uri, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, OnErrorFormat.json, pathExport).Result;
            Console.WriteLine($"ExportFile Result: {ExportFile} to : {pathExport}");

            Console.WriteLine("Calling DeleteFile() . . .");
            var DeleteFile = redcapApi.DeleteFileAsync("1", "protocol_upload", "event_1_arm_1", "", OnErrorFormat.json).Result;
            Console.WriteLine($"File has been deleted! To verify, field history!");

            Console.WriteLine("Calls to < 1.0.0 completed...");
            // Make a sound!
            Console.Beep();




            Console.WriteLine("-----------------------------Starting API Version 1.0.2-------------");
            Console.WriteLine("Starting demo for API Version 1.0.0+");

            Console.ReadLine();
            Console.WriteLine("----------------------------Press Enter to Continue-------------");







            Console.WriteLine("Creating a new instance of RedcapApi");
            var redcap_api_1_0_2 = new RedcapApi(_uri);

            #region ImportRecordsAsync()
            Console.WriteLine("Calling ImportRecordsAsync() . . .");
            /*
             * Create a list of object of type instrument or fields. Add its properties then add it to the list.
             * record_id is required
             */ 
            var data = new List<Demographic> { new Demographic {FirstName = "Jon", LastName = "Doe", RecordId = "1" } };
            Console.WriteLine($"Importing record {string.Join(",", data.Select(x=>x.RecordId).ToList())} . . .");
            var ImportRecordsAsync = redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json).Result;
            var ImportRecordsAsyncData = JsonConvert.DeserializeObject(ImportRecordsAsync);
            Console.WriteLine($"ImportRecordsAsync Result: {ImportRecordsAsyncData}");
            #endregion ImportRecordsAsync()


            #region DeleteRecordsAsync()
            Console.WriteLine("Calling DeleteRecordsAsync() . . .");
            var records = new string[] { "1" };
            Console.WriteLine($"Deleting record {string.Join(",", records)} . . .");
            var DeleteRecordsAsync = redcapApi.DeleteRecordsAsync(_token, Content.Record, RedcapAction.Delete, records, 1).Result;
            var DeleteRecordsAsyncData = JsonConvert.DeserializeObject(DeleteRecordsAsync);
            Console.WriteLine($"DeleteRecordsAsync Result: {DeleteRecordsAsyncData}");
            #endregion DeleteRecordsAsync()

            #region ExportArmsAsync()
            var arms = new string[] {};
            Console.WriteLine("Calling ExportArmsAsync()");
            var ExportArmsAsyncResult = redcap_api_1_0_2.ExportArmsAsync(_token, Content.Arm, ReturnFormat.json, arms, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportArmsAsyncResult: {ExportArmsAsyncResult}");
            #endregion ExportArmsAsync()

            #region ImportArmsAsync()
            var ImportArmsAsyncData = new List<RedcapArm>{ new RedcapArm {arm_num = "1", name = "hooo" }, new RedcapArm { arm_num = "2", name = "heee" }, new RedcapArm { arm_num = "3", name = "hawww" } };
            Console.WriteLine("Calling ImportArmsAsync()");
            var ImportArmsAsyncResult = redcap_api_1_0_2.ImportArmsAsync(_token, Content.Arm, Override.False, RedcapAction.Import, ReturnFormat.json, ImportArmsAsyncData, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportArmsAsyncResult: {ImportArmsAsyncResult}");
            #endregion ImportArmsAsync()

            #region DeleteArmsAsync()
            var DeleteArmsAsyncData = new string[] {"3"};            
            Console.WriteLine("Calling DeleteArmsAsync()");
            var DeleteArmsAsyncResult = redcap_api_1_0_2.DeleteArmsAsync(_token, Content.Arm, RedcapAction.Delete, DeleteArmsAsyncData).Result;
            Console.WriteLine($"DeleteArmsAsyncResult: {DeleteArmsAsyncResult}");
            #endregion DeleteArmsAsync()

            #region ExportEventsAsync()
            var ExportEventsAsyncData = new string[] { "1" };
            Console.WriteLine("Calling ExportEventsAsync()");
            var ExportEventsAsyncResult = redcap_api_1_0_2.ExportEventsAsync(_token, Content.Event, ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json).Result;
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
            var ImportEventsAsyncResult = redcap_api_1_0_2.ImportEventsAsync(_token, Content.Event, RedcapAction.Import, Override.False, ReturnFormat.json, eventList, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportEventsAsyncResult: {ImportEventsAsyncResult}");
            #endregion ImportEventsAsync()

            #region DeleteEventsAsync()
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };
            Console.WriteLine("Calling DeleteEventsAsync()");
            var DeleteEventsAsyncResult = redcap_api_1_0_2.DeleteEventsAsync(_token, Content.Event, RedcapAction.Delete, DeleteEventsAsyncData ).Result;
            Console.WriteLine($"DeleteEventsAsyncResult: {DeleteEventsAsyncResult}");
            #endregion DeleteEventsAsync()

            #region ExportFieldNamesAsync()
            Console.WriteLine("Calling ExportFieldNamesAsync(), first_name");
            var ExportFieldNamesAsyncResult = redcap_api_1_0_2.ExportFieldNamesAsync(_token, Content.ExportFieldNames, ReturnFormat.json, "first_name", OnErrorFormat.json).Result;
            Console.WriteLine($"ExportFieldNamesAsyncResult: {ExportFieldNamesAsyncResult}");
            #endregion ExportFieldNamesAsync()

            #region ImportFileAsync()
            var recordId = "1";
            var fileName = "test.txt";
            var fileUploadPath = @"C:\redcap_upload_files";
            Console.WriteLine($"Calling ImportFileAsync(), {fileName}");
            var ImportFileAsyncResult = redcap_api_1_0_2.ImportFileAsync(_token, Content.File, RedcapAction.Import, recordId, fieldName, eventName, null, fileName, fileUploadPath, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportFileAsyncResult: {ImportFileAsyncResult}");
            #endregion ImportFileAsync()


            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, not save the file.");
            var ExportFileAsyncResult = redcap_api_1_0_2.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportFileAsyncResult: {ExportFileAsyncResult}");
            #endregion ExportFileAsync()


            #region ExportFileAsync()
            var filedDownloadPath = @"C:\redcap_download_files";
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, saving the file.");
            var ExportFileAsyncResult2 = redcap_api_1_0_2.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, OnErrorFormat.json, filedDownloadPath).Result;
            Console.WriteLine($"ExportFileAsyncResult2: {ExportFileAsyncResult2}");
            #endregion ExportFileAsync()

            #region DeleteFileAsync()
            Console.WriteLine($"Calling DeleteFileAsync(), deleting file: {fileName} for field: {fieldName}");
            var DeleteFileAsyncResult = redcap_api_1_0_2.DeleteFileAsync(_token, Content.File, RedcapAction.Delete, recordId, fieldName, eventName, "1", OnErrorFormat.json).Result;
            Console.WriteLine($"DeleteFileAsyncResult: {DeleteFileAsyncResult}");
            #endregion DeleteFileAsync()

            #region ExportInstrumentsAsync()
            Console.WriteLine($"Calling DeleteFileAsync()");
            var ExportInstrumentsAsyncResult = redcap_api_1_0_2.ExportInstrumentsAsync(_token, Content.Instrument, ReturnFormat.json).Result;
            Console.WriteLine($"ExportInstrumentsAsyncResult: {ExportInstrumentsAsyncResult}");
            #endregion ExportInstrumentsAsync()

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), returns raw");
            var ExportPDFInstrumentsAsyncResult = redcap_api_1_0_2.ExportPDFInstrumentsAsync(_token, Content.Pdf, recordId, eventName, "demographics", true, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportInstrumentsAsyncResult: {JsonConvert.SerializeObject(ExportPDFInstrumentsAsyncResult)}");
            #endregion ExportPDFInstrumentsAsync()


            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), saving pdf file to {filedDownloadPath}");
            var ExportPDFInstrumentsAsyncResult2 = redcap_api_1_0_2.ExportPDFInstrumentsAsync(_token, Content.Pdf, recordId, eventName, "demographics", true, filedDownloadPath, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportPDFInstrumentsAsyncResult2: {ExportPDFInstrumentsAsyncResult2}");
            #endregion ExportPDFInstrumentsAsync()


            #region ExportInstrumentMappingAsync()
            Console.WriteLine($"Calling ExportInstrumentMappingAsync()");
            var ExportInstrumentMappingAsyncResult = redcap_api_1_0_2.ExportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, arms, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportInstrumentMappingAsyncResult: {ExportInstrumentMappingAsyncResult}");
            #endregion ExportInstrumentMappingAsync()

            #region ImportInstrumentMappingAsync()
            var importInstrumentMappingData = new List<FormEventMapping>{new FormEventMapping {arm_num = "1", unique_event_name = "clinical_arm_1", form= "demographics" } };
            Console.WriteLine($"Calling ImportInstrumentMappingAsync()");
            var ImportInstrumentMappingAsyncResult = redcap_api_1_0_2.ImportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, importInstrumentMappingData, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportInstrumentMappingAsyncResult: {ImportInstrumentMappingAsyncResult}");
            #endregion ImportInstrumentMappingAsync()

            #region ExportMetaDataAsync()
            Console.WriteLine($"Calling ExportMetaDataAsync()");
            var ExportMetaDataAsyncResult = redcap_api_1_0_2.ExportMetaDataAsync(_token, Content.MetaData, ReturnFormat.json, null, null, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportMetaDataAsyncResult: {ExportMetaDataAsyncResult}");
            #endregion ExportMetaDataAsync()

            #region ImportMetaDataAsync()
            /*
             * This imports 1 field into the data dictionary
             */
            var importMetaData = new List<RedcapMetaData> { new RedcapMetaData { field_name = "first_name", form_name = "demographics", field_type = "text", field_label = "First Name" } };
            Console.WriteLine($"Not calling ImportMetaDataAsync(), still change data dictionary to include 1 field");
            //var ImportMetaDataAsyncResult = redcap_api_1_0_2.ImportMetaDataAsync(_token, "metadata", ReturnFormat.json, importMetaData, OnErrorFormat.json).Result;
            //Console.WriteLine($"ImportMetaDataAsyncResult: {ImportMetaDataAsyncResult}");
            #endregion ImportMetaDataAsync()

            #region CreateProjectAsync()
            var projectData = new List<RedcapProject> { new RedcapProject {project_title = "Amazing Project ", purpose = ProjectPurpose.Other, purpose_other = "Test"} };
            Console.WriteLine($"Calling CreateProjectAsync(), creating a new project with Amazing Project as title, purpose 1 (other) ");
            Console.WriteLine($"-----------------------Notice the use of SUPER TOKEN------------------------");
            var CreateProjectAsyncResult = redcap_api_1_0_2.CreateProjectAsync(_superToken,Content.Project, ReturnFormat.json, projectData, OnErrorFormat.json, null).Result;
            Console.WriteLine($"CreateProjectAsyncResult: {CreateProjectAsyncResult}");
            #endregion CreateProjectAsync()


            #region ImportProjectInfoAsync()
            var projectInfo = new RedcapProjectInfo { project_title = "Updated Amazing Project ", purpose = ProjectPurpose.QualityImprovement, surveys_enabled = 1 };
            Console.WriteLine($"Calling ImportProjectInfoAsync()");
            var ImportProjectInfoAsyncResult = redcap_api_1_0_2.ImportProjectInfoAsync(_token, Content.ProjectSettings, ReturnFormat.json, projectInfo).Result;
            Console.WriteLine($"ImportProjectInfoAsyncResult: {ImportProjectInfoAsyncResult}");
            #endregion ImportProjectInfoAsync()


            #region ExportProjectInfoAsync()
            Console.WriteLine($"Calling ExportProjectInfoAsync()");
            var ExportProjectInfoAsyncResult = redcap_api_1_0_2.ExportProjectInfoAsync(_token, Content.ProjectSettings, ReturnFormat.json).Result;
            Console.WriteLine($"ExportProjectInfoAsyncResult: {ExportProjectInfoAsyncResult}");
            #endregion ExportProjectInfoAsync()


            Console.ReadLine();

        }
    }
}
