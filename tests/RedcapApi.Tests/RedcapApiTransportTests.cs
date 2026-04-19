using System.Net.Http;

using Xunit;

using Redcap.Interfaces;
using Redcap.Models;

namespace RedcapApi.Tests;

public class RedcapApiTransportTests
{
    [Fact]
    public async Task ExportDagsAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportDagsAsync("token123", Content.Dag, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("dag", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        }
    }

    [Fact]
    public async Task ImportDagsAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapDag> { new() { GroupName = "CA Site", UniqueGroupName = "ca_site" } };

        await api.ImportDagsAsync("token123", Content.Dag, RedcapAction.Import, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("dag", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("import", transport.LastDictionaryPayload["action"]);
            Assert.Equal("json", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Contains("ca_site", transport.LastDictionaryPayload["data"]);
        }
    }

    [Fact]
    public async Task DeleteDagsAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteDagsAsync("token123", Content.Dag, RedcapAction.Delete, new[] { "ca_site", "fl_site" });

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("dag", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
            Assert.Equal("ca_site", transport.LastDictionaryPayload["dags[0]"]);
            Assert.Equal("fl_site", transport.LastDictionaryPayload["dags[1]"]);
        }
    }

    [Fact]
    public async Task DeleteDagsAsync_WithNoItems_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.DeleteDagsAsync("token123", Content.Dag, RedcapAction.Delete, Array.Empty<string>());

        Assert.Contains("No dags to delete", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task SwitchDagAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.SwitchDagAsync("token123", new RedcapDag { UniqueGroupName = "ca_site" });

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("ca_site", transport.LastDictionaryPayload!["dag"]);
            Assert.Equal("dag", transport.LastDictionaryPayload["content"]);
            Assert.Equal("switch", transport.LastDictionaryPayload["action"]);
        }
    }

    [Fact]
    public async Task ExportUserDagAssignmentAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportUserDagAssignmentAsync("token123", Content.UserDagMapping, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("userDagMapping", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        }
    }

    [Fact]
    public async Task ImportUserDagAssignmentAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<TestUserDagAssignment> { new() { Username = "alice", RedcapDataAccessGroup = "ca_site" } };

        await api.ImportUserDagAssignmentAsync("token123", Content.UserDagMapping, RedcapAction.Import, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("userDagMapping", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("import", transport.LastDictionaryPayload["action"]);
            Assert.Equal("json", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Contains("ca_site", transport.LastDictionaryPayload["data"]);
        }
    }

    [Fact]
    public async Task ImportUserDagAssignmentAsync_WithNoData_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.ImportUserDagAssignmentAsync("token123", Content.UserDagMapping, RedcapAction.Import, RedcapFormat.json, new List<TestUserDagAssignment>());

        Assert.Contains("No data to import", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task GenerateNextRecordNameAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.GenerateNextRecordNameAsync("token123");

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("generateNextRecordName", transport.LastDictionaryPayload!["content"]);
    }

    [Fact]
    public async Task GenerateNextRecordNameAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.GenerateNextRecordNameAsync("token123", Content.GenerateNextRecordName);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("generateNextRecordName", transport.LastDictionaryPayload!["content"]);
    }

    [Fact]
    public async Task ExportRecordsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRecordsAsync(
            token: "token123",
            format: RedcapFormat.csv,
            redcapDataType: RedcapDataType.flat,
            records: new[] { "1", "2" },
            fields: new[] { "field_1", "field_2" },
            forms: new[] { "form_1" },
            events: new[] { "event_1_arm_1" },
            rawOrLabel: RawOrLabel.label,
            rawOrLabelHeaders: RawOrLabelHeaders.label,
            exportCheckboxLabel: true,
            exportSurveyFields: true,
            exportDataAccessGroups: true,
            filterLogic: "[age] > 30",
            dateRangeBegin: new DateTime(2024, 1, 2, 3, 4, 5),
            dateRangeEnd: new DateTime(2024, 1, 3, 3, 4, 5),
            csvDelimiter: CsvDelimiter.comma,
            decimalCharacter: DecimalCharacter.dot,
            exportBlankForGrayFormStatus: true,
            combineCheckboxOptions: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("record", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("flat", transport.LastDictionaryPayload["type"]);
            Assert.Equal("1,2", transport.LastDictionaryPayload["records"]);
            Assert.Equal("field_1,field_2", transport.LastDictionaryPayload["fields"]);
            Assert.Equal("form_1", transport.LastDictionaryPayload["forms"]);
            Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["events"]);
            Assert.Equal("label", transport.LastDictionaryPayload["rawOrLabel"]);
            Assert.Equal("label", transport.LastDictionaryPayload["rawOrLabelHeaders"]);
            Assert.Equal("True", transport.LastDictionaryPayload["exportCheckboxLabel"]);
            Assert.Equal("True", transport.LastDictionaryPayload["exportSurveyFields"]);
            Assert.Equal("True", transport.LastDictionaryPayload["exportDataAccessGroups"]);
            Assert.Equal("[age] > 30", transport.LastDictionaryPayload["filterLogic"]);
            Assert.Equal("comma", transport.LastDictionaryPayload["csvDelimiter"]);
            Assert.Equal("dot", transport.LastDictionaryPayload["decimalCharacter"]);
            Assert.Equal("True", transport.LastDictionaryPayload["exportBlankForGrayFormStatus"]);
            Assert.Equal("True", transport.LastDictionaryPayload["combineCheckboxOptions"]);
        }
    }

    [Fact]
    public async Task ExportRecordsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRecordsAsync("token123", Content.Record, records: new[] { "5" }, rawOrLabelHeaders: RawOrLabelHeaders.label);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("record", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("5", transport.LastDictionaryPayload["records"]);
        Assert.Equal("label", transport.LastDictionaryPayload["rawOrLabelHeaders"]);
    }

    [Fact]
    public async Task ExportRecordsAsync_DefaultOverload_JsonFormat_OmitsCsvSpecificOptionalKeys()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRecordsAsync(
            token: "token123",
            format: RedcapFormat.json,
            redcapDataType: RedcapDataType.flat,
            records: null,
            fields: null,
            forms: null,
            events: null,
            decimalCharacter: DecimalCharacter.none,
            combineCheckboxOptions: false,
            exportSurveyFields: false,
            exportDataAccessGroups: false,
            exportCheckboxLabel: false);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.False(transport.LastDictionaryPayload!.ContainsKey("csvDelimiter"));
        Assert.False(transport.LastDictionaryPayload.ContainsKey("decimalCharacter"));
        Assert.False(transport.LastDictionaryPayload.ContainsKey("combineCheckboxOptions"));
        Assert.False(transport.LastDictionaryPayload.ContainsKey("records"));
        Assert.False(transport.LastDictionaryPayload.ContainsKey("fields"));
    }

    [Fact]
    public async Task ExportRecordAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRecordAsync(
            token: "token123",
            content: Content.Record,
            record: "7",
            format: RedcapFormat.csv,
            redcapDataType: RedcapDataType.flat,
            fields: new[] { "field_a" },
            forms: new[] { "form_a" },
            events: new[] { "event_a" },
            rawOrLabel: RawOrLabel.label,
            rawOrLabelHeaders: RawOrLabelHeaders.label,
            exportCheckboxLabel: true,
            onErrorFormat: RedcapReturnFormat.xml,
            exportSurveyFields: true,
            exportDataAccessGroups: true,
            filterLogic: "[id] = 7",
            dateRangeBegin: new DateTime(2024, 2, 1, 1, 1, 1),
            dateRangeEnd: new DateTime(2024, 2, 2, 1, 1, 1),
            csvDelimiter: CsvDelimiter.comma,
            decimalCharacter: DecimalCharacter.dot,
            exportBlankForGrayFormStatus: true,
            combineCheckboxOptions: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("7", transport.LastDictionaryPayload!["records"]);
            Assert.Equal("label", transport.LastDictionaryPayload["rawOrLabelHeaders"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Equal("True", transport.LastDictionaryPayload["exportBlankForGrayFormStatus"]);
            Assert.Equal("True", transport.LastDictionaryPayload["combineCheckboxOptions"]);
        }
    }

    [Fact]
    public async Task ImportRecordsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<TestRecordPayload> { new() { RecordId = "1", FirstName = "Alice" } };

        await api.ImportRecordsAsync("token123", RedcapFormat.json, RedcapDataType.flat, OverwriteBehavior.overwrite, true, true, data, dateFormat: "YMD", csvDelimiter: CsvDelimiter.comma, returnContent: ReturnContent.ids);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("record", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("overwrite", transport.LastDictionaryPayload["overwriteBehavior"]);
            Assert.Equal("True", transport.LastDictionaryPayload["forceAutoNumber"]);
            Assert.Equal("True", transport.LastDictionaryPayload["backgroundProcess"]);
            Assert.Equal("YMD", transport.LastDictionaryPayload["dateFormat"]);
            Assert.Equal("ids", transport.LastDictionaryPayload["returnContent"]);
            Assert.Contains("Alice", transport.LastDictionaryPayload["data"]);
        }
    }

    [Fact]
    public async Task ImportRecordsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<TestRecordPayload> { new() { RecordId = "2", FirstName = "Bob" } };

        await api.ImportRecordsAsync("token123", Content.Record, RedcapFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, false, data);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("record", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("normal", transport.LastDictionaryPayload["overwriteBehavior"]);
        Assert.Contains("Bob", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportSurveyAccessCodeAsync_UsesInjectedTransport()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.ExportSurveyAccessCodeAsync("token123", "1", "survey_form", "event_1", 2);

        Assert.Equal("transport-response", result);
        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyAccessCode", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
        Assert.Equal("survey_form", transport.LastDictionaryPayload["instrument"]);
        Assert.Equal("event_1", transport.LastDictionaryPayload["event"]);
        Assert.Equal("2", transport.LastDictionaryPayload["repeat_instance"]);
    }

    [Fact]
    public async Task DeleteRecordsAsync_UsesInjectedTransportAndIncludesDeleteLogging()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteRecordsAsync("token123", new[] { "10", "11" }, 1, deleteLogging: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("record", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("10", transport.LastDictionaryPayload["records[0]"]);
        Assert.Equal("11", transport.LastDictionaryPayload["records[1]"]);
        Assert.Equal("True", transport.LastDictionaryPayload["delete_logging"]);
    }

    [Fact]
    public async Task DeleteRecordsAsync_DetailedOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteRecordsAsync(
            "token123",
            Content.Record,
            RedcapAction.Delete,
            new[] { "1" },
            2,
            new RedcapInstrument { InstrumentName = "demographics" },
            new RedcapEvent { EventName = "event_1_arm_1" },
            new RedcapRepeatInstance { RepeatInstance = 3 },
            deleteLogging: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("demographics", transport.LastDictionaryPayload!["instrument"]);
            Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["event"]);
            Assert.Equal("3", transport.LastDictionaryPayload["repeat_instance"]);
            Assert.Equal("True", transport.LastDictionaryPayload["delete_logging"]);
        }
    }

    [Fact]
    public async Task DeleteRecordsAsync_ContentArmOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteRecordsAsync("token123", Content.Record, RedcapAction.Delete, new[] { "1", "2" }, 2, deleteLogging: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("record", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("2", transport.LastDictionaryPayload["arm"]);
        Assert.Equal("True", transport.LastDictionaryPayload["delete_logging"]);
    }

    [Fact]
    public async Task DeleteRecordsAsync_WithNoRecords_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.DeleteRecordsAsync("token123", Array.Empty<string>(), 1);

        Assert.Contains("Please provide the records", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task RenameRecordAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.RenameRecordAsync("token123", Content.Record, RedcapAction.Rename, "old-id", "new-id", 1);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("old-id", transport.LastDictionaryPayload!["record"]);
            Assert.Equal("new-id", transport.LastDictionaryPayload["new_record_name"]);
            Assert.Equal("1", transport.LastDictionaryPayload["arm"]);
        }
    }

    [Fact]
    public async Task RandomizeRecord_UsesInjectedTransport()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.RandomizeRecord("token123", Content.Record, RedcapAction.Randomize, "55", "7", RedcapFormat.json, returnAlt: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("randomize", transport.LastDictionaryPayload!["action"]);
        Assert.Equal("record", transport.LastDictionaryPayload["content"]);
        Assert.Equal("55", transport.LastDictionaryPayload["record"]);
        Assert.Equal("7", transport.LastDictionaryPayload["randomization_id"]);
        Assert.Equal("True", transport.LastDictionaryPayload["returnAlt"]);
    }

    [Fact]
    public async Task ExportRepeatingInstrumentsAndEvents_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRepeatingInstrumentsAndEvents("token123", RedcapFormat.odm);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("repeatingFormsEvents", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("odm", transport.LastDictionaryPayload["format"]);
    }

    [Fact]
    public async Task ExportRepeatingInstrumentsAndEvents_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRepeatingInstrumentsAndEvents("token123", Content.RepeatingFormsEvents, RedcapFormat.json);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("repeatingFormsEvents", transport.LastDictionaryPayload!["content"]);
    }

    [Fact]
    public async Task ImportRepeatingInstrumentsAndEvents_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new[] { new { instrument_name = "form_a", event_name = "event_1_arm_1" } }.ToList();

        await api.ImportRepeatingInstrumentsAndEvents("token123", data, format: RedcapFormat.json, returnFormat: RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("repeatingFormsEvents", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Contains("form_a", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportReports_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportReportsAsync("token123", 4, RedcapFormat.csv, RedcapReturnFormat.xml, RawOrLabel.label, RawOrLabelHeaders.label, exportCheckboxLabel: true, csvDelimiter: ",", decimalCharacter: ".");

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("report", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("4", transport.LastDictionaryPayload["report_id"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Equal("label", transport.LastDictionaryPayload["rawOrLabel"]);
            Assert.Equal("label", transport.LastDictionaryPayload["rawOrLabelHeaders"]);
            Assert.Equal("True", transport.LastDictionaryPayload["exportCheckboxLabel"]);
            Assert.Equal(",", transport.LastDictionaryPayload["csvDelimiter"]);
            Assert.Equal(".", transport.LastDictionaryPayload["decimalCharacter"]);
        }
    }

    [Fact]
    public async Task ExportReports_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportReportsAsync("token123", Content.Report, 5);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("report", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("5", transport.LastDictionaryPayload["report_id"]);
    }

    [Fact]
    public async Task ExportReports_ContentOverload_WithDefaults_OmitsOptionalKeys()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportReportsAsync("token123", Content.Report, 7, RedcapFormat.json, RedcapReturnFormat.json, RawOrLabel.raw, RawOrLabelHeaders.raw, exportCheckboxLabel: false, csvDelimiter: null, decimalCharacter: null);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("report", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("7", transport.LastDictionaryPayload["report_id"]);
        Assert.False(transport.LastDictionaryPayload.ContainsKey("exportCheckboxLabel"));
        Assert.False(transport.LastDictionaryPayload.ContainsKey("csvDelimiter"));
        Assert.False(transport.LastDictionaryPayload.ContainsKey("decimalCharacter"));
    }

    [Fact]
    public async Task ExportUsersAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportUsersAsync("token123", RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("user", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        }
    }

    [Fact]
    public async Task ExportUsersAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportUsersAsync("token123", Content.User, RedcapFormat.json, RedcapReturnFormat.json);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("user", transport.LastDictionaryPayload!["content"]);
    }

    [Fact]
    public async Task ImportUsersAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapUser>
        {
            new()
            {
                Username = "alice",
                Design = "1",
                ApiExport = "1"
            }
        };

        await api.ImportUsersAsync("token123", data, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("user", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Contains("alice", transport.LastDictionaryPayload["data"]);
        }
    }

    [Fact]
    public async Task ImportUsersAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapUser>
        {
            new()
            {
                Username = "bob",
                Design = "1"
            }
        };

        await api.ImportUsersAsync("token123", Content.User, data);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("user", transport.LastDictionaryPayload!["content"]);
        Assert.Contains("bob", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task DeleteUsersAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteUsersAsync("token123", new List<string> { "alice", "bob" });

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("user", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
            Assert.Equal("alice", transport.LastDictionaryPayload["users[0]"]);
            Assert.Equal("bob", transport.LastDictionaryPayload["users[1]"]);
        }
    }

    [Fact]
    public async Task ExportUserRolesAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportUserRolesAsync("token123", Content.UserRole, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("userRole", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        }
    }

    [Fact]
    public async Task ImportUserRolesAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<TestUserRole>
        {
            new()
            {
                UniqueRoleName = "U-123",
                RoleLabel = "Coordinator",
                ApiExport = "1"
            }
        };

        await api.ImportUserRolesAsync("token123", data, Content.UserRole, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("token123", transport.LastDictionaryPayload!["token"]);
            Assert.Equal("userRole", transport.LastDictionaryPayload["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Contains("Coordinator", transport.LastDictionaryPayload["data"]);
        }
    }

    [Fact]
    public async Task DeleteUserRolesAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteUserRolesAsync("token123", new List<string> { "U-123", "U-456" });

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("userRole", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
            Assert.Equal("U-123", transport.LastDictionaryPayload["roles[0]"]);
            Assert.Equal("U-456", transport.LastDictionaryPayload["roles[1]"]);
        }
    }

    [Fact]
    public async Task ExportUserRoleAssignmentAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportUserRoleAssignmentAsync("token123", Content.UserRoleMapping, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("userRoleMapping", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        }
    }

    [Fact]
    public async Task ImportUserRoleAssignmentAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<TestUserRoleAssignment>
        {
            new()
            {
                Username = "alice",
                UniqueRoleName = "U-123"
            }
        };

        await api.ImportUserRoleAssignmentAsync("token123", data, Content.UserRoleMapping, RedcapAction.Import, RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        {
            Assert.Equal("token123", transport.LastDictionaryPayload!["token"]);
            Assert.Equal("userRoleMapping", transport.LastDictionaryPayload["content"]);
            Assert.Equal("import", transport.LastDictionaryPayload["action"]);
            Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
            Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
            Assert.Contains("U-123", transport.LastDictionaryPayload["data"]);
        }
    }

    [Fact]
    public async Task ExportEventsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportEventsAsync("token123", RedcapFormat.csv, new[] { "1" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("event", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
    }

    [Fact]
    public async Task ExportEventsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportEventsAsync("token123", Content.Event, RedcapFormat.csv, new[] { "1" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("event", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
    }

    [Fact]
    public async Task ImportEventsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapEvent> { new() { EventName = "baseline", ArmNumber = "1" } };

        await api.ImportEventsAsync("token123", Content.Event, RedcapAction.Import, Override.False, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("event", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("import", transport.LastDictionaryPayload["action"]);
        Assert.Equal("false", transport.LastDictionaryPayload["override"]);
        Assert.Contains("baseline", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ImportEventsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapEvent> { new() { EventName = "baseline", ArmNumber = "1" } };

        await api.ImportEventsAsync("token123", Override.False, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("event", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("import", transport.LastDictionaryPayload["action"]);
        Assert.Contains("baseline", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task DeleteEventsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteEventsAsync("token123", Content.Event, RedcapAction.Delete, new[] { "event_1_arm_1" });

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("event", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["events[0]"]);
    }

    [Fact]
    public async Task DeleteEventsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteEventsAsync("token123", new[] { "event_1_arm_1" });

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("event", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["events[0]"]);
    }

    [Fact]
    public async Task DeleteEventsAsync_WithNoEvents_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.DeleteEventsAsync("token123", Array.Empty<string>());

        Assert.Contains("No events to delete", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task ExportInstrumentsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportInstrumentsAsync("token123", RedcapFormat.csv);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("instrument", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
    }

    [Fact]
    public async Task ExportInstrumentMappingAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportInstrumentMappingAsync("token123", Content.FormEventMapping, RedcapFormat.csv, new[] { "1" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("formEventMapping", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ImportInstrumentMappingAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<FormEventMapping>
        {
            new() { arm_num = "1", unique_event_name = "event_1_arm_1", form = "demographics" }
        };

        await api.ImportInstrumentMappingAsync("token123", RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("formEventMapping", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Contains("demographics", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportMetaDataAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportMetaDataAsync("token123", RedcapFormat.csv, new[] { "record_id" }, new[] { "demographics" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("metadata", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("record_id", transport.LastDictionaryPayload["fields[0]"]);
        Assert.Equal("demographics", transport.LastDictionaryPayload["forms[0]"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ImportMetaDataAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapMetaData> { new() { field_name = "record_id", form_name = "demographics", field_type = "text" } };

        await api.ImportMetaDataAsync("token123", Content.MetaData, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("metadata", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Contains("record_id", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ImportMetaDataAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapMetaData> { new() { field_name = "sex", form_name = "demographics", field_type = "radio" } };

        await api.ImportMetaDataAsync("token123", RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("metadata", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Contains("sex", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task CreateProjectAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapProject> { new() { project_title = "My Project", purpose = ProjectPurpose.Other } };

        await api.CreateProjectAsync("token123", RedcapFormat.json, data, RedcapReturnFormat.xml, "<odm />");

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Contains("My Project", transport.LastDictionaryPayload["data"]);
        Assert.Equal("<odm />", transport.LastDictionaryPayload["odm"]);
    }

    [Fact]
    public async Task CreateProjectAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapProject> { new() { project_title = "My Project 2", purpose = ProjectPurpose.Other } };

        await api.CreateProjectAsync("token123", Content.Project, RedcapFormat.json, data, RedcapReturnFormat.xml, "<odm2 />");

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project", transport.LastDictionaryPayload!["content"]);
        Assert.Contains("My Project 2", transport.LastDictionaryPayload["data"]);
        Assert.Equal("<odm2 />", transport.LastDictionaryPayload["odm"]);
    }

    [Fact]
    public async Task ImportProjectInfoAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var info = new RedcapProjectInfo { ProjectTitle = "Updated Project", SurveysEnabled = 1 };

        await api.ImportProjectInfoAsync("token123", Content.ProjectSettings, RedcapFormat.json, info);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project_settings", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Contains("Updated Project", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportProjectInfoAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportProjectInfoAsync("token123", RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ExportProjectXmlAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportProjectXmlAsync(
            token: "token123",
            content: Content.ProjectXml,
            returnMetadataOnly: true,
            records: new[] { "1", "2" },
            events: new[] { "event_1_arm_1" },
            returnFormat: RedcapReturnFormat.xml,
            exportSurveyFields: true,
            exportDataAccessGroups: true,
            filterLogic: "[record_id] = '1'",
            exportFiles: true);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project_xml", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
        Assert.Equal("True", transport.LastDictionaryPayload["returnMetadataOnly"]);
        Assert.Equal("1,2", transport.LastDictionaryPayload["records"]);
        Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["events"]);
        Assert.Equal("True", transport.LastDictionaryPayload["exportSurveyFields"]);
        Assert.Equal("True", transport.LastDictionaryPayload["exportDataAccessGroups"]);
        Assert.Equal("[record_id] = '1'", transport.LastDictionaryPayload["filterLogic"]);
        Assert.Equal("True", transport.LastDictionaryPayload["exportFiles"]);
    }

    [Fact]
    public async Task ExportArmsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportArmsAsync("token123", RedcapFormat.csv, new[] { "1" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ExportArmsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportArmsAsync("token123", Content.Arm, RedcapFormat.csv, new[] { "1" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
    }

    [Fact]
    public async Task ImportArmsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<TestArmPayload> { new() { arm_num = "1", name = "Arm A" } };

        await api.ImportArmsAsync("token123", Override.False, RedcapAction.Import, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("import", transport.LastDictionaryPayload["action"]);
        Assert.Equal("false", transport.LastDictionaryPayload["override"]);
        Assert.Contains("Arm A", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ImportArmsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapArm> { new() { ArmNumber = "1", Name = "Arm B" } };

        await api.ImportArmsAsync("token123", Content.Arm, Override.False, RedcapAction.Import, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("import", transport.LastDictionaryPayload["action"]);
        Assert.Contains("Arm B", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ImportArmsAsync_ContentOverload_ListRedcapArmVariant_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<RedcapArm> { new() { ArmNumber = "2", Name = "Arm C" } };

        await api.ImportArmsAsync<object>("token123", Content.Arm, Override.False, RedcapAction.Import, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("import", transport.LastDictionaryPayload["action"]);
        Assert.Contains("Arm C", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportFieldNamesAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportFieldNamesAsync("token123", RedcapFormat.csv, "record_id", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("exportFieldNames", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("record_id", transport.LastDictionaryPayload["field"]);
    }

    [Fact]
    public async Task ExportFieldNamesAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportFieldNamesAsync("token123", Content.ExportFieldNames, RedcapFormat.csv, "record_id", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("exportFieldNames", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("record_id", transport.LastDictionaryPayload["field"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task DeleteArmsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteArmsAsync("token123", Content.Arm, RedcapAction.Delete, new[] { "1", "2" });

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
        Assert.Equal("2", transport.LastDictionaryPayload["arms[1]"]);
    }

    [Fact]
    public async Task DeleteArmsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteArmsAsync("token123", new[] { "1", "2" });

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("arm", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
        Assert.Equal("2", transport.LastDictionaryPayload["arms[1]"]);
    }

    [Fact]
    public async Task DeleteArmsAsync_WithNoArms_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.DeleteArmsAsync("token123", Array.Empty<string>());

        Assert.Contains("No arm to delete", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task ExportFileAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            var result = await api.ExportFileAsync("token123", Content.File, RedcapAction.Export, "1", "upload", "event_1_arm_1", "2", RedcapReturnFormat.xml, tempFolder);

            Assert.Equal("transport-response", result);
            Assert.NotNull(transport.LastDictionaryPayload);
            Assert.Equal("file", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("export", transport.LastDictionaryPayload["action"]);
            Assert.Equal(tempFolder, transport.LastDictionaryPayload["filePath"]);
            Assert.Equal("2", transport.LastDictionaryPayload["repeat_instance"]);
        }
        finally
        {
            if(Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }

    [Fact]
    public async Task ExportFileAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            await api.ExportFileAsync("token123", "1", "upload", "event_1_arm_1", "2", RedcapReturnFormat.xml, tempFolder);

            Assert.NotNull(transport.LastDictionaryPayload);
            Assert.Equal("file", transport.LastDictionaryPayload!["content"]);
            Assert.Equal("export", transport.LastDictionaryPayload["action"]);
            Assert.Equal("2", transport.LastDictionaryPayload["repeat_instance"]);
            Assert.False(transport.LastDictionaryPayload.ContainsKey("filePath"));
        }
        finally
        {
            if(Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }

    [Fact]
    public async Task ExportFileAsync_DefaultOverload_WithMissingFilePath_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.ExportFileAsync("token123", "1", "upload", "event_1_arm_1", "1", RedcapReturnFormat.xml, filePath: null);

        Assert.Contains("file path", result, StringComparison.OrdinalIgnoreCase);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task ExportFileAsync_DefaultOverload_WithMissingRecord_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            var result = await api.ExportFileAsync("token123", record: null, field: "upload", eventName: "event_1_arm_1", repeatInstance: "1", returnFormat: RedcapReturnFormat.xml, filePath: tempFolder);

            Assert.Contains("No record provided", result, StringComparison.OrdinalIgnoreCase);
            Assert.Null(transport.LastDictionaryPayload);
        }
        finally
        {
            if(Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }

    [Fact]
    public async Task ExportFileAsync_ContentOverload_WithNullRepeatInstance_OmitsRepeatInstanceKey()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            await api.ExportFileAsync("token123", Content.File, RedcapAction.Export, "1", "upload", "event_1_arm_1", null, RedcapReturnFormat.xml, tempFolder);

            Assert.NotNull(transport.LastDictionaryPayload);
            Assert.False(transport.LastDictionaryPayload!.ContainsKey("repeat_instance"));
            Assert.Equal(tempFolder, transport.LastDictionaryPayload["filePath"]);
        }
        finally
        {
            if(Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }

    [Fact]
    public async Task ImportFileAsync_DefaultOverload_UsesMultipartPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempFolder);
        var fileName = "sample.txt";
        await File.WriteAllTextAsync(Path.Combine(tempFolder, fileName), "sample-body");

        try
        {
            await api.ImportFileAsync("token123", "1", "upload", "event_1_arm_1", "1", fileName, tempFolder, RedcapReturnFormat.xml);

            Assert.NotNull(transport.LastMultipartPayload);
            var fields = await ReadMultipartFieldsAsync(transport.LastMultipartPayload!);
            Assert.Equal("token123", fields["token"]);
            Assert.Equal("file", fields["content"]);
            Assert.Equal("import", fields["action"]);
            Assert.Equal("1", fields["record"]);
            Assert.Equal("upload", fields["field"]);
            Assert.Equal("event_1_arm_1", fields["event"]);
            Assert.Equal("1", fields["repeat_instance"]);
            Assert.True(fields.ContainsKey("file"));
        }
        finally
        {
            Directory.Delete(tempFolder, true);
        }
    }

    [Fact]
    public async Task ImportFileAsync_ContentOverload_UsesMultipartPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempFolder);
        var fileName = "sample2.txt";
        await File.WriteAllTextAsync(Path.Combine(tempFolder, fileName), "sample-body-2");

        try
        {
            await api.ImportFileAsync("token123", Content.File, RedcapAction.Import, "1", "upload", "event_1_arm_1", "2", fileName, tempFolder, RedcapReturnFormat.xml);

            Assert.NotNull(transport.LastMultipartPayload);
            var fields = await ReadMultipartFieldsAsync(transport.LastMultipartPayload!);
            Assert.Equal("file", fields["content"]);
            Assert.Equal("import", fields["action"]);
            Assert.Equal("2", fields["repeat_instance"]);
        }
        finally
        {
            Directory.Delete(tempFolder, true);
        }
    }

    [Fact]
    public async Task DeleteFileAsync_ContentOverload_UsesMultipartPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteFileAsync("token123", Content.File, RedcapAction.Delete, "1", "upload", "event_1_arm_1", "3", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastMultipartPayload);
        var fields = await ReadMultipartFieldsAsync(transport.LastMultipartPayload!);
        Assert.Equal("token123", fields["token"]);
        Assert.Equal("file", fields["content"]);
        Assert.Equal("delete", fields["action"]);
        Assert.Equal("3", fields["repeat_instance"]);
    }

    [Fact]
    public async Task DeleteFileAsync_DefaultOverload_UsesMultipartPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteFileAsync("token123", "1", "upload", "event_1_arm_1", "4", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastMultipartPayload);
        var fields = await ReadMultipartFieldsAsync(transport.LastMultipartPayload!);
        Assert.Equal("file", fields["content"]);
        Assert.Equal("delete", fields["action"]);
        Assert.Equal("4", fields["repeat_instance"]);
    }

    [Fact]
    public async Task DeleteFileAsync_DefaultOverload_WithNullRepeatInstance_DefaultsToOne()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteFileAsync("token123", "1", "upload", "event_1_arm_1", null, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastMultipartPayload);
        var fields = await ReadMultipartFieldsAsync(transport.LastMultipartPayload!);
        Assert.Equal("1", fields["repeat_instance"]);
    }

    [Fact]
    public async Task DeleteFileAsync_ContentOverload_WithNullRepeatInstance_DefaultsToOne()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteFileAsync("token123", Content.File, RedcapAction.Delete, "1", "upload", "event_1_arm_1", null, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastMultipartPayload);
        var fields = await ReadMultipartFieldsAsync(transport.LastMultipartPayload!);
        Assert.Equal("1", fields["repeat_instance"]);
    }

    [Fact]
    public async Task CreateFolderFileRepositoryAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.CreateFolderFileRepositoryAsync("token123", Content.FileRepository, RedcapAction.CreateFolder, "new-folder", RedcapFormat.json, "10", "20", "30", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("fileRepository", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("createFolder", transport.LastDictionaryPayload["action"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Equal("10", transport.LastDictionaryPayload["folder_id"]);
        Assert.Equal("20", transport.LastDictionaryPayload["dag_id"]);
        Assert.Equal("30", transport.LastDictionaryPayload["role_id"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task CreateFolderFileRepositoryAsync_WithNoName_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.CreateFolderFileRepositoryAsync("token123", Content.FileRepository, RedcapAction.CreateFolder, null!, RedcapFormat.json);

        Assert.Contains("Please provide a valid name", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task ExportFilesFoldersFileRepositoryAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportFilesFoldersFileRepositoryAsync("token123", folderId: "12", format: RedcapFormat.csv, returnFormat: RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("fileRepository", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("list", transport.LastDictionaryPayload["action"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
        Assert.Equal("12", transport.LastDictionaryPayload["folder_id"]);
    }

    [Fact]
    public async Task ExportFileFileRepositoryAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportFileFileRepositoryAsync("token123", Content.FileRepository, RedcapAction.Export, "55", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("fileRepository", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("export", transport.LastDictionaryPayload["action"]);
        Assert.Equal("55", transport.LastDictionaryPayload["doc_id"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ImportFileRepositoryAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ImportFileRepositoryAsync("token123", Content.FileRepository, RedcapAction.Import, "file-content", "9", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("fileRepository", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("import", transport.LastDictionaryPayload["action"]);
        Assert.Equal("file-content", transport.LastDictionaryPayload["file"]);
        Assert.Equal("9", transport.LastDictionaryPayload["folder_id"]);
    }

    [Fact]
    public async Task DeleteFileRepositoryAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.DeleteFileRepositoryAsync("token123", Content.FileRepository, RedcapAction.Delete, "55", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("fileRepository", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("delete", transport.LastDictionaryPayload["action"]);
        Assert.Equal("55", transport.LastDictionaryPayload["doc_id"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task DeleteFileRepositoryAsync_WithNoDocId_ReturnsErrorMessage()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        var result = await api.DeleteFileRepositoryAsync("token123");

        Assert.Contains("Please provide a document id", result);
        Assert.Null(transport.LastDictionaryPayload);
    }

    [Fact]
    public async Task ExportSurveyLinkAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyLinkAsync("token123", Content.SurveyLink, "1", "survey_a", "event_1_arm_1", 2, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyLink", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
        Assert.Equal("survey_a", transport.LastDictionaryPayload["instrument"]);
        Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["event"]);
        Assert.Equal("2", transport.LastDictionaryPayload["repeat_instance"]);
    }

    [Fact]
    public async Task ExportSurveyLinkAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyLinkAsync("token123", "1", "survey_a", "event_1_arm_1", 2, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyLink", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
    }

    [Fact]
    public async Task ExportSurveyParticipantsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyParticipantsAsync("token123", "survey_a", "event_1_arm_1", RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("participantList", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("survey_a", transport.LastDictionaryPayload["instrument"]);
        Assert.Equal("event_1_arm_1", transport.LastDictionaryPayload["event"]);
        Assert.Equal("csv", transport.LastDictionaryPayload["format"]);
    }

    [Fact]
    public async Task ExportSurveyParticipantsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyParticipantsAsync("token123", Content.ParticipantList, "survey_a", "event_1_arm_1", RedcapFormat.csv, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("participantList", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("survey_a", transport.LastDictionaryPayload["instrument"]);
    }

    [Fact]
    public async Task ExportSurveyQueueLinkAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyQueueLinkAsync("token123", "1", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyQueueLink", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ExportSurveyQueueLinkAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyQueueLinkAsync("token123", Content.SurveyQueueLink, "1", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyQueueLink", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
    }

    [Fact]
    public async Task ExportSurveyReturnCodeAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyReturnCodeAsync("token123", Content.SurveyReturnCode, "1", "survey_a", "event_1_arm_1", "3", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyReturnCode", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("3", transport.LastDictionaryPayload["repeat_instance"]);
    }

    [Fact]
    public async Task ExportSurveyReturnCodeAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyReturnCodeAsync("token123", "1", "survey_a", "event_1_arm_1", "3", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyReturnCode", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("3", transport.LastDictionaryPayload["repeat_instance"]);
    }

    [Fact]
    public async Task ExportSurveyAccessCodeAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportSurveyAccessCodeAsync("token123", Content.SurveyAccessCode, "1", "survey_a", "event_1_arm_1", 2, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("surveyAccessCode", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("2", transport.LastDictionaryPayload["repeat_instance"]);
    }

    [Fact]
    public async Task ExportRedcapVersionAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRedcapVersionAsync("token123", RedcapFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("version", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["format"]);
    }

    [Fact]
    public async Task ExportRedcapVersionAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportRedcapVersionAsync("token123", Content.Version, RedcapFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("version", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["format"]);
    }

    [Fact]
    public async Task ExportInstrumentsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportInstrumentsAsync("token123", Content.Instrument, RedcapFormat.json);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("instrument", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
    }

    [Fact]
    public async Task ExportPDFInstrumentsAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportPDFInstrumentsAsync("token123", Content.Pdf, "1", "event_1_arm_1", "survey_a", allRecords: true, compactDisplay: true, returnFormat: RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("pdf", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
        Assert.Equal("True", transport.LastDictionaryPayload["allRecords"]);
        Assert.Equal("True", transport.LastDictionaryPayload["compactDisplay"]);
    }

    [Fact]
    public async Task ExportPDFInstrumentsAsync_WithFilePath_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            await api.ExportPDFInstrumentsAsync("token123", "1", "event_1_arm_1", "survey_a", true, tempFolder, RedcapReturnFormat.xml);

            Assert.NotNull(transport.LastDictionaryPayload);
            Assert.Equal("pdf", transport.LastDictionaryPayload!["content"]);
            Assert.Equal(tempFolder, transport.LastDictionaryPayload["filePath"]);
            Assert.Equal("True", transport.LastDictionaryPayload["allRecords"]);
        }
        finally
        {
            if(Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }

    [Fact]
    public async Task ExportPDFInstrumentsAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportPDFInstrumentsAsync("token123", "1", "event_1_arm_1", "survey_a", true, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("pdf", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
        Assert.Equal("True", transport.LastDictionaryPayload["allRecords"]);
    }

    [Fact]
    public async Task ExportLoggingAsync_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportLoggingAsync("token123", Content.Log, RedcapFormat.csv, LogType.RecordEdit, "alice", "1", "2", "2024-01-01 10:00", "2024-01-02 10:00", RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("log", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("record_edit", transport.LastDictionaryPayload["logtype"]);
        Assert.Equal("alice", transport.LastDictionaryPayload["user"]);
        Assert.Equal("1", transport.LastDictionaryPayload["record"]);
    }

    [Fact]
    public async Task ExportInstrumentMappingAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportInstrumentMappingAsync("token123", RedcapFormat.json, new[] { "1" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("formEventMapping", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("1", transport.LastDictionaryPayload["arms[0]"]);
    }

    [Fact]
    public async Task ImportInstrumentMappingAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var data = new List<FormEventMapping> { new() { arm_num = "1", unique_event_name = "event_1_arm_1", form = "survey_a" } };

        await api.ImportInstrumentMappingAsync("token123", Content.FormEventMapping, RedcapFormat.json, data, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("formEventMapping", transport.LastDictionaryPayload!["content"]);
        Assert.Contains("survey_a", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportMetaDataAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportMetaDataAsync("token123", Content.MetaData, RedcapFormat.json, new[] { "record_id" }, new[] { "survey_a" }, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("metadata", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("record_id", transport.LastDictionaryPayload["fields[0]"]);
        Assert.Equal("survey_a", transport.LastDictionaryPayload["forms[0]"]);
    }

    [Fact]
    public async Task ExportProjectInfoAsync_ContentOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportProjectInfoAsync("token123", Content.Project, RedcapFormat.json, RedcapReturnFormat.xml);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Equal("xml", transport.LastDictionaryPayload["returnFormat"]);
    }

    [Fact]
    public async Task ImportProjectInfoAsync_DefaultOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);
        var info = new RedcapProjectInfo { ProjectTitle = "Title", ProjectNotes = "Notes" };

        await api.ImportProjectInfoAsync("token123", RedcapFormat.json, info);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project_settings", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("json", transport.LastDictionaryPayload["format"]);
        Assert.Contains("Title", transport.LastDictionaryPayload["data"]);
    }

    [Fact]
    public async Task ExportProjectXmlAsync_ContentlessOverload_UsesExpectedPayload()
    {
        var transport = new FakeTransport();
        var api = new Redcap.RedcapApi("http://localhost/", transport);

        await api.ExportProjectXmlAsync("token123", true, new[] { "1" }, null, new[] { "event_1_arm_1" }, RedcapReturnFormat.xml, true, true, "[record_id] = '1'", true);

        Assert.NotNull(transport.LastDictionaryPayload);
        Assert.Equal("project_xml", transport.LastDictionaryPayload!["content"]);
        Assert.Equal("True", transport.LastDictionaryPayload["returnMetadataOnly"]);
    }

    [Fact]
    public async Task ProtectedWrapperMethods_AreReachableViaSubclass()
    {
        var transport = new FakeTransport();
        var api = new TestableRedcapApi("http://localhost/", transport);

        await api.CallMultipartWrapper(new MultipartFormDataContent());
        await api.CallDictionaryStreamWrapper(new Dictionary<string, string> { { "token", "token123" } });
        await api.CallConvertIntArrayWrapper(new[] { 1, 2 });
        await api.CallHandleFormatWrapper(RedcapFormat.json, RedcapReturnFormat.xml, RedcapDataType.flat);
        await api.CallHandleReturnContentWrapper(ReturnContent.count);
        await api.CallExtractBehaviorWrapper(OverwriteBehavior.normal);

        Assert.NotNull(transport.LastDictionaryPayload);
    }

    private static async Task<Dictionary<string, string>> ReadMultipartFieldsAsync(MultipartFormDataContent payload)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach(var part in payload)
        {
            var name = part.Headers.ContentDisposition?.Name?.Trim('"');
            if(string.IsNullOrEmpty(name))
            {
                continue;
            }

            if(part.Headers.ContentDisposition?.FileName is not null)
            {
                fields[name] = "<binary>";
                continue;
            }

            fields[name] = await part.ReadAsStringAsync();
        }

        return fields;
    }

    private sealed class TestRecordPayload
    {
        public string RecordId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
    }

    private sealed class TestUserDagAssignment
    {
        public string Username { get; set; } = string.Empty;

        public string RedcapDataAccessGroup { get; set; } = string.Empty;
    }

    private sealed class TestUserRole
    {
        public string UniqueRoleName { get; set; } = string.Empty;

        public string RoleLabel { get; set; } = string.Empty;

        public string ApiExport { get; set; } = string.Empty;
    }

    private sealed class TestUserRoleAssignment
    {
        public string Username { get; set; } = string.Empty;

        public string UniqueRoleName { get; set; } = string.Empty;
    }

    private sealed class TestArmPayload
    {
        public string arm_num { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;
    }

    private sealed class TestableRedcapApi : Redcap.RedcapApi
    {
        public TestableRedcapApi(string redcapApiUrl, IRedcapTransport transport)
            : base(redcapApiUrl, transport)
        {
        }

        public Task<string> CallMultipartWrapper(MultipartFormDataContent payload)
            => base.SendPostRequestAsync(payload, new Uri("http://localhost/"));

        public Task<Stream> CallDictionaryStreamWrapper(Dictionary<string, string> payload)
            => base.GetStreamContentAsync(payload, new Uri("http://localhost/"));

        public Task<string> CallConvertIntArrayWrapper(int[] input)
            => base.ConvertIntArraytoString(input);

        public Task<(string format, string onErrorFormat, string redcapDataType)> CallHandleFormatWrapper(RedcapFormat? format, RedcapReturnFormat? onErrorFormat, RedcapDataType? redcapDataType)
            => base.HandleFormat(format, onErrorFormat, redcapDataType);

        public Task<string> CallHandleReturnContentWrapper(ReturnContent returnContent)
            => base.HandleReturnContent(returnContent);

        public Task<string> CallExtractBehaviorWrapper(OverwriteBehavior overwriteBehavior)
            => base.ExtractBehaviorAsync(overwriteBehavior);
    }

    private sealed class FakeTransport : IRedcapTransport
    {
        public Dictionary<string, string>? LastDictionaryPayload { get; private set; }

        public MultipartFormDataContent? LastMultipartPayload { get; private set; }

        public Task<Stream> GetStreamContentAsync(Redcap.RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            LastDictionaryPayload = new Dictionary<string, string>(payload);
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task<string> SendPostRequestAsync(Redcap.RedcapApi redcapApi, MultipartFormDataContent payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            LastMultipartPayload = payload;
            return Task.FromResult("transport-response");
        }

        public Task<string> SendPostRequestAsync(Redcap.RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            LastDictionaryPayload = new Dictionary<string, string>(payload);
            return Task.FromResult("transport-response");
        }
    }
}


