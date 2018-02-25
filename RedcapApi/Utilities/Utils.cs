
using Newtonsoft.Json;
using Redcap.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.String;

namespace Redcap.Utilities
{
    /// <summary>
    /// Utility class for extension methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Extension method reads a stream and saves content to a local file.
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="overwrite"></param>
        /// <returns>HttpContent</returns>
        public static Task ReadAsFileAsync(this HttpContent httpContent, string fileName, string path, bool overwrite)
        {

            if (!overwrite && File.Exists(Path.Combine(fileName, path)))
            {
                throw new InvalidOperationException($"File {fileName} already exists.");
            }
            FileStream filestream = null;
            try
            {
                fileName = fileName.Replace("\"", "");
                filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
                return httpContent.CopyToAsync(filestream).ContinueWith(
                    (copyTask) =>
                    {
                        filestream.Flush();
                        filestream.Dispose();
                    }
                );
            }
            catch(Exception Ex)
            {
                Log.Error(Ex.Message);
                if(filestream != null)
                {
                    filestream.Flush();
                }
                throw new InvalidOperationException($"{Ex.Message}");
            }
        }
        /// https://stackoverflow.com/questions/8560106/isnullorempty-equivalent-for-array-c-sharp
        /// <summary>Indicates whether the specified array is null or has a length of zero.</summary>
        /// <param name="array">The array to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return (array == null || array.Length == 0);
        }
        /// <summary>
        /// This method converts string[] into string. For example, given string of "firstName, lastName, age"
        /// gets converted to "["firstName","lastName","age"]" 
        /// This is used as optional arguments for the Redcap Api
        /// </summary>
        /// <param name="redcapApi"></param>
        /// <param name="inputArray"></param>
        /// <returns>string[]</returns>
        public static async Task<string> ConvertStringArraytoString(this RedcapApi redcapApi, string[] inputArray)
        {
            try
            {
                if (inputArray.IsNullOrEmpty())
                {
                    throw new ArgumentNullException("Please provide a valid array.");
                }
                StringBuilder builder = new StringBuilder();
                foreach (string v in inputArray)
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
        /// <param name="redcapApi"></param>
        /// <param name="inputArray"></param>
        /// <returns>string</returns>
        public static async Task<string> ConvertIntArraytoString(this RedcapApi redcapApi, int[] inputArray)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                foreach (var v in inputArray)
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
        ///The method hands the return content from a request, the response.
        /// The method allows the calling method to choose a return type.
        /// </summary>
        /// <param name="redcapApi"></param>
        /// <param name="returnContent"></param>
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
        /// <param name="redcapApi"></param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <returns>tuple, string, string, string</returns>
        public static async Task<(string inputFormat, string returnFormat, string redcapDataType)> HandleFormat(this RedcapApi redcapApi, InputFormat? inputFormat = InputFormat.json, ReturnFormat? returnFormat = ReturnFormat.json, RedcapDataType? redcapDataType = RedcapDataType.flat)
        {
            // default
            var _inputFormat = InputFormat.json.ToString();
            var _returnFormat = ReturnFormat.json.ToString();
            var _redcapDataType = RedcapDataType.flat.ToString();

            try
            {

                switch (inputFormat)
                {
                    case InputFormat.json:
                        _inputFormat = InputFormat.json.ToString();
                        break;
                    case InputFormat.csv:
                        _inputFormat = InputFormat.csv.ToString();
                        break;
                    case InputFormat.xml:
                        _inputFormat = InputFormat.xml.ToString();
                        break;
                    default:
                        _inputFormat = InputFormat.json.ToString();
                        break;
                }

                switch (returnFormat)
                {
                    case ReturnFormat.json:
                        _returnFormat = ReturnFormat.json.ToString();
                        break;
                    case ReturnFormat.csv:
                        _returnFormat = ReturnFormat.csv.ToString();
                        break;
                    case ReturnFormat.xml:
                        _returnFormat = ReturnFormat.xml.ToString();
                        break;
                    default:
                        _returnFormat = ReturnFormat.json.ToString();
                        break;
                }

                switch (redcapDataType)
                {
                    case RedcapDataType.flat:
                        _returnFormat = RedcapDataType.flat.ToString();
                        break;
                    case RedcapDataType.eav:
                        _returnFormat = RedcapDataType.eav.ToString();
                        break;
                    case RedcapDataType.longitudinal:
                        _returnFormat = RedcapDataType.longitudinal.ToString();
                        break;
                    case RedcapDataType.nonlongitudinal:
                        _returnFormat = RedcapDataType.nonlongitudinal.ToString();
                        break;
                    default:
                        _returnFormat = RedcapDataType.flat.ToString();
                        break;
                }

                return await Task.FromResult((_inputFormat, _returnFormat, _redcapDataType));
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return await Task.FromResult((_inputFormat, _returnFormat, _redcapDataType));
            }
        }

        /// <summary>
        /// Method gets the overwrite behavior type and converts into string
        /// </summary>
        /// <param name="redcapApi"></param>
        /// <param name="overwriteBehavior"></param>
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
        /// <param name="redcapApi"></param>
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
        /// <param name="redcapApi"></param>
        /// <param name="events"></param>
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
        /// <param name="redcapApi"></param>
        /// <param name="fields"></param>
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
        /// <param name="redcapApi"></param>
        /// <param name="records"></param>
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
        /// <param name="redcapApi"></param>
        /// <param name="forms"></param>
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
        /// 
        /// </summary>
        /// <param name="redcapApi"></param>
        /// <param name="payload">data</param>
        /// <param name="uri">URI of the api instance</param>
        /// <returns>Stream</returns>
        public static async Task<Stream> GetStreamContentAsync(this RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri)
        {
            try
            {
                Stream stream = null;
                using (var client = new HttpClient())
                {
                    // Encode the values for payload
                    var content = new FormUrlEncodedContent(payload);
                    using (var response = await client.PostAsync(uri, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            stream = await response.Content.ReadAsStreamAsync();
                            return stream;
                        }
                    }

                }
                return null;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redcapApi"></param>
        /// <param name="payload">data</param>
        /// <param name="uri">URI of the api instance</param>
        /// <returns>string</returns>
        public static async Task<string> SendRequestAsync(this RedcapApi redcapApi, MultipartFormDataContent payload, Uri uri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.PostAsync(uri, payload))
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
        /// <param name="redcapApi"></param>
        /// <param name="payload">data</param>
        /// <param name="uri">URI of the api instance</param>
        /// <param name="isLargeDataset">Requests size > 32k chars </param>
        /// <returns></returns>
        public static async Task<string> SendRequestAsync(this RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, bool isLargeDataset = false)
        {
            try
            {
                string _responseMessage = Empty;
                using (var client = new HttpClient())
                {
                    // extract the filepath
                    var pathValue = payload.Where(x => x.Key == "filePath").FirstOrDefault().Value;
                    var pathkey = payload.Where(x => x.Key == "filePath").FirstOrDefault().Key;
                    if (!string.IsNullOrEmpty(pathkey))
                    {
                        // the actual payload does not contain a 'filePath' key
                        payload.Remove(pathkey);
                    }

                    /*
                     * Encode the values for payload
                     * Add in ability to process large data set, using StringContent
                     * Thanks to Ibrahim for pointing this out.
                     * https://stackoverflow.com/questions/23703735/how-to-set-large-string-inside-httpcontent-when-using-httpclient/23740338
                     */
                    if (isLargeDataset)
                    {
                        var serializedPayload = JsonConvert.SerializeObject(payload);
                        using (var content = new StringContent(serializedPayload, Encoding.UTF8, "application/json"))
                        {
                            using (var response = await client.PostAsync(uri, content))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    // Get the filename so we can save with the name
                                    var fileHeaders = response.Content.Headers;
                                    var fileName = fileHeaders.ContentType.Parameters.Select(x => x.Value).FirstOrDefault();
                                    var contentDisposition = response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                                    {
                                        FileName = fileName
                                    };

                                    if (!string.IsNullOrEmpty(pathValue))
                                    {
                                        // save the file to a specified location using an extension method
                                        await response.Content.ReadAsFileAsync(fileName, pathValue, true);
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
                            }

                        }
                        return _responseMessage;
                    }
                    else
                    {
                        /*
                        * Maximum character limit of 32,000 using FormUrlEncodedContent
                        */
                        using (var content = new FormUrlEncodedContent(payload))
                        {
                            using (var response = await client.PostAsync(uri, content))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    // Get the filename so we can save with the name
                                    var fileHeaders = response.Content.Headers;
                                    var fileName = fileHeaders.ContentType.Parameters.Select(x => x.Value).FirstOrDefault();
                                    var contentDisposition = response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                                    {
                                        FileName = fileName
                                    };

                                    if (!string.IsNullOrEmpty(pathValue))
                                    {
                                        // save the file to a specified location using an extension method
                                        await response.Content.ReadAsFileAsync(fileName, pathValue, true);
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
                            }
                        }

                    }
                    return _responseMessage;
                }
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
        /// <param name="redcapApi"></param>
        /// <param name="payload">data </param>
        /// <param name="uri">URI of the api instance</param>
        /// <returns>string</returns>
        public static async Task<string> SendRequest(this RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri)
        {
            string responseString;
            using (var client = new HttpClient())
            {
                // Encode the values for payload
                using (var content = new FormUrlEncodedContent(payload))
                {
                    using (var response = await client.PostAsync(uri, content))
                    {
                        // check the response and make sure its successful
                        response.EnsureSuccessStatusCode();
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return responseString;
        }

    }
}
