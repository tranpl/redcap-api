using Redcap.Models;
using Redcap.Utilities;
namespace RedcapApi.Tests
{
    public class RecordsTest: IDisposable
    {
        private Redcap.RedcapApi _redcapApi;
        private string _token = "3700B3FCDE4316B6A994B3B2EFEF7516";

        public RecordsTest()
        {
            _redcapApi = new Redcap.RedcapApi("http://localhost/redcap/api/");
        }
        [Fact]
        public async void ExportRecords_ShouldReturn_ListRecords()
        {
            //Arrange
            string recordNumber = "1";
            string[] fields = ["record_id, first_name, last_name"];
            string[] forms = ["form_1"];
            var apiResult = await _redcapApi.ExportRecordAsync(
                token: _token,
                content: Content.Record,
                record: recordNumber,
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
            var result = apiResult;
            //Assert
            Assert.Equal(",", result);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}