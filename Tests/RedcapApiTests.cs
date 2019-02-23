using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using Redcap.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
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
    /// <summary>
    /// Very simplified test class for Redcap Api
    /// This is not a comprehensive test, add more if you'd like.
    /// Make sure you have some records in the redcap project for the instance you are testing
    /// </summary>
    public class RedcapApiTests
    {
        // API Token for a project in a local instance of redcap
        private const string _token = "A8E6949EF4380F1111C66D5374E1AE6C";
        // local instance of redcap api uri
        private const string _uri = "http://localhost/redcap/api/";
        private readonly RedcapApi _redcapApi;
        public RedcapApiTests()
        {
            _redcapApi = new RedcapApi(_uri);
        }

        [Fact]
        public void CanImportRecordsAsyncAsDictionary_ShouldReturn_CountString()
        {
            // Arrange
            var data = new List<Dictionary<string, string>> { };
            var keyValues = new Dictionary<string, string> { };
            keyValues.Add("first_name", "Jon");
            keyValues.Add("last_name", "Doe");
            keyValues.Add("record_id", "8");
            data.Add(keyValues);
            // Act
            var result = _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json).Result;

            var res = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting a string of 1 since we are importing one record
            Assert.Contains("1", res);
        }
        [Fact, TestPriority(0)]
        public void CanImportRecordsAsync_ShouldReturn_CountString()
        {
            // Arrange
            var data = new List<Demographic> { new Demographic { FirstName = "Jon", LastName = "Doe", RecordId = "1" } };
            // Act
            /*
             * Using API Version 1.0.0+
             */
            // executing method using default options
            var result = _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json).Result;

            var res = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting a string of 1 since we are importing one record
            Assert.Contains("1", res);
        }
        [Fact]
        public async void CanDeleteRecordsAsync_ShouldReturn_string()
        {
            // Arrange
            var record = new List<Dictionary<string, string>> { };
            var keyValues = new Dictionary<string, string> { };
            keyValues.Add("first_name", "Jon");
            keyValues.Add("last_name", "Doe");
            keyValues.Add("record_id", "8");
            record.Add(keyValues);
            // import records so we can delete it for this test
            await _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, true, record, "MDY", ReturnContent.count, OnErrorFormat.json);

            var records = new string[]
            {
                "8"
            };
            var arm = 1;
            // Act
            var result = await _redcapApi.DeleteRecordsAsync(_token, Content.Record, RedcapAction.Delete, records, arm);

            // Assert 
            // Expecting a string of 1 since we are deleting one record
            Assert.Contains("1", result);
        }
        [Fact]
        public async void CanImportRepeatingInstrumentsAndEvents_ShouldReturn_string()
        {
            // exact instrument names in data dictionary to repeat
            var repeatingInstruments = new List<RedcapRepeatInstrument> {
                new RedcapRepeatInstrument {
                    EventName = "event_1_arm_1",
                    FormName = "demographics",
                    CustomFormLabel = "TestTestTest"
                }
            };
            var result = await _redcapApi.ImportRepeatingInstrumentsAndEvents(_token, repeatingInstruments);
            // Expect "1" as we are importing a single repeating instrument
            Assert.Contains("1", result);
        }
        /// <summary>
        /// Test ability to export repeating instruments and events
        /// API Version 1.0.0+
        /// </summary>
        [Fact]
        public async void CanExportRepeatingInstrumentsAndEvents_ShouldReturn_string()
        {
            // Arrange
            // We'll importa a single repeating form so we can run test
            // By executing the import repeating instrument api, redcap will
            // enable the repeating instruments and events feature automatically
            var repeatingInstruments = new List<RedcapRepeatInstrument> {
                new RedcapRepeatInstrument {
                    EventName = "event_1_arm_1",
                    FormName = "demographics",
                    CustomFormLabel = "TestTestTest"
                }
            };
            await _redcapApi.ImportRepeatingInstrumentsAndEvents(_token, repeatingInstruments);

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var result = await _redcapApi.ExportRepeatingInstrumentsAndEvents(_token);

            // Assert 
            // Expecting event names, form name and custom form labels
            // we imported it above
            Assert.Contains("event_name", result);
            Assert.Contains("form_name", result);
            Assert.Contains("custom_form_label", result);
            Assert.Contains("demographics", result);
            Assert.Contains("event_1_arm_1", result);
            Assert.Contains("TestTestTest", result);
        }

        /// <summary>
        /// Can Export Arms
        /// All arms should be returned
        /// Using API version 1.0.0+
        /// </summary>
        [Fact, TestPriority(1)]
        public void CanExportArmsAsync_AllArms_ShouldContain_armnum()
        {
            // Arrange


            // Act
            /*
             * Using API Version 1.0.0+
             */
            var result = _redcapApi.ExportArmsAsync(_token).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting multiple arms to be return since we asked for all arms by not providing any arms by passing null for the params
            // ** Important to notice is that if we didn't add any events to an arm, even if there are more, only
            // arms with events will be returned **
            Assert.Contains("1", data);
            Assert.Contains("2", data);
        }


        /// <summary>
        /// Can Import Arms
        /// Using API version 1.0.0+
        /// </summary>
        [Fact, TestPriority(0)]
        public async void CanImportArmsAsync_SingleArm_ShouldReturn_number()
        {
            // Arrange
            var armlist = new List<RedcapArm>
            {
                new RedcapArm{ArmNumber = "3", Name = "testarm3_this_will_be_deleted"},
                new RedcapArm{ArmNumber = "2", Name = "testarm2_this_will_be_deleted"},
                new RedcapArm{ArmNumber = "4", Name = "testarm4_this_will_be_deleted"},
            };
            // Act
            /*
             * Using API Version 1.0.0+
             */
            var result = await _redcapApi.ImportArmsAsync(_token, Content.Arm, Override.False, RedcapAction.Import, ReturnFormat.json, armlist, OnErrorFormat.json);

            // Assert 
            // Expecting "3", the number of arms imported, since we pass 3 arm to be imported
            Assert.Contains("3", result);
        }
        /// <summary>
        /// Can Delete Arms
        /// Using API version 1.0.0+
        /// </summary>
        [Fact, TestPriority(99)]
        public async void CanDeleteArmsAsync_SingleArm_ShouldReturn_number()
        {
            // Arrange
            // Initially if we enable a project as longitudinal, redcap will append a single arm.
            // We are adding a single arm(3) to the project.
            var redcapArms = new List<RedcapArm> {
                new RedcapArm { ArmNumber = "3", Name = "Arm 3" },
            };

            // Make sure we have an arm with a few events before trying to delete one.
            var importArmsResults = await _redcapApi.ImportArmsAsync(_token, Content.Arm, Override.True, RedcapAction.Import, ReturnFormat.json, redcapArms, OnErrorFormat.json);

            // arms(#) to be deleted
            var armarray = new string[]
            {
               "3"
            };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var result = _redcapApi.DeleteArmsAsync(_token, Content.Arm, RedcapAction.Delete, armarray).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "1", the number of arms deleted, since we pass 1 arm to be deleted
            // You'll need an arm 3 to be available first, run import arm
            Assert.Contains("1", data);
        }
        /// <summary>
        /// Can Export Events
        /// Using API version 1.0.0+
        /// </summary>
        [Fact]
        public void CanExportEventsAsync_SingleEvent_ShouldReturn_event()
        {
            // Arrange

            var ExportEventsAsyncData = new string[] { "1" };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var result = _redcapApi.ExportEventsAsync(_token, Content.Event, ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            Assert.Contains("event_name", data);
        }
        /// <summary>
        /// Can Import Events
        /// Using API version 1.0.0+
        /// </summary>
        [Fact, TestPriority(0)]
        public void CanImportEventsAsync_MultipleEvents_ShouldReturn_number()
        {
            // Arrange

            var apiEndpoint = _uri;
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
                    UniqueEventName = "clinical_arm_2",
                    CustomEventLabel = "hello clinical 2"
                },
                new RedcapEvent {
                    EventName = "clinical",
                    ArmNumber = "3",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "clinical_arm_3",
                    CustomEventLabel = "hello clinical 3"
                }
            };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ImportEventsAsync(_token, Content.Event, RedcapAction.Import, Override.False, ReturnFormat.json, eventList, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "3", since we had 3 redcap events imported
            Assert.Contains("3", data);
        }
        /// <summary>
        /// Can delete Events
        /// Using API version 1.0.0+
        /// </summary>
        [Fact, TestPriority(10)]
        public async void CanDeleteEventsAsync_SingleEvent_ShouldReturn_number()
        {
            // Arrange
            var events = new List<RedcapEvent> {
                new RedcapEvent { ArmNumber = "1", EventName = "Clinical Event 1", UniqueEventName = "clinical_arm_1"}
            };
            var importResult = _redcapApi.ImportEventsAsync(_token, Content.Event, RedcapAction.Import, Override.True, ReturnFormat.json, events);
            // the event above, redcap appends _arm_1 when you add an arm_#
            var DeleteEventsAsyncData = new string[] { "clinical_event_1_arm_1" };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var result = await _redcapApi.DeleteEventsAsync(_token, Content.Event, RedcapAction.Delete, DeleteEventsAsyncData);

            // Assert 
            // Expecting "1", since we had 1 redcap events imported
            Assert.Contains("1", result);
        }
        [Fact]
        public void CanExportRecordAsync_SingleRecord_ShouldReturn_String_1()
        {
            // Arrange
            var recordId = "1";
            // Act
            /*
             * Just passing the required parameters
             */
            var result = _redcapApi.ExportRecordAsync(_token, Content.Record, recordId).Result;

            // Assert
            Assert.Contains("1", result);

        }
        /// <summary>
        /// Export / Get single record
        /// </summary>
        [Fact]
        public void CanExportRecordsAsync_SingleRecord_ShouldContain_string_1()
        {
            // Arrange
            var record = new string[]
            {
                "1"
            };
            var redcapEvent = new string[] { "event_1_arm_1" };
            // Act
            var _redcapApi = new RedcapApi(_uri);
            var result = _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, record, null, null, redcapEvent).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
        /// <summary>
        /// Can export multiple records
        /// </summary>
        [Fact]
        public void CanExportRecordsAsync_MultipleRecord_ShouldContain_string_1_2()
        {
            // Arrange

            var apiEndpoint = _uri;
            var records = new string[] { "1, 2" };

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, records).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
            Assert.Contains("2", data);
        }
        /// <summary>
        /// Can export single record
        /// </summary>
        [Fact]
        public void CanExportRecordAsync_SingleRecord_ShouldContain_string_1()
        {
            // Arrange

            var apiEndpoint = _uri;
            var records = new string[] { "1" };

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, records).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
        /// <summary>
        /// Can export events
        /// </summary>
        [Fact]
        public void CanExportAsync_AllEvents_ShouldContain_event_name()
        {
            // Arrange

            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportEventsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("event_name", data);

        }
        /// <summary>
        /// Can get all records
        /// </summary>
        [Fact]
        public void CanGetRecordsAsync1_AllRecords_ShouldContain_string_record_id()
        {
            // Arrange

            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportRecordsAsync(_token, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);

        }
        /// <summary>
        /// Can get all records
        /// </summary>
        [Fact]
        public void CanGetRecordsAsync2_AllRecords_ShouldContain_string_record_id()
        {
            // Arrange

            var apiEndpoint = _uri;
            char[] delimiters = new char[] { ';', ',' };

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportRecordsAsync(_token, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);

        }
        /// <summary>
        /// Can get redcap version
        /// </summary>
        [Fact]
        public void CanGetRedcapVersion_VersionNumber_Shouldontain_Number()
        {
            // Arrange
            // Assume current redcap version is 8.11.4
            var currentRedcapVersion = "8.11.4";

            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportRedcapVersionAsync(_token, Content.Version).Result;
            var data = result;

            // Assert
            Assert.Equal(currentRedcapVersion, data);

        }
        /// <summary>
        /// Can export redcap version
        /// </summary>
        [Fact]
        public void CanExportRedcapVersion_VersionNumber_Shouldontain_Number()
        {
            // Arrange
            // Assume current redcap version is 8.5.5
            var currentRedcapVersion = "8.5.5";

            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportRedcapVersionAsync(_token, Content.Version).Result;
            var data = result;

            // Assert
            Assert.Equal(currentRedcapVersion, data);

        }
        /// <summary>
        /// Can export users
        /// </summary>
        [Fact]
        public void CanExportUsers_AllUsers_ShouldReturn_username()
        {
            // Arrange

            var username = "tranpl";
            // Act
            var result = _redcapApi.ExportUsersAsync(_token, ReturnFormat.json).Result;
            var data = result;

            // Assert
            Assert.Contains(username, data);

        }
        /// <summary>
        /// Can save record
        /// </summary>
        [Fact]
        public void CanSaveRecord1_SingleRecord_ShouldReturn_Ids()
        {
            // Arrange

            var apiEndpoint = _uri;

            var record = new
            {
                record_id = "1",
                redcap_event_name = "event1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };
            var data = new List<object> { record };
            // Act
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.overwrite, false, data, "MDY").Result;
            var deSerializedData = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", deSerializedData);

        }
        /// <summary>
        /// Can save record
        /// </summary>
        [Fact]
        public void CanSaveRecord2_SingleRecord_ShouldReturn_Ids()
        {
            // Arrange

            var apiEndpoint = _uri;
            var dateFormat = "YMD";
            var record = new
            {
                record_id = "1",
                redcap_event_name = "event1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };

            // Act
            var _redcapApi = new RedcapApi(_token, apiEndpoint);
            var result = _redcapApi.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, dateFormat).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);

        }
        /// <summary>
        /// Can export records
        /// </summary>
        [Fact]
        public void CanExportRecordsAsync_AllRecords_ShouldReturn_string_record_id()
        {
            // Arrange

            // Act
            var _redcapApi = new RedcapApi(_uri);
            var result = _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat).Result;
            /*
             * We are using a default project with demographic form.
             * Just binding it to a simplified model so we can more easily work with it
             */
            var data = JsonConvert.DeserializeObject<List<Demographic>>(result);

            // Assert
            Assert.True(data.Count > 1);
        }
        /// <summary>
        /// Can export meta data
        /// </summary>
        [Fact]
        public async void CanExportMetaDataAsync_Metadata_ShouldReturn_string_record_id()
        {
            // Arrange

            // Act
            var result = await _redcapApi.ExportMetaDataAsync(_token, ReturnFormat.json);

            // Assert
            Assert.Contains("record_id", result);
        }
        /// <summary>
        /// Can export arms
        /// </summary>
        [Fact]
        public async void CanExportArmsAsync_Arms_ShouldReturn_arms_array()
        {
            // Arrange
            // Importing 3 arms so that we can run the test to export
            var redcapArms = new List<RedcapArm>
            {
                new RedcapArm{ArmNumber = "3", Name = "testarm3_this_will_be_deleted"},
                new RedcapArm{ArmNumber = "2", Name = "testarm2_this_will_be_deleted"},
                new RedcapArm{ArmNumber = "4", Name = "testarm4_this_will_be_deleted"},
            };
            // Import Arms so we can test
            await _redcapApi.ImportArmsAsync(_token, Content.Arm, Override.False, RedcapAction.Import, ReturnFormat.json, redcapArms, OnErrorFormat.json);
            var listOfEvents = new List<RedcapEvent>() {
                new RedcapEvent{
                    ArmNumber = "2",
                    CustomEventLabel = "HelloEvent1",
                    EventName = "Import Event 1",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "import_event_1_arm_2"
                },
                new RedcapEvent{
                    ArmNumber = "2",
                    CustomEventLabel = "HelloEvent2",
                    EventName = "Import Event 2",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "import_event_2_arm_2"
                },
                new RedcapEvent{
                    ArmNumber = "2",
                    CustomEventLabel = "HelloEvent3",
                    EventName = "Import Event 3",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "import_event_3_arm_2"
                }
            };
            // Import Events so we can test 
            await _redcapApi.ImportEventsAsync(_token, Override.False, ReturnFormat.json, listOfEvents, OnErrorFormat.json);
            // we want to export arm 1 and arm 2
            var exportArms = new string[] { "1", "2" };
            // Act
            var result = await _redcapApi.ExportArmsAsync(_token, Content.Arm, ReturnFormat.json, exportArms);
            
            // Assert
            // In order for the arms array to be returned, events for the specific arm
            // needs to be present. An arm without any events will not be returned.
            Assert.Contains("arm_num", result);
            Assert.Contains("1", result);
            Assert.Contains("2", result);
        }
        /// <summary>
        /// Can import arms
        /// </summary>
        [Fact, TestPriority(0)]
        public async void CanImportEventsAsync_Events_ShouldReturn_Number()
        {
            // Arrange
            var listOfEvents = new List<RedcapEvent>() {
                new RedcapEvent{
                    ArmNumber = "3",
                    CustomEventLabel = "HelloEvent1",
                    EventName = "Import Event 1",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "import_event_1_arm_3"
                },
                new RedcapEvent{
                    ArmNumber = "3",
                    CustomEventLabel = "HelloEvent2",
                    EventName = "Import Event 2",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "import_event_2_arm_3"
                },
                new RedcapEvent{
                    ArmNumber = "3",
                    CustomEventLabel = "HelloEvent3",
                    EventName = "Import Event 3",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "import_event_3_arm_3"
                }
            };

            // Act
            var result = await _redcapApi.ImportEventsAsync(_token, Override.False, ReturnFormat.json, listOfEvents, OnErrorFormat.json);

            // Assert
            // Expecting 3 since we imported 3 events
            Assert.Contains("3", result);
        }
        /// <summary>
        /// Test attempts to import a file into the redcap project
        /// There are a few assumptions, please make sure you have the files and folders
        /// exactly as shown, or name it to your needs.
        /// API Version 1.0.0+
        /// </summary>
        [Fact, TestPriority(0)]
        public async void CanImportFileAsync_File_ShouldReturn_Empty_string()
        {
            // Arrange

            var pathImport = "C:\\redcap_download_files";
            string importFileName = "test.txt";
            var record = "1";
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";
            var repeatingInstrument = "1";
            // Act
            var result = await _redcapApi.ImportFileAsync(_token, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, importFileName, pathImport, OnErrorFormat.json);

            // Assert
            // Expecting an empty string. This API returns an empty string on success.
            Assert.True(string.IsNullOrEmpty(result));
        }
        /// <summary>
        /// Test attempts exports the file previously imported
        /// </summary>
        [Fact]
        public async void CanExportFileAsync_File_ShouldReturn_string()
        {
            // Arrange
            var pathExport = "C:\\redcap_download_files";
            var record = "1";
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";
            var repeatingInstrument = "1";
            var expectedString = "test.txt";
            // Act
            var result = await _redcapApi.ExportFileAsync(_token, Content.File, RedcapAction.Export, record, fieldName, eventName, repeatingInstrument, OnErrorFormat.json, pathExport);

            // Assert
            Assert.Contains(expectedString, result);
        }
        /// <summary>
        /// Can delete file previously uploaded
        /// </summary>
        [Fact]
        public async void CanDeleteFileAsync_File_ShouldReturn_Empty_string()
        {
            // Arrange
            
            var pathImport = "C:\\redcap_download_files";
            string importFileName = "test.txt";
            var record = "1";
            var fieldName = "protocol_upload";
            // In order for us to import a file in a longitudinal format, we'll need to specify
            // which event it belongs. 
            var eventName = "event_1_arm_1";
            var repeatingInstrument = "1";
            // Import a file to a record so we can do the integrated test below.
            await _redcapApi.ImportFileAsync(_token, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, importFileName, pathImport, OnErrorFormat.json);

            // Act
            var result = await _redcapApi.DeleteFileAsync(_token, Content.File, RedcapAction.Delete, record, fieldName, eventName, repeatingInstrument, OnErrorFormat.json);

            // Assert
            Assert.Contains(string.Empty, result);
        }
        /// <summary>
        /// Can export records for projects that does not contain repeating forms/instruments
        /// API Version 1.0.0+
        /// </summary>
        [Fact]
        public async void CanExportRecordsAsync_NonRepeating_Should_Return_String()
        {
            // Arrange
            // Arrange
            var data = new List<Dictionary<string, string>> { };
            var keyValues = new Dictionary<string, string> { };
            keyValues.Add("first_name", "Jon");
            keyValues.Add("last_name", "Doe");
            keyValues.Add("record_id", "8");
            data.Add(keyValues);
            // import a record so we can test
            await _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json);

            var records = new string[] { "8" };

            // Act
            var result = await _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, records);

            // Assert
            Assert.Contains("1", result);
        }
        /// <summary>
        /// Can export records for project with repeating instruments/forms
        /// API Version 1.0.0+
        /// </summary>
        [Fact]
        public async void CanExportRecordsAsync_Repeating_Should_Return_Record()
        {
            // Arrange
            var repeatingInstruments = new List<RedcapRepeatInstrument> {
                new RedcapRepeatInstrument {
                    EventName = "event_1_arm_1",
                    FormName = "demographics",
                    CustomFormLabel = "TestTestTest"
                }
            };
            // We import a repeating instrument to 'turn on' repeating instruments feature
            // as well as setting an initial repeating instrument 
            await _redcapApi.ImportRepeatingInstrumentsAndEvents(_token, repeatingInstruments);
            // Get the redcap event from above
            var redcapEvent = repeatingInstruments.FirstOrDefault();
            var data = new List<Dictionary<string, string>> { };
            // passing in minimum requirements for a project with repeating instruments/forms
            var keyValues = new Dictionary<string, string> { };
            keyValues.Add("first_name", "Jon");
            keyValues.Add("last_name", "Doe");
            keyValues.Add("record_id", "8");
            keyValues.Add("redcap_repeat_instance", "1");
            keyValues.Add("redcap_repeat_instrument", redcapEvent.FormName);
            data.Add(keyValues);
            // import a record so we can test
            await _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json);
            var records = new string[] { "8" };

            // Act
            var result = await _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, records);

            // Assert
            Assert.Contains("1", result);
        }
    }
}
