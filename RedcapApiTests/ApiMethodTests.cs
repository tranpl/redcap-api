using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using NUnit.Framework;

namespace RedcapApiTests
{
    [TestFixture]
    public class ApiMethodTests
    {
        [TestCase]
        public void CanExportEventsShouldContain_1()
        {
            // Arrange
            var apiKey = "3D57A7FA57C8A43F6C8803A84BB3957B";
            var apiEndpoint = "http://localhost/redcap/api/";
            
            // Act
            var redcap_api = new RedcapApi(apiKey, apiEndpoint);
            var result = redcap_api.GetRecordAsync("1", RedcapFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            var data = JsonConvert.DeserializeObject(result).ToString();

            // Assert
            StringAssert.Contains("1", data);
        }
    }
}
