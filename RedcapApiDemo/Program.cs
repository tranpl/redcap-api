using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Redcap;
using Redcap.Models;

using RedcapApiDemo.Utilities;

namespace RedcapApiDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*
             * This is a demo. This program provides a demonstration of potential calls using the API library.
             * 
             * This program runs through all the APIs methods sequentially.
             * 
             * Directions:
             * 
             * 1. Go into your Redcap instance and create a new project with demographics form.
             * 2. Turn on longitudinal and add two additional event. Event name should be "Event 1, Event 2, Event 3"
             *  Important, make sure you designate the instrument to atleast one event
             * 3. Create a folder in C: , name it redcap_download_files
             * 4. Create a text file in that folder, save it as test.txt
             * 5. Add a field in the project that was created; the field type should contain file upload, name it "protocol_upload"
             * This allows the upload file method to upload files
             * 
             */
            await InitializeDemo();
        }
        static async Task InitializeDemo()
        {
            /*
             * Change this token for your demo project
             * Using one created from a local dev instance
             */
            string _token = string.Empty;
            /*
             * Change this token for your demo project
             * Using one created from a local dev instance
            */
            string _superToken = "2E59CA118ABC17D393722524C501CF0BAC51689746E24BFDAF47B38798BD827A";
            /*
             * Using a local redcap development instsance
             */
            string _uri = string.Empty;
            var fieldName = "file_upload";
            var eventName = "event_1_arm_1";

            /*
            * Output to console
            */
            Console.WriteLine("Starting Redcap Api Demo..");
            Console.WriteLine("Please make sure redcap is running locally.");
            Console.WriteLine("Please make sure you include a working redcap api token.");
            Console.WriteLine("Enter your redcap instance uri, example: http://localhost/redcap");
            _uri = Console.ReadLine();
            if (string.IsNullOrEmpty(_uri))
            {
                // provide a default one here..
                _uri = "http://localhost/redcap";
            }
            _uri = _uri + "/api/";
            Console.WriteLine("Enter your api token for the project to test: ");
            var token = Console.ReadLine();

            if (string.IsNullOrEmpty(token))
            {
                _token = "DF70F2EC94AE05021F66423B386095BD";
            }
            else
            {
                _token = token;
            }
            Console.WriteLine($"Using Endpoint=> {_uri} Token => {_token}");

            Console.WriteLine("-----------------------------Starting API Demonstration-------------");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine("Creating a new instance of RedcapApi");
            var redcapApi = new RedcapApi(_uri);
            Console.WriteLine($"Using {_uri} for redcap api endpoint.");

            #region ExportLoggingAsync()
            Console.WriteLine("Calling ExportLoggingAsync() . . .");
            Console.WriteLine($"Exporting logs for User . . .");
            var ExportLoggingAsync = await redcapApi.ExportLoggingAsync(_token, RedcapContent.Log, RedcapFormat.json, LogType.User);
            Console.WriteLine($"ExportLoggingAsync Results: {JsonConvert.DeserializeObject(ExportLoggingAsync)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion

            #region ImportDagsAsync()
            Console.WriteLine("Calling ImportDagsAsync() . . .");
            Console.WriteLine($"Importing Dags . . .");
            var dags = RedcapDemoUtilities.CreateDags(5);
            var ImportDagsAsyncResult = await redcapApi.ImportDagsAsync(_token, RedcapContent.Dag, RedcapAction.Import, RedcapFormat.json, dags);
            Console.WriteLine($"ImportDagsAsync Results: {JsonConvert.DeserializeObject(ImportDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion

            #region ExportDagsAsync()
            Console.WriteLine("Calling ExportDagsAsync() . . .");
            Console.WriteLine($"Exporting Dags . . .");
            var ExportDagsAsyncResult = await redcapApi.ExportDagsAsync(_token, RedcapContent.Dag, RedcapFormat.json);
            Console.WriteLine($"ExportDagsAsync Results: {JsonConvert.DeserializeObject(ExportDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion

            #region DeleteDagsAsync()
            Console.WriteLine("Calling DeleteDagsAsync() . . .");
            Console.WriteLine($"Deleting Dags . . .");
            var dagsToDelete = JsonConvert.DeserializeObject<List<RedcapDag>>(ExportDagsAsyncResult).Select(x => x.UniqueGroupName).ToArray();
            var DeleteDagsAsyncResult = await redcapApi.DeleteDagsAsync(_token, RedcapContent.Dag, RedcapAction.Delete, dagsToDelete);
            Console.WriteLine($"DeleteDagsAsync Results: {JsonConvert.DeserializeObject(DeleteDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion

            #region ImportRecordsAsync()
            Console.WriteLine("Calling ImportRecordsAsync() . . .");
            // get demographics data
            var importDemographicsData = RedcapDemoUtilities.CreateDemographics(includeBio: true, 5);
            Console.WriteLine("Serializing the data . . .");
            Console.WriteLine($"Importing record {string.Join(",", importDemographicsData.Select(x => x.RecordId).ToList())} . . .");
            var ImportRecordsAsync = await redcapApi.ImportRecordsAsync(_token, RedcapContent.Record, RedcapFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, importDemographicsData, "MDY", CsvDelimiter.tab, ReturnContent.count, RedcapReturnFormat.json);
            var ImportRecordsAsyncData = JsonConvert.DeserializeObject(ImportRecordsAsync);
            Console.WriteLine($"ImportRecordsAsync Result: {ImportRecordsAsyncData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion ImportRecordsAsync()

            #region ExportRecordsAsync()
            Console.WriteLine($"Calling ExportRecordsAsync()");
            Console.WriteLine($"Using records from the imported method..");
            var recordsToExport = importDemographicsData.Select(x => x.RecordId).ToArray();
            var instrumentName = new string[] { "demographics" };
            var ExportRecordsAsyncResult = await redcapApi.ExportRecordsAsync(_token, RedcapContent.Record, RedcapFormat.json, RedcapDataType.flat, recordsToExport, null, instrumentName);
            Console.WriteLine($"ExportRecordsAsyncResult: {ExportRecordsAsyncResult}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion ExportRecordsAsync()

            #region DeleteRecordsAsync()
            Console.WriteLine("Calling DeleteRecordsAsync() . . .");
            var records = importDemographicsData.Select(x => x.RecordId).ToArray();
            Console.WriteLine($"Deleting record {string.Join(",", recordsToExport)} . . .");
            var DeleteRecordsAsync = await redcapApi.DeleteRecordsAsync(_token, RedcapContent.Record, RedcapAction.Delete, recordsToExport, 1);
            var DeleteRecordsAsyncData = JsonConvert.DeserializeObject(DeleteRecordsAsync);
            Console.WriteLine($"DeleteRecordsAsync Result: {DeleteRecordsAsyncData}");

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion DeleteRecordsAsync()

            #region RenameRecordAsync()
            Console.WriteLine("Calling RenameRecordAsync() . . .");
            var recordToRename = importDemographicsData.Select(x => x.RecordId).SingleOrDefault();
            Console.WriteLine($"Renaming record {recordToRename} . . .");
            var newRecordName = "2";
            var RenameRecordAsyncResult = await redcapApi.RenameRecordAsync(_token, RedcapContent.Record, RedcapAction.Rename,recordToRename, newRecordName, 1);
            var RenameRecordAsyncData = JsonConvert.DeserializeObject(RenameRecordAsyncResult);
            Console.WriteLine($"RenameRecordAsync Result: {DeleteRecordsAsyncData}");

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion RenameRecordAsync()



            #region Users & User Priveleges
            Console.WriteLine("Calling ImportUsersAsync() . . .");
            var redcapUser1 = RedcapDemoUtilities.CreateRedcapUser("test1");
            var redcapUser2 = RedcapDemoUtilities.CreateRedcapUser("test2");
            var redcapUsers = new List<RedcapUser>();
            redcapUsers.Add(redcapUser1);
            redcapUsers.Add(redcapUser2);
            Console.WriteLine($"Importing  {redcapUsers.Count} user. . .");
            var ImportUsersAsyncResult = await redcapApi.ImportUsersAsync(_token, redcapUsers, RedcapFormat.json, RedcapReturnFormat.json);
            var ImportUsersAsyncData = JsonConvert.DeserializeObject(ImportUsersAsyncResult);
            Console.WriteLine($"ImportUsersAsync Result: {ImportUsersAsyncData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion  Users & User Priveleges


            #region ExportArmsAsync()
            var arms = new string[] { };
            Console.WriteLine("Calling ExportArmsAsync()");
            var ExportArmsAsyncResult = await redcapApi.ExportArmsAsync(_token, RedcapContent.Arm, RedcapFormat.json, arms, RedcapReturnFormat.json);
            Console.WriteLine($"ExportArmsAsyncResult: {JsonConvert.DeserializeObject(ExportArmsAsyncResult)}");
            #endregion ExportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportArmsAsync()
            var ImportArmsAsyncData = RedcapDemoUtilities.CreateArms(count: 3);
            Console.WriteLine("Calling ImportArmsAsync()");
            var ImportArmsAsyncResult = await redcapApi.ImportArmsAsync(_token, RedcapContent.Arm, Override.False, RedcapAction.Import, RedcapFormat.json, ImportArmsAsyncData, RedcapReturnFormat.json);
            Console.WriteLine($"ImportArmsAsyncResult: {JsonConvert.DeserializeObject(ImportArmsAsyncResult)}");
            #endregion ImportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteArmsAsync()
            var DeleteArmsAsyncData = ImportArmsAsyncData.Select(x => x.ArmNumber).ToArray();
            Console.WriteLine("Calling DeleteArmsAsync()");
            var DeleteArmsAsyncResult = await redcapApi.DeleteArmsAsync(_token, RedcapContent.Arm, RedcapAction.Delete, DeleteArmsAsyncData);
            Console.WriteLine($"DeleteArmsAsyncResult: {JsonConvert.DeserializeObject(DeleteArmsAsyncResult)}");
            #endregion DeleteArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportEventsAsync()
            var ExportEventsAsyncData = new string[] { "1" };
            Console.WriteLine("Calling ExportEventsAsync()");
            var ExportEventsAsyncResult = await redcapApi.ExportEventsAsync(_token, RedcapContent.Event, RedcapFormat.json, ExportEventsAsyncData, RedcapReturnFormat.json);
            Console.WriteLine($"ExportEventsAsyncResult: {JsonConvert.DeserializeObject(ExportEventsAsyncResult)}");
            #endregion ExportEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportEventsAsync()
            Console.WriteLine("Calling ExportEventsAsync()");
            var eventList = new List<RedcapEvent> {
                new RedcapEvent {
                    EventName = "baseline",
                    ArmNumber = "1",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "baseline_arm_1",
                    CustomEventLabel = "hello baseline"
                },
                new RedcapEvent {
                    EventName = "clinical",
                    ArmNumber = "1",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "clinical_arm_1",
                    CustomEventLabel = "hello clinical"
                }
            };
            var ImportEventsAsyncResult = await redcapApi.ImportEventsAsync(_token, RedcapContent.Event, RedcapAction.Import, Override.False, RedcapFormat.json, eventList, RedcapReturnFormat.json);
            Console.WriteLine($"ImportEventsAsyncResult: {ImportEventsAsyncResult}");
            #endregion ImportEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region DeleteEventsAsync()
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };
            Console.WriteLine("Calling DeleteEventsAsync()");
            var DeleteEventsAsyncResult = await redcapApi.DeleteEventsAsync(_token, RedcapContent.Event, RedcapAction.Delete, DeleteEventsAsyncData);
            Console.WriteLine($"DeleteEventsAsyncResult: {DeleteEventsAsyncResult}");
            #endregion DeleteEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region SwitchDagAsync()
            var SwitchDagAsyncData = new RedcapDag { GroupName = "testGroup", UniqueGroupName = "unique_name" };
            Console.WriteLine("Calling SwitchDagAsync()");
            var SwitchDagAsyncResult = await redcapApi.SwitchDagAsync(_token, SwitchDagAsyncData, RedcapContent.Dag, RedcapAction.Switch);
            Console.WriteLine($"SwitchDagAsyncResult: {SwitchDagAsyncResult}");
            #endregion SwitchDagAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();



            #region ExportFieldNamesAsync()
            Console.WriteLine("Calling ExportFieldNamesAsync(), first_name");
            var ExportFieldNamesAsyncResult = await redcapApi.ExportFieldNamesAsync(_token, RedcapContent.ExportFieldNames, RedcapFormat.json, "first_name", RedcapReturnFormat.json);
            Console.WriteLine($"ExportFieldNamesAsyncResult: {ExportFieldNamesAsyncResult}");
            #endregion ExportFieldNamesAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region ImportFileAsync()
            var recordId = "1";
            var fileName = "Demographics_TestProject_DataDictionary.csv";
            DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            string parentDirectory = myDirectory.Parent.FullName;
            var parent = Directory.GetParent(parentDirectory).FullName;
            var filePath = Directory.GetParent(parent).FullName + @"\Docs\";
            Console.WriteLine($"Calling ImportFileAsync(), {fileName}");
            var ImportFileAsyncResult = await redcapApi.ImportFileAsync(_token, RedcapContent.File, RedcapAction.Import, recordId, fieldName, eventName, null, fileName, filePath, RedcapReturnFormat.json);
            Console.WriteLine($"ImportFileAsyncResult: {ImportFileAsyncResult}");
            #endregion ImportFileAsync()


            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, not save the file.");
            var ExportFileAsyncResult = await redcapApi.ExportFileAsync(_token, RedcapContent.File, RedcapAction.Export, recordId, fieldName, eventName, null, RedcapReturnFormat.json);
            Console.WriteLine($"ExportFileAsyncResult: {ExportFileAsyncResult}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            var filedDownloadPath = @"C:\redcap_download_files";
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, saving the file.");
            var ExportFileAsyncResult2 = await redcapApi.ExportFileAsync(_token, RedcapContent.File, RedcapAction.Export, recordId, fieldName, eventName, null, RedcapReturnFormat.json, filedDownloadPath);
            Console.WriteLine($"ExportFileAsyncResult2: {ExportFileAsyncResult2}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteFileAsync()
            Console.WriteLine($"Calling DeleteFileAsync(), deleting file: {fileName} for field: {fieldName}");
            var DeleteFileAsyncResult = await redcapApi.DeleteFileAsync(_token, RedcapContent.File, RedcapAction.Delete, recordId, fieldName, eventName, "1", RedcapReturnFormat.json);
            Console.WriteLine($"DeleteFileAsyncResult: {DeleteFileAsyncResult}");
            #endregion DeleteFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportInstrumentsAsync()
            Console.WriteLine($"Calling DeleteFileAsync()");
            var ExportInstrumentsAsyncResult = await redcapApi.ExportInstrumentsAsync(_token, RedcapContent.Instrument, RedcapFormat.json);
            Console.WriteLine($"ExportInstrumentsAsyncResult: {ExportInstrumentsAsyncResult}");
            #endregion ExportInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), returns raw");
            var ExportPDFInstrumentsAsyncResult = await redcapApi.ExportPDFInstrumentsAsync(_token, RedcapContent.Pdf, recordId, eventName, "demographics", true);
            Console.WriteLine($"ExportInstrumentsAsyncResult: {JsonConvert.SerializeObject(ExportPDFInstrumentsAsyncResult)}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), saving pdf file to {filedDownloadPath}");
            var ExportPDFInstrumentsAsyncResult2 = await redcapApi.ExportPDFInstrumentsAsync(_token, recordId, eventName, "demographics", true, filedDownloadPath, RedcapReturnFormat.json);
            Console.WriteLine($"ExportPDFInstrumentsAsyncResult2: {ExportPDFInstrumentsAsyncResult2}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            //#region ExportInstrumentMappingAsync()
            //Console.WriteLine($"Calling ExportInstrumentMappingAsync()");
            //var armsToExportInstrumentMapping = arms;
            //var ExportInstrumentMappingAsyncResult = await redcap_api_1_1_0.ExportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, armsToExportInstrumentMapping, OnErrorFormat.json);
            //Console.WriteLine($"ExportInstrumentMappingAsyncResult: {ExportInstrumentMappingAsyncResult}");
            //#endregion ExportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportInstrumentMappingAsync()
            var importInstrumentMappingData = new List<FormEventMapping> { new FormEventMapping { arm_num = "1", unique_event_name = "clinical_arm_1", form = "demographics" } };
            Console.WriteLine($"Calling ImportInstrumentMappingAsync()");
            var ImportInstrumentMappingAsyncResult = await redcapApi.ImportInstrumentMappingAsync(_token, RedcapContent.FormEventMapping, RedcapFormat.json, importInstrumentMappingData, RedcapReturnFormat.json);
            Console.WriteLine($"ImportInstrumentMappingAsyncResult: {ImportInstrumentMappingAsyncResult}");
            #endregion ImportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportMetaDataAsync()
            Console.WriteLine($"Calling ExportMetaDataAsync()");
            var ExportMetaDataAsyncResult = await redcapApi.ExportMetaDataAsync(_token, RedcapContent.MetaData, RedcapFormat.json, null, null, RedcapReturnFormat.json);
            Console.WriteLine($"ExportMetaDataAsyncResult: {ExportMetaDataAsyncResult}");
            #endregion ExportMetaDataAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportMetaDataAsync()
            /*
             * This imports 1 field into the data dictionary
             */
            var importMetaData = new List<RedcapMetaData> { new RedcapMetaData { field_name = "first_name", form_name = "demographics", field_type = "text", field_label = "First Name" } };
            Console.WriteLine($"Not calling ImportMetaDataAsync(), still change data dictionary to include 1 field");
            //var ImportMetaDataAsyncResult = redcap_api_1_0_7.ImportMetaDataAsync(_token, "metadata", ReturnFormat.json, importMetaData, OnErrorFormat.json);
            //Console.WriteLine($"ImportMetaDataAsyncResult: {ImportMetaDataAsyncResult}");
            #endregion ImportMetaDataAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region CreateProjectAsync()
            var projectData = new List<RedcapProject> { new RedcapProject { project_title = "Amazing Project ", purpose = ProjectPurpose.Other, purpose_other = "Test" } };
            Console.WriteLine($"Calling CreateProjectAsync(), creating a new project with Amazing Project as title, purpose 1 (other) ");
            Console.WriteLine($"-----------------------Notice the use of SUPER TOKEN------------------------");
            var CreateProjectAsyncResult = await redcapApi.CreateProjectAsync(_superToken, RedcapContent.Project, RedcapFormat.json, projectData, RedcapReturnFormat.json, null);
            Console.WriteLine($"CreateProjectAsyncResult: {CreateProjectAsyncResult}");
            #endregion CreateProjectAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportProjectInfoAsync()
            var projectInfo = new RedcapProjectInfo { ProjectTitle = "Updated Amazing Project ", Purpose = ProjectPurpose.QualityImprovement, SurveysEnabled = 1 };
            Console.WriteLine($"Calling ImportProjectInfoAsync()");
            var ImportProjectInfoAsyncResult = await redcapApi.ImportProjectInfoAsync(_token, RedcapContent.ProjectSettings, RedcapFormat.json, projectInfo);
            Console.WriteLine($"ImportProjectInfoAsyncResult: {ImportProjectInfoAsyncResult}");
            #endregion ImportProjectInfoAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportProjectInfoAsync()
            Console.WriteLine($"Calling ExportProjectInfoAsync()");
            var ExportProjectInfoAsyncResult = await redcapApi.ExportProjectInfoAsync(_token, RedcapContent.ProjectSettings, RedcapFormat.json);
            Console.WriteLine($"ExportProjectInfoAsyncResult: {ExportProjectInfoAsyncResult}");
            #endregion ExportProjectInfoAsync()

            Console.WriteLine("----------------------------Demo completed! Press Enter to Exit-------------");
            Console.ReadLine();

        }
    }


}
