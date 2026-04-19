using Xunit;

using Redcap.Models;

using System.Text.Json;
namespace RedcapApi.Tests
{
    public class TestPerson
    {
        public string? record_id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
    }
    [Xunit.Trait("Category", "E2E")]
        public class RecordsTest
    {
        [Fact]
        public async Task ExportRecords_ShouldReturn_ListRecords()
        {
            var url = Environment.GetEnvironmentVariable("REDCAP_E2E_URL");
            var token = Environment.GetEnvironmentVariable("REDCAP_E2E_TOKEN") ?? string.Empty;
            var recordId = Environment.GetEnvironmentVariable("REDCAP_E2E_RECORD_ID") ?? "1";
            var form = Environment.GetEnvironmentVariable("REDCAP_E2E_FORM") ?? "form_1";

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            var redcapApi = new Redcap.RedcapApi(url);

            string[] fields = ["record_id", "first_name", "last_name"];
            string[] forms = [form];
            var apiResult = await redcapApi.ExportRecordAsync(
                token: token,
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

            var returnedData = JsonSerializer.Deserialize<TestPerson[]>(apiResult);
            var firstPerson = returnedData?.FirstOrDefault();

            Assert.NotNull(firstPerson);
            Assert.Equal(recordId, firstPerson!.record_id);
            Assert.NotNull(firstPerson.first_name);
            Assert.NotNull(firstPerson.last_name);
        }
    }
}
