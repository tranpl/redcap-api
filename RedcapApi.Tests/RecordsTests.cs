using NUnit.Framework;

using Redcap.Models;

using System.Text.Json;
namespace RedcapApi.Tests
{
    public class TestPerson
    {
        public string record_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
    [TestFixture]
    public class RecordsTest
    {
        private Redcap.RedcapApi _redcapApi;
        private string _token = "3700B3FCDE4316B6A994B3B2EFEF7516";

        [OneTimeSetUp]
        public void Setup()
        {
            _redcapApi = new Redcap.RedcapApi("http://localhost/redcap/api/");
        }
        [Test]
        [TestCase("1", "form_1")]
        public async Task ExportRecords_ShouldReturn_ListRecords(string recordId, string form)
        {
            //Arrange
            string[] fields = ["record_id", "first_name", "last_name"];
            string[] forms = [];
            forms.Append(form);
            var apiResult = await _redcapApi.ExportRecordAsync(
                token: _token,
                content: Content.Record,
                record: recordId,
                format: RedcapFormat.json,
                redcapDataType: RedcapDataType.flat,
                fields: fields,
                forms: forms,
                events: null,
                rawOrLabel:  RawOrLabel.raw,
                rawOrLabelHeaders: RawOrLabelHeaders.raw,
                exportCheckboxLabel: false,
                onErrorFormat: RedcapReturnFormat.json,
                exportSurveyFields: true,
                exportDataAccessGroups: false,
                filterLogic: null,
                dateRangeBegin: null,
                dateRangeEnd: null,
                csvDelimiter: CsvDelimiter.comma,
                decimalCharacter: DecimalCharacter.dot,
                exportBlankForGrayFormStatus: false);
            //Act
            var returnedData = JsonSerializer.Deserialize<TestPerson[]>(apiResult);
            var firstPerson = returnedData.FirstOrDefault();
            //Assert
            Assert.That(firstPerson, Is.Not.Null);
            Assert.That(firstPerson.record_id, Is.EqualTo(recordId));
            Assert.That(firstPerson.first_name, Is.Not.Null);
            Assert.That(firstPerson.last_name, Is.Not.Null);
            
        }

    }
}