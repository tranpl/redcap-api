using System.Net.Http;
using System.Text;

using Xunit;

using Redcap;
using Redcap.Http;
using Redcap.Models;
using Redcap.Utilities;

namespace RedcapApi.Tests;

public class UtilitiesTests
{
    private readonly Redcap.RedcapApi _api = new("http://localhost/");

    [Fact]
    public void GetDisplayName_ReturnsDisplayAttributeName()
    {
        Assert.Equal("project_xml", Content.ProjectXml.GetDisplayName());
        Assert.Equal("odm", RedcapFormat.odm.GetDisplayName());
    }

    [Fact]
    public void IsNullOrEmpty_HandlesNullEmptyAndPopulatedArrays()
    {
        string[]? nullArray = null;
        var emptyArray = Array.Empty<string>();
        var values = new[] { "value" };

        Assert.True(nullArray.IsNullOrEmpty());
        Assert.True(emptyArray.IsNullOrEmpty());
        Assert.False(values.IsNullOrEmpty());
    }

    [Fact]
    public async Task ConvertArrayToString_FormatsValues()
    {
        var single = await _api.ConvertArraytoString(new[] { "one" });
        var multiple = await _api.ConvertArraytoString(new[] { "one", "two", "three" });
        var invalid = await _api.ConvertArraytoString<string>(null!);

        Assert.Equal("one", single);
        Assert.Equal("one,two,three", multiple);
        Assert.Equal(string.Empty, invalid);
    }

    [Fact]
    public async Task ConvertIntArrayToString_FormatsValues()
    {
        var single = await _api.ConvertIntArraytoString(new[] { 1 });
        var multiple = await _api.ConvertIntArraytoString(new[] { 1, 2, 3 });
        var invalid = await _api.ConvertIntArraytoString(null!);

        Assert.Equal("1", single);
        Assert.Equal("1,2,3", multiple);
        Assert.Equal(string.Empty, invalid);
    }

    [Fact]
    public async Task HandleReturnContent_ReturnsExpectedValues()
    {
        Assert.Equal("ids", await _api.HandleReturnContent(ReturnContent.ids));
        Assert.Equal("count", await _api.HandleReturnContent(ReturnContent.count));
        Assert.Equal("count", await _api.HandleReturnContent((ReturnContent)999));
    }

    [Fact]
    public async Task HandleFormat_ReturnsExpectedTuple()
    {
        var result = await _api.HandleFormat(RedcapFormat.odm, RedcapReturnFormat.xml, RedcapDataType.longitudinal);
        var fallback = await _api.HandleFormat((RedcapFormat)999, (RedcapReturnFormat)999, (RedcapDataType)999);

        Assert.Equal("odm", result.format);
        Assert.Equal("xml", result.onErrorFormat);
        Assert.Equal("longitudinal", result.redcapDataType);

        Assert.Equal("json", fallback.format);
        Assert.Equal("json", fallback.onErrorFormat);
        Assert.Equal("flat", fallback.redcapDataType);
    }

    [Fact]
    public async Task ExtractBehaviorAsync_ReturnsExpectedValue()
    {
        Assert.Equal("normal", await _api.ExtractBehaviorAsync(OverwriteBehavior.normal));
        Assert.Equal("overwrite", await _api.ExtractBehaviorAsync(OverwriteBehavior.overwrite));
        Assert.Equal("overwrite", await _api.ExtractBehaviorAsync((OverwriteBehavior)999));
    }

    [Fact]
    public async Task GetProperties_UsesLowerCaseKeysAndConvertsNullableTypes()
    {
        var sample = new SampleProperties
        {
            Name = "Alice",
            IsActive = true,
            VisitDate = new DateTime(2024, 1, 2, 3, 4, 5),
            OptionalValue = null
        };

        var properties = await _api.GetProperties(sample);

        Assert.Equal("Alice", properties["name"]);
        Assert.Equal("1", properties["isactive"]);
        Assert.Equal(sample.VisitDate.Value.ToString(), properties["visitdate"]);
        Assert.True(properties.ContainsKey("optionalvalue"));
        Assert.Null(properties["optionalvalue"]);
    }

    [Fact]
    public async Task ExtractHelpers_SplitDelimitedValues()
    {
        var delimiters = new[] { ',', ';' };

        Assert.Equal(new[] { "event_1", "event_2" }, await _api.ExtractEventsAsync("event_1,event_2", delimiters));
        Assert.Equal(new[] { "field_1", "field_2" }, await _api.ExtractFieldsAsync("field_1;field_2", delimiters));
        Assert.Equal(new[] { "1", "2" }, await _api.ExtractRecordsAsync("1,2", delimiters));
        Assert.Equal(new[] { "form_1", "form_2" }, await _api.ExtractFormsAsync("form_1;form_2", delimiters));
        Assert.Equal(new[] { "1", "2" }, await _api.ExtractArmsAsync<string>("1,2", delimiters));
    }

    [Fact]
    public void CheckToken_ThrowsForMissingToken()
    {
        Assert.Throws<ArgumentNullException>(() => _api.CheckToken(string.Empty));
        var ex = Xunit.Record.Exception(() => _api.CheckToken("token"));
        Assert.Null(ex);
    }

    [Fact]
    public void GetHttpHandler_UsesUnsafeCertificateModeWhenConfigured()
    {
        _ = new Redcap.RedcapApi("http://localhost/", useInsecureCertificates: true);
        var insecureHandler = Utils.GetHttpHandler();

        _ = new Redcap.RedcapApi("http://localhost/", useInsecureCertificates: false);
        var secureHandler = Utils.GetHttpHandler();

        Assert.NotNull(insecureHandler.ServerCertificateCustomValidationCallback);
        Assert.Null(secureHandler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public async Task CustomFormUrlEncodedContent_EncodesKeysAndValues()
    {
        using var content = new CustomFormUrlEncodedContent(new Dictionary<string, string>
        {
            ["a key"] = "value with spaces",
            ["sym&bol"] = "1+2"
        });

        var body = await content.ReadAsStringAsync();

        Assert.Equal("a+key=value+with+spaces&sym%26bol=1%2B2", body);
    }

    [Fact]
    public async Task ReadAsFileAsync_WritesContentToDisk()
    {
        using var content = new StringContent("hello world", Encoding.UTF8, "text/plain");
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDirectory);

        try
        {
            await content.ReadAsFileAsync("sample", tempDirectory, overwrite: true, fileExtension: "txt");
            var filePath = Path.Combine(tempDirectory, "sample.txt");

            Assert.True(File.Exists(filePath));
            Assert.Equal("hello world", await File.ReadAllTextAsync(filePath));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task GetStreamContentAsync_ReturnsResponseStream()
    {
        using var server = new LocalHttpServer(_ => new TestResponse(200, "stream-body"));
        using var stream = await _api.GetStreamContentAsync(new Dictionary<string, string> { ["token"] = "abc" }, server.Url);

        Assert.NotNull(stream);
        Assert.False(stream!.CanRead);
    }

    [Fact]
    public async Task SendPostRequestAsync_WithDictionary_ReturnsResponseBody()
    {
        using var server = new LocalHttpServer(_ => new TestResponse(200, "ok"));
        var payload = new Dictionary<string, string>
        {
            ["token"] = "abc",
            ["content"] = "record"
        };

        var response = await _api.SendPostRequestAsync(payload, server.Url);

        Assert.Equal("ok", response);
        Assert.True(server.Requests.TryPeek(out var request));
        Assert.Contains("token=abc", request!.Body);
        Assert.Contains("content=record", request.Body);
    }

    [Fact]
    public async Task SendPostRequest_ThrowsWhenHttpStatusIsNotSuccessful()
    {
        using var server = new LocalHttpServer(_ => new TestResponse(500, "failure"));

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _api.SendPostRequest(new Dictionary<string, string> { ["token"] = "abc" }, server.Url));
    }

    [Fact]
    public async Task ExportSurveyAccessCodeAsync_SendsExpectedPayload()
    {
        using var server = new LocalHttpServer(_ => new TestResponse(200, "access-code"));
        var api = new Redcap.RedcapApi(server.Url.ToString());

        var response = await api.ExportSurveyAccessCodeAsync("token123", "1", "survey_form", "event_1", 2);

        Assert.Equal("access-code", response);
        Assert.True(server.Requests.TryPeek(out var request));
        Assert.Contains("content=surveyAccessCode", request!.Body);
        Assert.Contains("record=1", request.Body);
        Assert.Contains("instrument=survey_form", request.Body);
        Assert.Contains("event=event_1", request.Body);
        Assert.Contains("repeat_instance=2", request.Body);
    }

    [Fact]
    public async Task DeleteRecordsAsync_IncludesDeleteLoggingFlagWhenEnabled()
    {
        using var server = new LocalHttpServer(_ => new TestResponse(200, "1"));
        var api = new Redcap.RedcapApi(server.Url.ToString());

        var response = await api.DeleteRecordsAsync("token123", new[] { "1", "2" }, 1, deleteLogging: true);

        Assert.Equal("1", response);
        Assert.True(server.Requests.TryPeek(out var request));
        Assert.Contains("content=record", request!.Body);
        Assert.Contains("action=delete", request.Body);
        Assert.Contains("records%5B0%5D=1", request.Body);
        Assert.Contains("records%5B1%5D=2", request.Body);
        Assert.Contains("delete_logging=True", request.Body);
    }

    [Fact]
    public async Task RandomizeRecord_SendsExpectedPayload()
    {
        using var server = new LocalHttpServer(_ => new TestResponse(200, "group-a"));
        var api = new Redcap.RedcapApi(server.Url.ToString());

        var response = await api.RandomizeRecord("token123", Content.Record, RedcapAction.Randomize, "1", "99", RedcapFormat.json, returnAlt: true);

        Assert.Equal("group-a", response);
        Assert.True(server.Requests.TryPeek(out var request));
        Assert.Contains("action=randomize", request!.Body);
        Assert.Contains("content=record", request.Body);
        Assert.Contains("randomization_id=99", request.Body);
        Assert.Contains("returnAlt=True", request.Body);
    }

    private sealed class SampleProperties
    {
        public string Name { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public DateTime? VisitDate { get; set; }

        public string? OptionalValue { get; set; }
    }
}
