using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    /// <summary>
    /// Very simplified test class for Redcap Api
    /// This is not a comprehensive test, add more if you'd like.
    /// Make sure you have some records in the redcap project for the instance you are testing
    /// </summary>
    public class RedcapApiTests
    {
        private const string _token = "A8E6949EF4380F1111C66D5374E1AE6C";
        private const string _uri = "http://localhost/redcap/api/";
        public RedcapApiTests()
        {
            // initialize stuff here
        }
        /// <summary>
        /// Can Export Arms
        /// All arms should be returned
        /// Using API version 1.0.0+
        /// </summary>
        [Fact]
        public void CanExportArmsAsync_AllArms_ShouldContain_armnum()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            /*
             * Using API Version 1.0.0+
             */ 
            var redcapApi = new RedcapApi(apiEndpoint);
            var result = redcapApi.ExportArmsAsync(apiKey, "arm", ReturnFormat.json, null, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting multiple arms to be return since we asked for all arms by not providing any arms by passing null for the params
            Assert.Contains("1", data);
            Assert.Contains("2", data);
        }
        /// <summary>
        /// Can Import Arms
        /// Using API version 1.0.0+
        /// </summary>
        [Fact]
        public void CanImportArmsAsync_SingleArm_ShouldReturn_number()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;
            var armlist = new List<RedcapArm>
            {
                new RedcapArm{arm_num = "3", name = "testarm_this_will_be_deleted"}
            };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var redcapApi = new RedcapApi(apiEndpoint);
            var result = redcapApi.ImportArmsAsync(apiKey, "arm", Override.False, "import", ReturnFormat.json, armlist, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "1", the number of arms imported, since we pass 1 arm to be imported
            Assert.Contains("1", data);
        }
        /// <summary>
        /// Can Delete Arms
        /// Using API version 1.0.0+
        /// </summary>
        [Fact]
        public void CanDeleteArmsAsync_SingleArm_ShouldReturn_number()
        {
            // Arrange
            var apiKey = _token;
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
            var redcapApi = new RedcapApi(apiEndpoint);
            var result = redcapApi.DeleteArmsAsync(apiKey, "arm", "delete", armarray).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "1", the number of arms deleted, since we pass 1 arm to be deleted
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
            var apiKey = _token;
            var apiEndpoint = _uri;
            var ExportEventsAsyncData = new string[] { "1" };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var redcapApi = new RedcapApi(apiEndpoint);
            var result = redcapApi.ExportEventsAsync(apiKey, null, ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            Assert.Contains("event_name", data);
        }
        /// <summary>
        /// Can Import Events
        /// Using API version 1.0.0+
        /// </summary>
        [Fact]
        public void CanImportEventsAsync_MultipleEvents_ShouldReturn_number()
        {
            // Arrange
            var apiKey = _token;
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
            var redcapApi = new RedcapApi(apiEndpoint);
            var result = redcapApi.ImportEventsAsync(apiKey, null, null, Override.False, ReturnFormat.json, eventList, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "3", since we had 3 redcap events imported
            Assert.Contains("3", data);
        }
        /// <summary>
        /// Can delete Events
        /// Using API version 1.0.0+
        /// </summary>
        [Fact]
        public void CanDeleteEventsAsync_SingleEvent_ShouldReturn_number()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };

            // Act
            /*
             * Using API Version 1.0.0+
             */
            var redcapApi = new RedcapApi(apiEndpoint);
            var result = redcapApi.DeleteEventsAsync(apiKey, null, null, DeleteEventsAsyncData).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert 
            // Expecting "3", since we had 3 redcap events imported
            Assert.Contains("1", data);
        }

        /// <summary>
        /// Export / Get single record
        /// </summary>
        [Fact]
        public void CanGetRecordsAsync_SingleRecord_ShouldContain_string_1()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordAsync("1", ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, null, null, null, null).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportRecordsAsync("1,2", ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, null, null, null, null).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportRecordAsync("1", ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, null, null, null, null).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportEventsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordsAsync(ReturnFormat.json, OnErrorFormat.json, RedcapDataType.flat).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;
            char[] delimiters = new char[] { ';', ',' };

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordsAsync(ReturnFormat.json, OnErrorFormat.json, RedcapDataType.flat, delimiters).Result;
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
            // Assume current redcap version is 8.1.9
            var currentRedcapVersion = "8.1.9";
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRedcapVersionAsync(ReturnFormat.json, RedcapDataType.flat).Result;
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
            // Assume current redcap version is 8.1.9
            var currentRedcapVersion = "8.1.9";
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportRedcapVersionAsync(ReturnFormat.json, RedcapDataType.flat).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;
            var username = "tranpl";
            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportUsersAsync(ReturnFormat.json, OnErrorFormat.json).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            var record = new
            {
                record_id = "1",
                redcap_event_name = "event1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };
            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json).Result;
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
            var apiKey = _token;
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
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, dateFormat).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportRecordsAsync(ReturnFormat.json, RedcapDataType.flat).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);
        }
        /// <summary>
        /// Can export meta data
        /// </summary>
        [Fact]
        public void CanExportMetaDataAsync_Metadata_ShouldReturn_string_record_id()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportMetaDataAsync(ReturnFormat.json, OnErrorFormat.json).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;
            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportArmsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("arm_num", data);
        }
        /// <summary>
        /// Can import arms
        /// </summary>
        [Fact]
        public void CanImportEventsAsync_Events_ShouldReturn_Number()
        {
            // Arrange
            var apiKey = _token;
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
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ImportEventsAsync(listOfEvents, Override.False, ReturnFormat.json, OnErrorFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
        /// <summary>
        /// Test attempts to import a file into the redcap project
        /// There are a few assumptions, please make sure you have the files and folders
        /// exactly as shown, or name it to your needs.
        /// </summary>
        [Fact]
        public void CanImportFileAsync_File_ShouldReturn_Empty_string()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;
            var pathImport = "C:\\redcap_download_files";
            string importFileName = "test2.java";

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ImportFileAsync("1", "protocol_upload", "event_1_arm_1", "", importFileName, pathImport, OnErrorFormat.json).Result;

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
            var apiKey = _token;
            var apiEndpoint = _uri;
            var pathExport = "C:\\redcap_download_files";

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportFileAsync("1", "protocol_upload", "event_1_arm_1", "", pathExport, OnErrorFormat.json).Result;
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
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.DeleteFileAsync(_token, "protocol_upload", "event_1_arm_1", null, OnErrorFormat.json).Result;

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
            var apiKey = _token;
            var apiEndpoint = _uri;
            var records = new string[] { "1" };
            var events = new string[] { };
            var fields = new string[] { };
            var forms = new string[] { };

            // Act
            var redcap_api = new RedcapApi(apiEndpoint);
            
            var result = redcap_api.ExportRecordsAsync(apiKey, "record", ReturnFormat.json, RedcapDataType.flat, records, fields, forms, events, RawOrLabel.raw, RawOrLabelHeaders.raw, false, OnErrorFormat.json, false, false, null).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
    }
}
