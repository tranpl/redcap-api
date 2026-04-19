using Newtonsoft.Json;

using Redcap;
using Redcap.Models;

using RedcapApiDemo.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedcapApiDemo
{
    partial class Program
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
            var methodFromArgs = GetMethodArgument(args);
            await InitializeDemo(methodFromArgs);
        }
        static async Task InitializeDemo(string methodToRun = null)
        {
            var devSettings = LoadDevelopmentSettings();
            var defaultBaseUri = string.IsNullOrWhiteSpace(devSettings.BaseUri) ? "http://localhost" : devSettings.BaseUri.Trim();
            var defaultProjectToken = string.IsNullOrWhiteSpace(devSettings.ProjectToken) ? "4B6C281C04A23A3C10A5AF336A44A05D" : devSettings.ProjectToken.Trim();
            /*
             * Change this token for your demo project
             * Using one created from a local dev instance
             */
            string _token = defaultProjectToken;
            /*
             * Change this token for your demo project
             * Using one created from a local dev instance
            */
            string _superToken = string.IsNullOrWhiteSpace(devSettings.SuperToken)
                ? "7A6DD591AA50723B86FDDD7000EB2C59193341F131644CA49A6FD0B7C26135E7"
                : devSettings.SuperToken.Trim();
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
            Console.WriteLine("Please make sure you include a working redcap api token.");
            Console.WriteLine($"Enter your redcap instance uri (default: {defaultBaseUri}):");
            var uriInput = Console.ReadLine();
            var baseUri = string.IsNullOrWhiteSpace(uriInput) ? defaultBaseUri : uriInput.Trim().TrimEnd('/');
            _uri = baseUri.EndsWith("/api", StringComparison.OrdinalIgnoreCase)
                ? baseUri + "/"
                : baseUri + "/api/";

            Console.WriteLine("Enter your project api token (press Enter for default/config value):");
            var token = Console.ReadLine();

            if(!string.IsNullOrWhiteSpace(token))
            {
                _token = token.Trim();
            }

            Console.WriteLine("Enter your super api token (press Enter for default/config value):");
            var superToken = Console.ReadLine();
            if(!string.IsNullOrWhiteSpace(superToken))
            {
                _superToken = superToken.Trim();
            }

            var runMode = methodToRun;
            if(string.IsNullOrWhiteSpace(runMode))
            {
                Console.WriteLine("Run mode: press Enter for full demo, or type 'import' to run only ImportRecordsAsync");
                runMode = Console.ReadLine();
            }

            Console.WriteLine($"Using Endpoint=> {_uri} Token => {_token}");

            if(!string.IsNullOrWhiteSpace(runMode))
            {
                var importOnlyApi = new RedcapApi(_uri);
                await RunSingleMethodAsync(importOnlyApi, _token, runMode);
                return;
            }

            var fileName = "Demographics_TestProject_DataDictionary.csv";
            var filedDownloadPath = @"C:\redcap_download_files";
            PrintDemoRequirements(_uri, eventName, fieldName, fileName, filedDownloadPath);
            var localArtifactsOk = EnsureDemoLocalArtifacts(fileName, filedDownloadPath, out var filePath);
            if(!localArtifactsOk)
            {
                Console.WriteLine("One or more local demo prerequisites are missing. Continue anyway? (yes/no)");
                var continueWithoutArtifacts = Console.ReadLine();
                if(string.IsNullOrEmpty(continueWithoutArtifacts) || !continueWithoutArtifacts.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Demo aborted. Fix prerequisites and rerun.");
                    return;
                }
            }

            Console.WriteLine("-----------------------------Starting API Version 1.0.5+-------------");
            Console.WriteLine("Starting demo for API Version 1.0.0+");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine("Creating a new instance of RedcapApi");
            var redcap_api = new RedcapApi(_uri);
            Console.WriteLine($"Using {_uri.ToString()} for redcap api endpoint.");

            #region ExportLoggingAsync()
            Console.WriteLine("Calling ExportLoggingAsync() . . .");
            Console.WriteLine($"Exporting logs for User . . .");
            var ExportLoggingAsync = await redcap_api.ExportLoggingAsync(_token, Content.Log, RedcapFormat.json, LogType.User);
            Console.WriteLine($"ExportLoggingAsync Results: {JsonConvert.DeserializeObject(ExportLoggingAsync)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion

            #region ImportDagsAsync()
            Console.WriteLine("Calling ImportDagsAsync() . . .");
            Console.WriteLine($"Importing Dags . . .");
            var dags = CreateDags(5);
            var ImportDagsAsyncResult = await redcap_api.ImportDagsAsync(_token, Content.Dag, RedcapAction.Import, RedcapFormat.json, dags);
            Console.WriteLine($"ImportDagsAsync Results: {JsonConvert.DeserializeObject(ImportDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion

            #region ExportDagsAsync()
            Console.WriteLine("Calling ExportDagsAsync() . . .");
            Console.WriteLine($"Exporting Dags . . .");
            var ExportDagsAsyncResult = await redcap_api.ExportDagsAsync(_token, Content.Dag, RedcapFormat.json);
            Console.WriteLine($"ExportDagsAsync Results: {JsonConvert.DeserializeObject(ExportDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion

            #region DeleteDagsAsync()
            Console.WriteLine("Calling DeleteDagsAsync() . . .");
            Console.WriteLine($"Deleting Dags . . .");
            var dagsToDelete = JsonConvert.DeserializeObject<List<RedcapDag>>(ExportDagsAsyncResult).Select(x => x.UniqueGroupName).ToArray();
            var DeleteDagsAsyncResult = await redcap_api.DeleteDagsAsync(_token, Content.Dag, RedcapAction.Delete, dagsToDelete);
            Console.WriteLine($"DeleteDagsAsync Results: {JsonConvert.DeserializeObject(DeleteDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion

            #region ImportRecordsAsync()
            Console.WriteLine("Calling ImportRecordsAsync() . . .");
            // get demographics data
            var importDemographicsData = CreateDemographics(includeBio: false, 5);
            Console.WriteLine("Serializing the data . . .");
            Console.WriteLine($"Importing record {string.Join(",", importDemographicsData.Select(x => x.RecordId).ToList())} . . .");
            var ImportRecordsAsync = await redcap_api.ImportRecordsAsync(_token, Content.Record, RedcapFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, forceAutoNumber: true, backgroundProcess: false, importDemographicsData, "MDY", CsvDelimiter.tab, ReturnContent.ids, RedcapReturnFormat.json);
            var ImportRecordsAsyncData = FormatApiResponse(ImportRecordsAsync);
            Console.WriteLine($"ImportRecordsAsync Result: {ImportRecordsAsyncData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion ImportRecordsAsync()

            #region ExportRecordsAsync()
            Console.WriteLine($"Calling ExportRecordsAsync()");
            Console.WriteLine($"Using records from the imported method..");
            var recordsToExport = importDemographicsData.Select(x => x.RecordId).ToArray();
            var instrumentName = new string[] { "demographics" };
            var ExportRecordsAsyncResult = await redcap_api.ExportRecordsAsync(_token, Content.Record, RedcapFormat.json, RedcapDataType.flat, recordsToExport, null, instrumentName);
            Console.WriteLine($"ExportRecordsAsyncResult: {ExportRecordsAsyncResult}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion ExportRecordsAsync()

            #region DeleteRecordsAsync()
            Console.WriteLine("Calling DeleteRecordsAsync() . . .");
            var records = importDemographicsData.Select(x => x.RecordId).ToArray();
            Console.WriteLine($"Deleting record {string.Join(",", recordsToExport)} . . .");
            var DeleteRecordsAsync = await redcap_api.DeleteRecordsAsync(_token, Content.Record, RedcapAction.Delete, recordsToExport, 1);
            var DeleteRecordsAsyncData = FormatApiResponse(DeleteRecordsAsync);
            Console.WriteLine($"DeleteRecordsAsync Result: {DeleteRecordsAsyncData}");

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion DeleteRecordsAsync()

            #region RenameRecordAsync()
            Console.WriteLine("Calling RenameRecordAsync() . . .");
            var recordToRename = importDemographicsData.Select(x => x.RecordId).FirstOrDefault();
            if(string.IsNullOrEmpty(recordToRename))
            {
                Console.WriteLine("RenameRecordAsync skipped: no records available to rename.");
            }
            else
            {
                Console.WriteLine($"Renaming record {recordToRename} . . .");
                var newRecordName = "2";
                var RenameRecordAsyncResult = await redcap_api.RenameRecordAsync(_token, Content.Record, RedcapAction.Rename, recordToRename, newRecordName, 1);
                var RenameRecordAsyncData = FormatApiResponse(RenameRecordAsyncResult);
                Console.WriteLine($"RenameRecordAsync Result: {RenameRecordAsyncData}");
            }

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion RenameRecordAsync()

            #region RandomizeRecord
            Console.WriteLine("Calling RandomizeRecord() Make sure project is randomization ready with tables etc. . . .");
            var record = "3";
            var randomizationId = "2";
            var randomizationResult = await redcap_api.RandomizeRecord(_token, Content.Record, RedcapAction.Randomize, record, randomizationId, RedcapFormat.json, RedcapReturnFormat.json);
            var randomizationData = JsonConvert.DeserializeObject(randomizationResult);
            Console.WriteLine($"RandomizeRecord Result: {randomizationData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion RandomizeRecord



            #region Users & User Priveleges
            Console.WriteLine("Calling ImportUsersAsync() . . .");
            var redcapUser1 = CreateRedcapUser("test1");
            var redcapUser2 = CreateRedcapUser("test2");
            var redcapUsers = new List<RedcapUser>();
            redcapUsers.Add(redcapUser1);
            redcapUsers.Add(redcapUser2);
            Console.WriteLine($"Importing  {redcapUsers.Count} user. . .");
            var ImportUsersAsyncResult = await redcap_api.ImportUsersAsync(_token, redcapUsers, RedcapFormat.json, RedcapReturnFormat.json);
            var ImportUsersAsyncData = JsonConvert.DeserializeObject(ImportUsersAsyncResult);
            Console.WriteLine($"ImportUsersAsync Result: {ImportUsersAsyncData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion  Users & User Priveleges


            #region ExportArmsAsync()
            var arms = new string[] { };
            Console.WriteLine("Calling ExportArmsAsync()");
            var ExportArmsAsyncResult = await redcap_api.ExportArmsAsync(_token, Content.Arm, RedcapFormat.json, arms, RedcapReturnFormat.json);
            Console.WriteLine($"ExportArmsAsyncResult: {JsonConvert.DeserializeObject(ExportArmsAsyncResult)}");
            #endregion ExportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportArmsAsync()
            var ImportArmsAsyncData = CreateArms(count: 3);
            Console.WriteLine("Calling ImportArmsAsync()");
            var ImportArmsAsyncResult = await redcap_api.ImportArmsAsync(_token, Content.Arm, Override.False, RedcapAction.Import, RedcapFormat.json, ImportArmsAsyncData, RedcapReturnFormat.json);
            Console.WriteLine($"ImportArmsAsyncResult: {JsonConvert.DeserializeObject(ImportArmsAsyncResult)}");
            #endregion ImportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteArmsAsync()
            var DeleteArmsAsyncData = ImportArmsAsyncData.Select(x => x.ArmNumber).ToArray();
            Console.WriteLine("Calling DeleteArmsAsync()");
            var DeleteArmsAsyncResult = await redcap_api.DeleteArmsAsync(_token, Content.Arm, RedcapAction.Delete, DeleteArmsAsyncData);
            Console.WriteLine($"DeleteArmsAsyncResult: {JsonConvert.DeserializeObject(DeleteArmsAsyncResult)}");
            #endregion DeleteArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportEventsAsync()
            var ExportEventsAsyncData = new string[] { "1" };
            Console.WriteLine("Calling ExportEventsAsync()");
            var ExportEventsAsyncResult = await redcap_api.ExportEventsAsync(_token, Content.Event, RedcapFormat.json, ExportEventsAsyncData, RedcapReturnFormat.json);
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
            var ImportEventsAsyncResult = await redcap_api.ImportEventsAsync(_token, Content.Event, RedcapAction.Import, Override.False, RedcapFormat.json, eventList, RedcapReturnFormat.json);
            Console.WriteLine($"ImportEventsAsyncResult: {ImportEventsAsyncResult}");
            #endregion ImportEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region DeleteEventsAsync()
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };
            Console.WriteLine("Calling DeleteEventsAsync()");
            var DeleteEventsAsyncResult = await redcap_api.DeleteEventsAsync(_token, Content.Event, RedcapAction.Delete, DeleteEventsAsyncData);
            Console.WriteLine($"DeleteEventsAsyncResult: {DeleteEventsAsyncResult}");
            #endregion DeleteEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region SwitchDagAsync()
            var SwitchDagAsyncData = new RedcapDag { GroupName = "testGroup", UniqueGroupName = "unique_name" };
            Console.WriteLine("Calling SwitchDagAsync()");
            var SwitchDagAsyncResult = await redcap_api.SwitchDagAsync(_token, SwitchDagAsyncData, Content.Dag, RedcapAction.Switch);
            Console.WriteLine($"SwitchDagAsyncResult: {SwitchDagAsyncResult}");
            #endregion SwitchDagAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();



            #region ExportFieldNamesAsync()
            Console.WriteLine("Calling ExportFieldNamesAsync(), first_name");
            var ExportFieldNamesAsyncResult = await redcap_api.ExportFieldNamesAsync(_token, Content.ExportFieldNames, RedcapFormat.json, "first_name", RedcapReturnFormat.json);
            Console.WriteLine($"ExportFieldNamesAsyncResult: {ExportFieldNamesAsyncResult}");
            #endregion ExportFieldNamesAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region ImportFileAsync()
            var recordId = "1";
            Console.WriteLine($"Calling ImportFileAsync(), {fileName}");
            var ImportFileAsyncResult = await redcap_api.ImportFileAsync(_token, Content.File, RedcapAction.Import, recordId, fieldName, eventName, null, fileName, filePath, RedcapReturnFormat.json);
            Console.WriteLine($"ImportFileAsyncResult: {ImportFileAsyncResult}");
            #endregion ImportFileAsync()


            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, not save the file.");
            var ExportFileAsyncResult = await redcap_api.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, RedcapReturnFormat.json);
            Console.WriteLine($"ExportFileAsyncResult: {ExportFileAsyncResult}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, saving the file.");
            var ExportFileAsyncResult2 = await redcap_api.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, RedcapReturnFormat.json, filedDownloadPath);
            Console.WriteLine($"ExportFileAsyncResult2: {ExportFileAsyncResult2}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteFileAsync()
            Console.WriteLine($"Calling DeleteFileAsync(), deleting file: {fileName} for field: {fieldName}");
            var DeleteFileAsyncResult = await redcap_api.DeleteFileAsync(_token, Content.File, RedcapAction.Delete, recordId, fieldName, eventName, "1", RedcapReturnFormat.json);
            Console.WriteLine($"DeleteFileAsyncResult: {DeleteFileAsyncResult}");
            #endregion DeleteFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportInstrumentsAsync()
            Console.WriteLine($"Calling DeleteFileAsync()");
            var ExportInstrumentsAsyncResult = await redcap_api.ExportInstrumentsAsync(_token, Content.Instrument, RedcapFormat.json);
            Console.WriteLine($"ExportInstrumentsAsyncResult: {ExportInstrumentsAsyncResult}");
            #endregion ExportInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), returns raw");
            var ExportPDFInstrumentsAsyncResult = await redcap_api.ExportPDFInstrumentsAsync(_token, Content.Pdf, recordId, eventName, "demographics", true);
            Console.WriteLine($"ExportInstrumentsAsyncResult: {JsonConvert.SerializeObject(ExportPDFInstrumentsAsyncResult)}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), saving pdf file to {filedDownloadPath}");
            var ExportPDFInstrumentsAsyncResult2 = await redcap_api.ExportPDFInstrumentsAsync(_token, recordId, eventName, "demographics", true, filedDownloadPath, RedcapReturnFormat.json);
            Console.WriteLine($"ExportPDFInstrumentsAsyncResult2: {ExportPDFInstrumentsAsyncResult2}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine("Would you like to run additional method wiring examples? (yes/no)");
            var additionalWiringResponse = Console.ReadLine();
            if (!string.IsNullOrEmpty(additionalWiringResponse) && additionalWiringResponse.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                await RunAdditionalMethodWiringAsync(redcap_api, _token, _superToken, recordId, eventName, filedDownloadPath);
            }

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
            var ImportInstrumentMappingAsyncResult = await redcap_api.ImportInstrumentMappingAsync(_token, Content.FormEventMapping, RedcapFormat.json, importInstrumentMappingData, RedcapReturnFormat.json);
            Console.WriteLine($"ImportInstrumentMappingAsyncResult: {ImportInstrumentMappingAsyncResult}");
            #endregion ImportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportMetaDataAsync()
            Console.WriteLine($"Calling ExportMetaDataAsync()");
            var ExportMetaDataAsyncResult = await redcap_api.ExportMetaDataAsync(_token, Content.MetaData, RedcapFormat.json, null, null, RedcapReturnFormat.json);
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
            var CreateProjectAsyncResult = await redcap_api.CreateProjectAsync(_superToken, Content.Project, RedcapFormat.json, projectData, RedcapReturnFormat.json, null);
            Console.WriteLine($"CreateProjectAsyncResult: {CreateProjectAsyncResult}");
            #endregion CreateProjectAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportProjectInfoAsync()
            var projectInfo = new RedcapProjectInfo { ProjectTitle = "Updated Amazing Project ", Purpose = ProjectPurpose.QualityImprovement, SurveysEnabled = 1 };
            Console.WriteLine($"Calling ImportProjectInfoAsync()");
            var ImportProjectInfoAsyncResult = await redcap_api.ImportProjectInfoAsync(_token, Content.ProjectSettings, RedcapFormat.json, projectInfo);
            Console.WriteLine($"ImportProjectInfoAsyncResult: {ImportProjectInfoAsyncResult}");
            #endregion ImportProjectInfoAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportProjectInfoAsync()
            Console.WriteLine($"Calling ExportProjectInfoAsync()");
            var ExportProjectInfoAsyncResult = await redcap_api.ExportProjectInfoAsync(_token, Content.ProjectSettings, RedcapFormat.json);
            Console.WriteLine($"ExportProjectInfoAsyncResult: {ExportProjectInfoAsyncResult}");
            #endregion ExportProjectInfoAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine("Run additional method wiring examples for remaining APIs? (y/N)");
            var runExtended = Console.ReadLine();
            if(string.Equals(runExtended, "y", StringComparison.OrdinalIgnoreCase))
            {
                await RunAdditionalMethodWiringAsync(redcap_api, _token, _superToken, recordId, eventName, filedDownloadPath);
            }

            Console.WriteLine("----------------------------Demo completed! Press Enter to Exit-------------");
            Console.ReadLine();

        }

    }


}
