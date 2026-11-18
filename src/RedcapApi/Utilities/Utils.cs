using Redcap.Http;
using Redcap.Models;

using Serilog;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

using static System.String;

namespace Redcap.Utilities
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// Method gets the display string for an enum
        /// </summary>
        /// <param name="enumString">The enumeration value carrying the display attribute.</param>
        /// <returns>The display name associated with the enumeration value.</returns>
        public static string GetDisplayName(this Enum enumString)
        {
            var returnvalue = enumString
                .GetType()
                .GetMember(enumString.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName();

            return returnvalue;
        }

        /// <summary>
        /// Extension method reads a stream and saves content to a local file.
        /// </summary>
        /// <param name="httpContent">The HTTP response content to save.</param>
        /// <param name="fileName">The file name within the selected directory.</param>
        /// <param name="path">The destination directory.</param>
        /// <param name="overwrite">Whether an existing file can be replaced.</param>
        /// <param name="fileExtension">The extension to append, or an empty string to keep the supplied file name.</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <returns>A task that completes after the file is written.</returns>
        public static async Task ReadAsFileAsync(this HttpContent httpContent, string fileName, string path, bool overwrite, string fileExtension = "pdf", CancellationToken cancellationToken = default)
        {
            fileName = fileName?.Trim('"');
            if(string.IsNullOrWhiteSpace(fileName) || fileName is "." or ".." ||
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                fileName.Contains('/') || fileName.Contains('\\') || fileName.Contains(':'))
            {
                throw new ArgumentException("Provide a file name without a directory.", nameof(fileName));
            }

            var outputName = string.IsNullOrEmpty(fileExtension) ? fileName : $"{fileName}.{fileExtension}";
            if(outputName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                outputName.Contains('/') || outputName.Contains('\\') || outputName.Contains(':'))
            {
                throw new ArgumentException("Provide a file extension without a directory.", nameof(fileExtension));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var temporaryPath = Path.Combine(path, Path.GetRandomFileName());
            var options = new FileStreamOptions { Mode = FileMode.CreateNew, Access = FileAccess.Write, Share = FileShare.None };
            if(!OperatingSystem.IsWindows())
            {
                options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            }
            var destination = new FileStream(temporaryPath, options);
            try
            {
                await using(destination)
                {
                    await httpContent.CopyToAsync(destination, cancellationToken);
                }
                cancellationToken.ThrowIfCancellationRequested();
                File.Move(temporaryPath, Path.Combine(path, outputName), overwrite);
            }
            finally
            {
                File.Delete(temporaryPath);
            }
        }
        /// https://stackoverflow.com/questions/8560106/isnullorempty-equivalent-for-array-c-sharp
        /// <summary>Indicates whether the specified array is null or has a length of zero.</summary>
        /// <param name="array">The array to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        /// <typeparam name="T">The array element type.</typeparam>
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return (array == null || array.Length == 0);
        }
        /// <summary>
        /// This method converts string[] into string. For example, given string of "firstName, lastName, age"
        /// gets converted to "["firstName","lastName","age"]" 
        /// This is used as optional arguments for the Redcap Api
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="inputArray">The array of values to process.</param>
        /// <returns>string[]</returns>
        /// <typeparam name="T">The array element type.</typeparam>
        public static async Task<string> ConvertArraytoString<T>(this RedcapApi redcapApi, T[] inputArray)
        {
            try
            {
                if (inputArray.IsNullOrEmpty())
                {
                    throw new ArgumentNullException("Please provide a valid array.");
                }
                StringBuilder builder = new StringBuilder();
                foreach (T v in inputArray)
                {

                    builder.Append(v);
                    // We do not need to append the , if less than or equal to a single string
                    if (inputArray.Length <= 1)
                    {
                        return await Task.FromResult(builder.ToString());
                    }
                    builder.Append(",");
                }
                // We trim the comma from the string for clarity
                return await Task.FromResult(builder.ToString().TrimEnd(','));

            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }
        }
        /// <summary>
        /// This method converts int[] into a string. For example, given int[] of "[1,2,3]"
        /// gets converted to "["1","2","3"]" 
        /// This is used as optional arguments for the Redcap Api
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="inputArray">The array of values to process.</param>
        /// <returns>string</returns>
        public static async Task<string> ConvertIntArraytoString(this RedcapApi redcapApi, int[] inputArray)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                foreach (var intValue in inputArray)
                {

                    builder.Append(intValue);
                    // We do not need to append the , if less than or equal to a single string
                    if (inputArray.Length <= 1)
                    {
                        return await Task.FromResult(builder.ToString());
                    }
                    builder.Append(",");
                }
                // We trim the comma from the string for clarity
                return await Task.FromResult(builder.ToString().TrimEnd(','));

            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }
        }
        /// <summary>
        ///The method hands the return content from a request, the response.
        /// The method allows the calling method to choose a return type.
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="returnContent">The response content requested from REDCap after import.</param>
        /// <returns>string</returns>
        public static async Task<string> HandleReturnContent(this RedcapApi redcapApi, ReturnContent returnContent = ReturnContent.count)
        {
            try
            {
                var _returnContent = returnContent.ToString();
                switch (returnContent)
                {
                    case ReturnContent.ids:
                        _returnContent = ReturnContent.ids.ToString();
                        break;
                    case ReturnContent.count:
                        _returnContent = ReturnContent.count.ToString();
                        break;
                    default:
                        _returnContent = ReturnContent.count.ToString();
                        break;
                }
                return await Task.FromResult(_returnContent);
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return await Task.FromResult(String.Empty);
            }
        }
        /// <summary>
        /// Tuple that returns both inputFormat and redcap returnFormat
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="format">csv, json[default], xml , odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="onErrorFormat">The requested format for REDCap errors.</param>
        /// <param name="redcapDataType">The record layout used in the request.</param>
        /// <returns>tuple, string, string, string</returns>
        public static async Task<(string format, string onErrorFormat, string redcapDataType)> HandleFormat(this RedcapApi redcapApi, RedcapFormat? format = RedcapFormat.json, RedcapReturnFormat? onErrorFormat = RedcapReturnFormat.json, RedcapDataType? redcapDataType = RedcapDataType.flat)
        {
            // default
            var _format = RedcapFormat.json.ToString();
            var _onErrorFormat = RedcapReturnFormat.json.ToString();
            var _redcapDataType = RedcapDataType.flat.ToString();

            try
            {

                switch (format)
                {
                    case RedcapFormat.json:
                        _format = RedcapFormat.json.ToString();
                        break;
                    case RedcapFormat.csv:
                        _format = RedcapFormat.csv.ToString();
                        break;
                    case RedcapFormat.xml:
                        _format = RedcapFormat.xml.ToString();
                        break;
                    case RedcapFormat.odm:
                        _format = RedcapFormat.odm.ToString();
                        break;
                    default:
                        _format = RedcapFormat.json.ToString();
                        break;
                }

                switch (onErrorFormat)
                {
                    case RedcapReturnFormat.json:
                        _onErrorFormat = RedcapReturnFormat.json.ToString();
                        break;
                    case RedcapReturnFormat.csv:
                        _onErrorFormat = RedcapReturnFormat.csv.ToString();
                        break;
                    case RedcapReturnFormat.xml:
                        _onErrorFormat = RedcapReturnFormat.xml.ToString();
                        break;
                    default:
                        _onErrorFormat = RedcapReturnFormat.json.ToString();
                        break;
                }

                switch (redcapDataType)
                {
                    case RedcapDataType.flat:
                        _redcapDataType = RedcapDataType.flat.ToString();
                        break;
                    case RedcapDataType.eav:
                        _redcapDataType = RedcapDataType.eav.ToString();
                        break;
                    case RedcapDataType.longitudinal:
                        _redcapDataType = RedcapDataType.longitudinal.ToString();
                        break;
                    case RedcapDataType.nonlongitudinal:
                        _redcapDataType = RedcapDataType.nonlongitudinal.ToString();
                        break;
                    default:
                        _redcapDataType = RedcapDataType.flat.ToString();
                        break;
                }

                return await Task.FromResult((_format, _onErrorFormat, _redcapDataType));
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return await Task.FromResult((_format, _onErrorFormat, _redcapDataType));
            }
        }

        /// <summary>
        /// Method gets the overwrite behavior type and converts into string
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="overwriteBehavior">Controls whether blank imported values overwrite existing values.</param>
        /// <returns>string</returns>
        public static async Task<string> ExtractBehaviorAsync(this RedcapApi redcapApi, OverwriteBehavior overwriteBehavior)
        {
            try
            {
                var _overwriteBehavior = OverwriteBehavior.normal.ToString();
                switch (overwriteBehavior)
                {
                    case OverwriteBehavior.overwrite:
                        _overwriteBehavior = OverwriteBehavior.overwrite.ToString();
                        break;
                    case OverwriteBehavior.normal:
                        _overwriteBehavior = OverwriteBehavior.normal.ToString();
                        break;
                    default:
                        _overwriteBehavior = OverwriteBehavior.overwrite.ToString();
                        break;
                }
                return await Task.FromResult(_overwriteBehavior);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }
        }
        /// <summary>
        /// This method extracts and converts an object's properties and associated values to redcap type and values.
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="input">Object</param>
        /// <returns>Dictionary of key value pair.</returns>
        public static async Task<Dictionary<string, string>> GetProperties(this RedcapApi redcapApi, object input)
        {
            try
            {
                if (input != null)
                {
                    // Get the type
                    var type = input.GetType();
                    var obj = new Dictionary<string, string>();
                    // Get the properties
                    var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] properties = input.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        // get type of column
                        // The the type of the property
                        Type columnType = prop.PropertyType;

                        // We need to set lower case for REDCap's variable nameing convention (lower casing)
                        string propName = prop.Name.ToLower();
                        // We check for null values
                        var propValue = type.GetProperty(prop.Name).GetValue(input, null)?.ToString();
                        if (propValue != null)
                        {
                            var t = columnType.GetGenericArguments();
                            if (t.Length > 0)
                            {
                                if (columnType.GenericTypeArguments[0].FullName == "System.DateTime")
                                {
                                    var dt = DateTime.Parse(propValue);
                                    propValue = dt.ToString();
                                }
                                if (columnType.GenericTypeArguments[0].FullName == "System.Boolean")
                                {
                                    if (propValue == "True")
                                    {
                                        propValue = "1";
                                    }
                                    else
                                    {
                                        propValue = "0";
                                    }
                                }
                            }
                            obj.Add(propName, propValue);
                        }
                        else
                        {
                            // We have to make sure we handle for null values.
                            obj.Add(propName, null);
                        }
                    }
                    return await Task.FromResult(obj);
                }
                return await Task.FromResult(new Dictionary<string, string> { });
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(new Dictionary<string, string> { });
            }
        }
        /// <summary>
        /// Method extracts events into list from string
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="events">The event identifiers to process.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>List of string</returns>
        public static async Task<List<string>> ExtractEventsAsync(this RedcapApi redcapApi, string events, char[] delimiters)
        {
            if (!String.IsNullOrEmpty(events))
            {
                try
                {
                    var formItems = events.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    List<string> eventsResult = new List<string>();
                    foreach (var form in formItems)
                    {
                        eventsResult.Add(form);
                    }
                    return await Task.FromResult(eventsResult);
                }
                catch (Exception Ex)
                {
                    Log.Error($"{Ex.Message}");
                    return await Task.FromResult(new List<string> { });
                }
            }
            return await Task.FromResult(new List<string> { });
        }
        /// <summary>
        /// Method gets / extracts fields into list from string
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="fields">The field names to process.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>List of string</returns>
        public static async Task<List<string>> ExtractFieldsAsync(this RedcapApi redcapApi, string fields, char[] delimiters)
        {
            if (!String.IsNullOrEmpty(fields))
            {
                try
                {
                    var fieldItems = fields.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    List<string> fieldsResult = new List<string>();
                    foreach (var field in fieldItems)
                    {
                        fieldsResult.Add(field);
                    }
                    return await Task.FromResult(fieldsResult);
                }
                catch (Exception Ex)
                {
                    Log.Error($"{Ex.Message}");
                    return await Task.FromResult(new List<string> { });
                }
            }
            return await Task.FromResult(new List<string> { });
        }
        /// <summary>
        /// Method gets / extract records into list from string
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="records">The record identifiers to process.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>List of string</returns>
        public static async Task<List<string>> ExtractRecordsAsync(this RedcapApi redcapApi, string records, char[] delimiters)
        {
            if (!String.IsNullOrEmpty(records))
            {
                try
                {
                    var recordItems = records.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    List<string> recordResults = new List<string>();
                    foreach (var record in recordItems)
                    {
                        recordResults.Add(record);
                    }
                    return await Task.FromResult(recordResults);
                }
                catch (Exception Ex)
                {
                    Log.Error($"{Ex.Message}");
                    return await Task.FromResult(new List<string> { });
                }
            }
            return await Task.FromResult(new List<string> { });
        }
        /// <summary>
        /// Method gets / extracts forms into list from string
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="forms">The instrument names to process.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>A list of string</returns>
        public static async Task<List<string>> ExtractFormsAsync(this RedcapApi redcapApi, string forms, char[] delimiters)
        {
            if (!String.IsNullOrEmpty(forms))
            {
                try
                {
                    var formItems = forms.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    List<string> formsResult = new List<string>();
                    foreach (var form in formItems)
                    {
                        formsResult.Add(form);
                    }
                    return await Task.FromResult(formsResult);
                }
                catch (Exception Ex)
                {
                    Log.Error($"{Ex.Message}");
                    return await Task.FromResult(new List<string> { });
                }
            }
            return await Task.FromResult(new List<string> { });
        }
        /// <summary>
        /// Creates a handler with standard certificate validation and automatic redirects disabled.
        /// </summary>
        /// <returns>A new HTTP handler with automatic redirects disabled.</returns>
        public static HttpClientHandler GetHttpHandler() => new HttpClientHandler { AllowAutoRedirect = false };

        /// <summary>Creates an HTTP handler using this client's certificate settings.</summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <returns>A new HTTP handler with automatic redirects disabled.</returns>
        public static HttpClientHandler GetHttpHandler(this RedcapApi redcapApi)
        {
            var handler = GetHttpHandler();
            if(redcapApi.UseInsecureCertificates)
            {
                handler.UseProxy = false;
                handler.ServerCertificateCustomValidationCallback = BrokenCertificate.DangerousAcceptAnyServerCertificateValidator;
            }
            return handler;
        }

        /// <summary>Validates the endpoint scheme and creates a handler with the client's certificate policy.</summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="uri">The URL of the REDCap endpoint.</param>
        /// <returns>A handler for the validated endpoint.</returns>
        private static HttpClientHandler CreateRequestHandler(RedcapApi redcapApi, Uri uri)
        {
            if(uri.Scheme != Uri.UriSchemeHttps && !(uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback))
            {
                throw new ArgumentException("Use an HTTPS endpoint for remote REDCap requests.", nameof(uri));
            }
            return redcapApi.GetHttpHandler();
        }
        /// <summary>Posts the form fields and returns the downloaded response as a stream.</summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">data</param>
        /// <param name="uri">URI of the api instance</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">The HTTP request timeout in seconds.</param>
        /// <returns>Stream</returns>
        public static async Task<Stream> GetStreamContentAsync(this RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            using var handler = CreateRequestHandler(redcapApi, uri);
            using var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(timeOutSeconds)
            };
            using var content = new FormUrlEncodedContent(payload);
            using var response = await client.PostAsync(uri, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return new MemoryStream(bytes, writable: false);
        }

        /// <summary>
        /// Method to send http request using MultipartFormDataContent
        /// Requests with attachments
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">data</param>
        /// <param name="uri">URI of the api instance</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">100</param>
        /// <returns>string</returns>
        public static async Task<string> SendPostRequestAsync(this RedcapApi redcapApi, MultipartFormDataContent payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            try
            {
                using (var handler = CreateRequestHandler(redcapApi, uri))
                using (var client = new HttpClient(handler))
                {
                    TimeSpan tsTimeOut = TimeSpan.FromTicks(timeOutSeconds * TimeSpan.TicksPerSecond);
                    client.Timeout = tsTimeOut;

                    using (var response = await client.PostAsync(uri, payload, cancellationToken))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                return Empty;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return Empty;
            }
        }

        /// <summary>
        /// Sends request using http
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">data</param>
        /// <param name="uri">URI of the api instance</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">100</param>
        /// <returns>The REDCap response body as text.</returns>
        public static async Task<string> SendPostRequestAsync(this RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            try
            {
                string _responseMessage;

                using var handler = CreateRequestHandler(redcapApi, uri);
                using var client = new HttpClient(handler);

                TimeSpan tsTimeOut = TimeSpan.FromTicks(timeOutSeconds * TimeSpan.TicksPerSecond);
                client.Timeout = tsTimeOut;

                // extract the filepath
                var pathValue = payload.FirstOrDefault(x => x.Key == "filePath").Value;
                var pathkey = payload.FirstOrDefault(x => x.Key == "filePath").Key;

                if (!string.IsNullOrEmpty(pathkey))
                {
                    // the actual payload does not contain a 'filePath' key
                    payload.Remove(pathkey);
                }

                using var content = new CustomFormUrlEncodedContent(payload);
                using var response = await client.PostAsync(uri, content, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(pathValue))
                    {
                        var headers = response.Content.Headers;
                        var fileName = headers.ContentDisposition?.FileNameStar ??
                            headers.ContentDisposition?.FileName ??
                            headers.ContentType?.Parameters.FirstOrDefault(parameter =>
                                string.Equals(parameter.Name, "name", StringComparison.OrdinalIgnoreCase))?.Value;
                        fileName = fileName?.Trim('"');

                        var isPdf = payload.TryGetValue("content", out var contentType) && contentType == "pdf";
                        if (string.IsNullOrEmpty(fileName) && isPdf)
                        {
                            fileName = payload.TryGetValue("instrument", out var instrument) ? instrument + ".pdf" : "instruments.pdf";
                        }

                        await response.Content.ReadAsFileAsync(fileName, pathValue, overwrite: true,
                            fileExtension: null, cancellationToken: cancellationToken);
                        _responseMessage = fileName;
                    }
                    else
                    {
                        _responseMessage = await response.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    _responseMessage = await response.Content.ReadAsStringAsync();
                }

                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return Empty;
            }
        }
        /// <summary>
        /// Sends http request to api
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">data </param>
        /// <param name="uri">URI of the api instance</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">100</param>
        /// <returns>string</returns>
        public static async Task<string> SendPostRequest(this RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            using var handler = CreateRequestHandler(redcapApi, uri);
            using var client = new HttpClient(handler);

            TimeSpan tsTimeOut = TimeSpan.FromTicks(timeOutSeconds * TimeSpan.TicksPerSecond);
            client.Timeout = tsTimeOut;

            // Encode the values for payload
            using var content = new FormUrlEncodedContent(payload);
            using var response = await client.PostAsync(uri, content, cancellationToken);
            // check the response and make sure its successful
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
        /// <summary>
        /// Method obtains list of string from comma seperated strings
        /// </summary>
        /// <typeparam name="T">Reserved for compatibility with the original signature.</typeparam>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="arms">The delimited arm numbers.</param>
        /// <param name="delimiters">The characters that separate items in the input.</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <returns>List of string</returns>
        public static async Task<List<string>> ExtractArmsAsync<T>(this RedcapApi redcapApi, string arms, char[] delimiters, CancellationToken cancellationToken = default)
        {
            if (!String.IsNullOrEmpty(arms))
            {
                try
                {
                    var _arms = arms.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    List<string> armsResult = new List<string>();
                    foreach (var arm in _arms)
                    {
                        armsResult.Add(arm);
                    }
                    return await Task.FromResult(armsResult);
                }
                catch (Exception Ex)
                {
                    Log.Error($"{Ex.Message}");
                    return await Task.FromResult(new List<string> { });
                }
            }
            return await Task.FromResult(new List<string> { });

        }
        /// <summary>
        /// Checks if the string passed is null or empty.
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="token">The project API token for this operation.</param>
        public static void CheckToken(this RedcapApi redcapApi, string token)
        {
            /*
             * Check the required parameters for empty or null
             */
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("Please provide a valid Redcap token.");
            }
        }

    }
}
