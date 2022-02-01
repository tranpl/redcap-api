using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using Redcap.Broker;
using Redcap.Models;
using Redcap.Services;

using RestSharp;

using Xunit;

namespace Tests
{
    public partial class RedcapApiTests
    {
        [Theory]
        [InlineData("SOME_TOKEN", Content.Record, "1", ReturnFormat.json, RedcapDataType.flat, new string[] { }, new string[] { },
            new string[] { }, RawOrLabel.raw, RawOrLabelHeaders.raw, false, OnErrorFormat.json, false, false, "", false)]
        public async Task ExportRecordAsync_ShouldReturnSingleRecord_GivenSingleRecordId(
            string apiToken, Content content, string recordId, ReturnFormat returnFormat, RedcapDataType redcapDataType,
            string[] fields, string[] forms, string[] events, RawOrLabel rawOrLabel, RawOrLabelHeaders rawOrLabelHeaders, 
            bool exportCheckboxLabel, OnErrorFormat onErrorFormat, bool exportSurveyFields, bool exportDataAccessGroup, string filterLogic, bool exportBlankForGrayFormStatus
            )
        {
            // arrange
            var demographic = new Demographic {
                FirstName = "John",
                LastName = "Doe",
                RecordId = "1"
            };
            var record = JsonSerializer.Serialize(demographic);
            _redcapApiMock.Setup(x => x.ExportRecordAsync(
                apiToken, content, recordId, returnFormat, redcapDataType, fields, forms, events, rawOrLabel, rawOrLabelHeaders, exportCheckboxLabel,
                onErrorFormat, exportSurveyFields, exportDataAccessGroup, filterLogic, exportBlankForGrayFormStatus
                )).ReturnsAsync(record);

            // act
            var result = await _sut.ExportRecordAsync(apiToken, content, recordId, returnFormat, redcapDataType, fields, forms, events, rawOrLabel, rawOrLabelHeaders, exportCheckboxLabel,
                onErrorFormat, exportSurveyFields, exportDataAccessGroup, filterLogic, exportBlankForGrayFormStatus);
            var recordResult = JsonSerializer.Deserialize<Demographic>(result);
            // assert
            Assert.Equal("1", recordResult.RecordId);
        }
    }
}
