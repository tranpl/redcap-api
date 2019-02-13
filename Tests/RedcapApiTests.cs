using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using Redcap.Utilities;
using System;
using System.Collections.Generic;
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
        private const string _token = "A8E6949EF4380F1111C66D5374E1AE6C";
        private const string _uri = "http://localhost/redcap/api/";
        private readonly RedcapApi _redcapApi;
        public RedcapApiTests()
        {
            _redcapApi = new RedcapApi(_uri);
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
            var _redcapApi = new RedcapApi(_uri);
            // executing method using default options
            var result = _redcapApi.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json).Result;

            var res = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting a string of 1 since we are importing one record
            Assert.Contains("1", res);
        }
        [Fact]
        public void CanDeleteRecordsAsync_ShouldReturn_string()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;
            var records = new string[]
            {
                "1"
            };
            var arm = 1;
            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            // executing method using default options
            var result = _redcapApi.DeleteRecordsAsync(apiToken, Content.Record, RedcapAction.Delete, records, arm).Result;

            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting a string of 1 since we are deleting one record
            Assert.Contains("1", data);
        }

        [Fact]
        public void CanExportRepeatingInstrumentsAndEvents_ShouldReturn_string()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            // executing method using default options
            var result = _redcapApi.ExportRepeatingInstrumentsAndEvents(apiToken).Result;

            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting multiple arms to be return since we asked for all arms by not providing any arms by passing null for the params
            Assert.Contains("event_name", data);
            Assert.Contains("form_name", data);
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
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportArmsAsync(apiToken).Result;
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
        public void CanImportArmsAsync_SingleArm_ShouldReturn_number()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;
            var armlist = new List<RedcapArm>
            {
                new RedcapArm{arm_num = "3", name = "testarm3_this_will_be_deleted"},
                new RedcapArm{arm_num = "2", name = "testarm2_this_will_be_deleted"},
                new RedcapArm{arm_num = "4", name = "testarm4_this_will_be_deleted"},

            };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ImportArmsAsync(apiToken, Content.Arm, Override.False, RedcapAction.Import, ReturnFormat.json, armlist, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "3", the number of arms imported, since we pass 3 arm to be imported
            Assert.Contains("3", data);
        }
        /// <summary>
        /// Can Delete Arms
        /// Using API version 1.0.0+
        /// </summary>
        [Fact, TestPriority(99)]
        public void CanDeleteArmsAsync_SingleArm_ShouldReturn_number()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;
            // arm 3 to be deleted
            var armarray = new string[]
            {
               "3"
            };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.DeleteArmsAsync(apiToken, Content.Arm, RedcapAction.Delete, armarray).Result;
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
            var apiToken = _token;
            var apiEndpoint = _uri;
            var ExportEventsAsyncData = new string[] { "1" };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ExportEventsAsync(apiToken, Content.Event, ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json).Result;
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
            var apiToken = _token;
            var apiEndpoint = _uri;
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
                    unique_event_name = "clinical_arm_2",
                    custom_event_label = "hello clinical 2"
                },
                new RedcapEvent {
                    event_name = "clinical",
                    arm_num = "3",
                    day_offset = "1",
                    offset_min = "0",
                    offset_max = "0",
                    unique_event_name = "clinical_arm_3",
                    custom_event_label = "hello clinical 3"
                }
            };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.ImportEventsAsync(apiToken, Content.Event, RedcapAction.Import, Override.False, ReturnFormat.json, eventList, OnErrorFormat.json).Result;
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
        public void CanDeleteEventsAsync_SingleEvent_ShouldReturn_number()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var _redcapApi = new RedcapApi(apiEndpoint);
            var result = _redcapApi.DeleteEventsAsync(apiToken, Content.Event, RedcapAction.Delete, DeleteEventsAsyncData).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "3", since we had 3 redcap events imported
            Assert.Contains("3", data);
        }

        [Fact]
        public void CanExportRecordAsync_SingleRecord_ShouldReturn_String_1()
        {
            // Arrange

            // Act
            var _redcapApi = new RedcapApi(_uri);
            /*
             * Just passing the required parameters
             */
            var result = _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);

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
            var apiToken = _token;
            var apiEndpoint = _uri;
            var records = new string[] { "1, 2" };

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
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
            var apiToken = _token;
            var apiEndpoint = _uri;
            var records = new string[] { "1" };

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
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
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
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
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.GetRecordsAsync(ReturnFormat.json, OnErrorFormat.json, RedcapDataType.flat).Result;
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
            var apiToken = _token;
            var apiEndpoint = _uri;
            char[] delimiters = new char[] { ';', ',' };

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.GetRecordsAsync(ReturnFormat.json, OnErrorFormat.json, RedcapDataType.flat, delimiters).Result;
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
            // Assume current redcap version is 8.5.5
            var currentRedcapVersion = "8.5.5";
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.GetRedcapVersionAsync(ReturnFormat.json, RedcapDataType.flat).Result;
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
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.ExportRedcapVersionAsync(ReturnFormat.json, RedcapDataType.flat).Result;
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
            var apiToken = _token;
            var apiEndpoint = _uri;
            var username = "tranpl";
            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.ExportUsersAsync(ReturnFormat.json, OnErrorFormat.json).Result;
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
            var apiToken = _token;
            var apiEndpoint = _uri;

            var record = new
            {
                record_id = "1",
                redcap_event_name = "event1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };
            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);

        }
        /// <summary>
        /// Can save record
        /// </summary>
        [Fact]
        public void CanSaveRecord2_SingleRecord_ShouldReturn_Ids()
        {
            // Arrange
            var apiToken = _token;
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
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
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
        public void CanExportMetaDataAsync_Metadata_ShouldReturn_string_record_id()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.ExportMetaDataAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);
        }
        /// <summary>
        /// Can export arms
        /// </summary>
        [Fact]
        public void CanExportArmsAsync_Arms_ShouldReturn_arms_array()
        {
            // Arrange
            // Make sure the project is a longitudinal project, no arms in classic
            var apiToken = _token;
            var apiEndpoint = _uri;
            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.ExportArmsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("arm_num", data);
        }
        /// <summary>
        /// Can import arms
        /// </summary>
        [Fact, TestPriority(0)]
        public void CanImportEventsAsync_Events_ShouldReturn_Number()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;
            var listOfEvents = new List<RedcapEvent>() {
                new RedcapEvent{
                    arm_num = "1",
                    custom_event_label = null,
                    event_name = "Event 1",
                    day_offset = "1",
                    offset_min = "0",
                    offset_max = "0",
                    unique_event_name = "event_1_arm_1"
                }
            };

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.ImportEventsAsync(listOfEvents, Override.False, ReturnFormat.json, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
        /// <summary>
        /// Test attempts to import a file into the redcap project
        /// There are a few assumptions, please make sure you have the files and folders
        /// exactly as shown, or name it to your needs.
        /// </summary>
        [Fact, TestPriority(0)]
        public void CanImportFileAsync_File_ShouldReturn_Empty_string()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;
            var pathImport = "C:\\redcap_download_files";
            string importFileName = "test.txt";
            var record = "1";
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";
            var repeatingInstrument = "1";
            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.ImportFileAsync(_token, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, importFileName, pathImport, OnErrorFormat.json).Result;

            // Assert
            Assert.Contains(string.Empty, result);
        }
        /// <summary>
        /// Test attempts exports the file previously imported
        /// </summary>
        [Fact]
        public void CanExportFileAsync_File_ShouldReturn_string()
        {
            // Arrange
            var pathExport = "C:\\redcap_download_files";
            var record = "1";
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";
            var repeatingInstrument = "1";
            // Act
            var result = _redcapApi.ExportFileAsync(_token, Content.File, RedcapAction.Export, record, fieldName, eventName, repeatingInstrument, OnErrorFormat.json, pathExport).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            var expectedString = "test.txt";
            Assert.Contains(expectedString, data);
        }
        /// <summary>
        /// Can delete file previously uploaded
        /// </summary>
        [Fact]
        public void CanDeleteFileAsync_File_ShouldReturn_Empty_string()
        {
            // Arrange
            var apiToken = _token;
            var apiEndpoint = _uri;

            // Act
            var _redcapApi = new RedcapApi(apiToken, apiEndpoint);
            var result = _redcapApi.DeleteFileAsync(_token, "protocol_upload", "event_1_arm_1", null, OnErrorFormat.json).Result;

            // Assert
            Assert.Contains(string.Empty, result);
        }
        /// <summary>
        /// Can export records
        /// </summary>
        [Fact]
        public void CanExportRecordsAsync_Should_Return_String()
        {
            // Arrange
            var records = new string[] { "1" };
            var events = new string[] { };
            var fields = new string[] { };
            var forms = new string[] { };

            // Act
            var _redcapApi = new RedcapApi(_uri);

            var result = _redcapApi.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, records).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }

    }
}
