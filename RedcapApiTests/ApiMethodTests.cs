using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace RedcapApiTests
{
    [TestFixture]
    public class ApiMethodTests
    {
        
        private string _apiKey = "3D57A7FA57C8A43F6C8803A84BB3957B";
        private string _apiEndpoint = "http://localhost/redcap/api/";
        public ApiMethodTests()
        {
            
            if (false) _apiKey = "BCC9D1F214B8BE2AA4F24C56ED7674E4";
        }
        [TestCase]
        public void CanExportAsync_SingleRecord_ShouldContain_string_1()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;
            
            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.GetRecordAsync("1", InputFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("1", data);
        }
        [TestCase]
        public void CanExportAsync_AllEvents_ShouldContain_event_name()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ExportEventsAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("event_name", data);

        }
        [TestCase]
        public void CanGetRecordsAsync1_AllRecords_ShouldContain_string_record_id()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.GetRecordsAsync(InputFormat.json, ReturnFormat.json, RedcapDataType.flat).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("record_id", data);

        }

        [TestCase]
        public void CanGetRecordsAsync2_AllRecords_ShouldContain_string_record_id()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;
            char[] delimiters = new char[] { ';', ',' };

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.GetRecordsAsync(InputFormat.json, ReturnFormat.json, RedcapDataType.flat, delimiters).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("record_id", data);

        }

        [TestCase]
        public void CanGetRedcapVersion_VersionNumber_Shouldontain_Number()
        {
            // Arrange
            // Assume current redcap version is 7.4.10
            var currentRedcapVersion = "7.4.10";
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.GetRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            var data = result;

            // Assert
            Assert.AreEqual(currentRedcapVersion, data);

        }

        [TestCase]
        public void CanExportRedcapVersion_VersionNumber_Shouldontain_Number()
        {
            // Arrange
            // Assume current redcap version is 7.4.10
            var currentRedcapVersion = "7.4.10";
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ExportRedcapVersionAsync(InputFormat.json, RedcapDataType.flat).Result;
            var data = result;

            // Assert
            Assert.AreEqual(currentRedcapVersion, data);

        }

        [TestCase]
        public void CanExportUsers_AllUsers_ShouldReturn_username()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;
            var username = "site_admin";
            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ExportUsersAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains(username, data);

        }

        [TestCase]
        public void CanSaveRecord1_SingleRecord_ShouldReturn_Ids()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            var record = new
            {
                record_id = "1",
                redcap_event_name = "event_1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };
            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("1", data);

        }

        [TestCase]
        public void CanSaveRecord2_SingleRecord_ShouldReturn_Ids()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;
            var dateFormat = "YMD";
            var record = new
            {
                record_id = "1",
                redcap_event_name = "event_1_arm_1",
                first_name = "John",
                last_name = "Doe"
            };

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.SaveRecordsAsync(record, ReturnContent.ids, OverwriteBehavior.overwrite, InputFormat.json, RedcapDataType.flat, ReturnFormat.json, dateFormat).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("1", data);

        }

        [TestCase]
        public void CanExportRecordsAsync_AllRecords_ShouldReturn_string_record_id()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ExportRecordsAsync(InputFormat.json, RedcapDataType.flat).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("record_id", data);
        }
        [TestCase]
        public void CanExportMetaDataAsync_Metadata_ShouldReturn_string_record_id()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;

            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ExportMetaDataAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("record_id", data);
        }
        [TestCase]
        public void CanExportArmsAsync_Arms_ShouldReturn_arms_array()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;
            // Act
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ExportArmsAsync(InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("event_1_arm_1", data);
        }

        [TestCase]
        public void CanImportEventsAsync_Events_ShouldReturn_Number()
        {
            // Arrange
            var apiKey = _apiKey;
            var apiEndpoint = _apiEndpoint;
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
            var redcapApi = new RedcapApi(apiKey, apiEndpoint);
            var result = redcapApi.ImportEventsAsync(listOfEvents,Override.False, InputFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();

            // Assert
            StringAssert.Contains("1", data);
        }

    }
}
