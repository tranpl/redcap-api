using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    /// <summary>
    /// Test class for Redcap Api
    /// </summary>
    public class RedcapApiTests
    {
        private const string _token = "4AAE216218B33700456A30898F2D6417";
        private const string _uri = "http://localhost/redcap/api/";
        public RedcapApiTests()
        {
            // initialize stuff here
        }

        [Fact]
        public void CanExportAsync_SingleRecord_ShouldContain_string_1()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordAsync("1", InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
        [Fact]
        public void CanExportAsync_AllEvents_ShouldContain_event_name()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportEventsAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("event_name", data);

        }
        [Fact]
        public void CanGetRecordsAsync1_AllRecords_ShouldContain_string_record_id()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordsAsync(InputFormat.json, ReturnFormat.json, RedcapDataType.flat).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);

        }

        [Fact]
        public void CanGetRecordsAsync2_AllRecords_ShouldContain_string_record_id()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;
            char[] delimiters = new char[] { ';', ',' };

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordsAsync(InputFormat.json, ReturnFormat.json, RedcapDataType.flat, delimiters).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);

        }

        [Fact]
        public void CanGetRedcapVersion_VersionNumber_Shouldontain_Number()
        {
            // Arrange
            // Assume current redcap version is 8.1.2
            var currentRedcapVersion = "8.1.2";
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            var data = result;

            // Assert
            Assert.Equal(currentRedcapVersion, data);

        }

        [Fact]
        public void CanExportRedcapVersion_VersionNumber_Shouldontain_Number()
        {
            // Arrange
            // Assume current redcap version is 8.1.2
            var currentRedcapVersion = "8.1.2";
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            var data = result;

            // Assert
            Assert.Equal(currentRedcapVersion, data);

        }

        [Fact]
        public void CanExportUsers_AllUsers_ShouldReturn_username()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;
            var username = "tranpl";
            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportUsersAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = result;

            // Assert
            Assert.Contains(username, data);

        }

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
            var result = redcap_api.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);

        }

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
            var result = redcap_api.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json, dateFormat).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);

        }

        [Fact]
        public void CanExportRecordsAsync_AllRecords_ShouldReturn_string_record_id()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);
        }
        [Fact]
        public void CanExportMetaDataAsync_Metadata_ShouldReturn_string_record_id()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportMetaDataAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("record_id", data);
        }
        [Fact]
        public void CanExportArmsAsync_Arms_ShouldReturn_arms_array()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;
            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.ExportArmsAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("arm_num", data);
        }

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
            var result = redcap_api.ImportEventsAsync(listOfEvents, Override.False, InputFormat.json, ReturnFormat.json).Result;
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
            var result = redcap_api.ImportFileAsync("1", "protocol_upload", "event_1_arm_1", "", importFileName, pathImport, ReturnFormat.json).Result;

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
            var result = redcap_api.ExportFileAsync("1", "protocol_upload", "event_1_arm_1", "", pathExport, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            var expectedString = "test2.java";
            Assert.Contains(expectedString, data);
        }
        [Fact]
        public void CanDeleteFileAsync_File_ShouldReturn_Empty_string()
        {
            // Arrange
            var apiKey = _token;
            var apiEndpoint = _uri;

            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.DeleteFileAsync("1", "protocol_upload", "event_1_arm_1", "", ReturnFormat.json).Result;

            // Assert
            Assert.Contains(string.Empty, result);
        }
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
            
            var result = redcap_api.ExportRecordsAsync(apiKey, "record", InputFormat.json, RedcapDataType.flat, records, fields, forms, events, RawOrLabel.raw, RawOrLabelHeaders.raw, false, ReturnFormat.json, false, false, null).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            Assert.Contains("1", data);
        }
    }
}
