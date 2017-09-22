using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redcap.Interfaces;
using System.Net.Http;
using Serilog;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Redcap.Models;
using System.IO;
using System.Linq;
using Redcap.Utilities;
using System.Net.Http.Headers;

namespace Redcap
{
    /// <summary>
    /// This api interacts with redcap instances. https://project-redcap.org
    /// Go to your http://redcap_instance/api/help for Redcap Api documentations
    /// Author: Michael Tran tranpl@outlook.com, tranpl@vcu.edu
    /// </summary>
    public class RedcapApi: IRedcap
    {
        private static string _apiToken;
        private static Uri _redcapApiUri;
        /// <summary>
        /// The version of redcap that the api is currently interacting with.
        /// </summary>
        public static string Version;
        /// <summary>
        /// Constructor requires an api token and a valid uri.
        /// </summary>
        /// <param name="apiToken"></param>
        /// <param name="redcapApiUrl"></param>
        public RedcapApi(string apiToken, string  redcapApiUrl)
        {
            _apiToken = apiToken?.ToString();
            _redcapApiUri = new Uri(redcapApiUrl.ToString());
        }
        /// <summary>
        /// Method sends payload using multipart form-data
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>String</returns>
        private async Task<HttpResponseMessage> SendRequestAsync(MultipartFormDataContent payload)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _redcapApiUri;
                using (var response = await client.PostAsync(client.BaseAddress, payload))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    return response;
                }
            }
        }
        /// <summary>
        /// Method sends payload using HttpClient
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendRequestAsync(Dictionary<string, string> payload)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _redcapApiUri;
                    // extract the filepath
                    var pathValue = payload.Where(x => x.Key == "filePath").FirstOrDefault().Value;
                    var pathkey = payload.Where(x => x.Key == "filePath").FirstOrDefault().Key;
                    if (!string.IsNullOrEmpty(pathkey))
                    {
                        // the actual payload does not contain a 'filePath' key
                        payload.Remove(pathkey);
                    }
                    // Encode the values for payload
                    using (var content = new FormUrlEncodedContent(payload))
                    {
                        using (var response = await client.PostAsync(client.BaseAddress, content))
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
                                }
                            }
                            return response;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// Method sends the payload using HttpClient.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>responseString</returns>
        private async Task<string> SendRequest(Dictionary<string, string> payload)
        {
            string responseString;
            using (var client = new HttpClient())
            {
                client.BaseAddress = _redcapApiUri;
                // Encode the values for payload
                using (var content = new FormUrlEncodedContent(payload))
                {
                    using (var response = await client.PostAsync(client.BaseAddress, content))
                    {
                        // check the response and make sure its successful
                        response.EnsureSuccessStatusCode();
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return responseString;
        }
        /// <summary>
        /// This method extracts and converts an object's properties and associated values to redcap type and values.
        /// </summary>
        /// <param name="input">Object</param>
        /// <returns>Dictionary of key value pair.</returns>
        private async Task<Dictionary<string, string>> GetProperties(object input)
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
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        public delegate Task<HttpResponseMessage> GetRedcapVersion(InputFormat inputFormat, RedcapDataType redcapDataType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="inputFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms"></param>
        /// <param name="events"></param>
        /// <param name="fields"></param>
        /// <returns>string</returns>
        public delegate Task<HttpResponseMessage> ExportRecord(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="inputFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms"></param>
        /// <param name="events"></param>
        /// <param name="fields"></param>
        /// <returns>string</returns>
        public delegate Task<HttpResponseMessage> ExportRecords(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);

        /// <summary>
        /// This method converts string[] into string. For example, given string of "firstName, lastName, age"
        /// gets converted to "["firstName","lastName","age"]" 
        /// This is used as optional arguments for the Redcap Api
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns>string[]</returns>
        private async Task<string> ConvertStringArraytoString(string[] inputArray)
        {
            try {
                StringBuilder builder = new StringBuilder();
                //builder.Append('[');
                foreach (string v in inputArray)
                {

                    builder.Append(v);
                    // We do not need to append the , if less than or equal to 1 record
                    if (inputArray.Length <= 1)
                    {
                        return await Task.FromResult(builder.ToString());
                    }
                    builder.Append(",");
                }
                // We trim the comma from the string for clarity
                //builder.Append(']');
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
        /// <param name="inputArray"></param>
        /// <returns>string</returns>
        private async Task<string> ConvertIntArraytoString(int[] inputArray)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                //builder.Append('[');
                foreach (var v in inputArray)
                {

                    builder.Append(v);
                    // We do not need to append the , if less than or equal to 1 record
                    if (inputArray.Length <= 1)
                    {
                        return await Task.FromResult(builder.ToString());
                    }
                    builder.Append(",");
                }
                // We trim the comma from the string for clarity
                //builder.Append(']');
                return await Task.FromResult(builder.ToString().TrimEnd(','));

            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }
        }

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<HttpResponseMessage> GetMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "metadata" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<HttpResponseMessage> GetMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _fields = "";
                var _forms = "";
                var _response = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                
                var fieldsResult = await ExtractFieldsAsync(fields, delimiters);
                var formsResult = await ExtractFormsAsync(forms, delimiters);

                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);

                if (!String.IsNullOrEmpty(fields))
                {
                    // Convert Array List into string array
                    string[] fieldsArray = fieldsResult.ToArray();
                    // Convert string array into String
                    _fields = await ConvertStringArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    // Convert string array into String
                    _forms = await ConvertStringArraytoString(formsArray);
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "metadata" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        
        /// <summary>
        ///The method hands the return content from a request, the response.
        /// The method allows the calling method to choose a return type.
        /// </summary>
        /// <param name="returnContent"></param>
        /// <returns>string</returns>
        private async Task<string> HandleReturnContent(ReturnContent returnContent = ReturnContent.count)
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
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <returns>tuple, string, string, string</returns>
        private async Task<(string inputFormat, string returnFormat, string redcapDataType)> HandleFormat(InputFormat? inputFormat = InputFormat.json, ReturnFormat? returnFormat = ReturnFormat.json, RedcapDataType? redcapDataType = RedcapDataType.flat)
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
        /// Method extracts events into list from string
        /// </summary>
        /// <param name="events"></param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>List of string</returns>
        private async Task<List<string>> ExtractEventsAsync(string events, char[] delimiters)
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
        /// Method gets / extracts forms into list from string
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>A list of string</returns>
        private async Task<List<string>> ExtractFormsAsync(string forms, char[] delimiters)
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
        /// Method gets / extracts fields into list from string
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>List of string</returns>
        private async Task<List<string>> ExtractFieldsAsync(string fields, char[] delimiters)
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
        /// <param name="records"></param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>List of string</returns>
        private async Task<List<string>> ExtractRecordsAsync(string records, char[] delimiters)
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
        /// Method gets the overwrite behavior type and converts into string
        /// </summary>
        /// <param name="overwriteBehavior"></param>
        /// <returns>string</returns>
        private async Task<string> ExtractBehaviorAsync(OverwriteBehavior overwriteBehavior)
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
        /// This method allows you to export a set of records for a project.
        /// example: "1,2,3,4"<br/>
        /// This method allows you to export a set of records for a project.
        /// Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have "No Access" data export rights in the project, then the 
        /// API data export will fail and return an error. And if you have "De-Identified" 
        /// or "Remove all tagged Identifier fields" data export rights, then some data 
        /// fields *might* be removed and filtered out of the data set returned from the API. 
        /// To make sure that no data is unnecessarily filtered out of your API request, 
        /// you should have "Full Data Set" export rights in the project.
        /// </summary>
        /// <param name="record">string records e.g "1,2,3,4"</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<HttpResponseMessage> GetRecordAsync(string record, InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _records = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordResults = await ExtractRecordsAsync(record, delimiters);
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                if (recordResults.Count == 0)
                {
                    Log.Error($"Missing required informaion.");
                    throw new InvalidOperationException($"Missing required informaion.");
                }
                else
                {
                    // Convert Array List into string array
                    var inputRecords = recordResults.ToArray();
                    // Convert string array into String
                    _records = await ConvertStringArraytoString(inputRecords);
                    payload.Add("records", _records);
                }
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }

        /// <summary>
        /// This method allows you to export a single or set of records for a project.
        /// example: "1,2,3,4"<br/>
        /// This method allows you to export a set of records for a project.
        /// Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have "No Access" data export rights in the project, then the 
        /// API data export will fail and return an error. And if you have "De-Identified" 
        /// or "Remove all tagged Identifier fields" data export rights, then some data 
        /// fields *might* be removed and filtered out of the data set returned from the API. 
        /// To make sure that no data is unnecessarily filtered out of your API request, 
        /// you should have "Full Data Set" export rights in the project.
        /// </summary>
        /// <param name="record">string records e.g "1,2,3,4"</param>
        /// <param name="forms"></param>
        /// <param name="events"></param>
        /// <param name="fields"></param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<HttpResponseMessage> GetRecordAsync(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            HttpResponseMessage _responseMessage;
            try
            {
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordItems = await ExtractRecordsAsync(records: record, delimiters: delimiters);
                var fieldItems = await ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                // Required
                if (recordItems.Count == 0)
                {
                    Log.Error($"Missing required informaion.");
                    throw new InvalidOperationException($"Missing required informaion.");
                }
                else
                {
                    // Convert Array List into string array
                    var _inputRecords = recordItems.ToArray();
                    payload.Add("records", await ConvertStringArraytoString(_inputRecords));
                }
                // Optional
                if(fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await ConvertStringArraytoString(_fields));
                }

                // Optional
                if(formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await ConvertStringArraytoString(_forms));
                }

                // Optional
                if(eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await ConvertStringArraytoString(_events));
                }

                return await SendRequestAsync(payload);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }

        /// <summary>
        /// This method allows you to export multiple(all) records for a project.
        /// Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have "No Access" data export rights in the project, then the 
        /// API data export will fail and return an error. And if you have "De-Identified" 
        /// or "Remove all tagged Identifier fields" data export rights, then some data 
        /// fields *might* be removed and filtered out of the data set returned from the API. 
        /// To make sure that no data is unnecessarily filtered out of your API request, 
        /// you should have "Full Data Set" export rights in the project.
        /// 
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<HttpResponseMessage> GetRecordsAsync(InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType)
        {
            HttpResponseMessage _responseMessage;
            try
            {
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);
                var response = String.Empty;
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }

        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        public async Task<HttpResponseMessage> GetRedcapVersionAsync(InputFormat inputFormat, RedcapDataType redcapDataType)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, ReturnFormat.json, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "version" },
                    { "format", _inputFormat },
                    { "type", _redcapDataType }
                };
                // Execute send request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }

        /// <summary>
        /// This method allows you to import a set of records for a project
        /// </summary>
        /// <param name="data">Object that contains the records to be saved.</param>
        /// <param name="returnContent">ids - a list of all record IDs that were imported, count [default] - the number of records imported</param>
        /// <param name="overwriteBehavior">0 = normal, 1 = overwrite</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>the content specified by returnContent</returns>
        public async Task<HttpResponseMessage> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);
                if (data != null)
                {
                    List<object> dataList = new List<object>
                    {
                        data
                    };
                    var _serializedData = JsonConvert.SerializeObject(dataList);
                    var _overWriteBehavior = await ExtractBehaviorAsync(overwriteBehavior);
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", _inputFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", "MDY" },
                        { "returnFormat", _inputFormat },
                        { "returnContent", "count" },
                        { "data", _serializedData }
                    };

                    // Execute send request
                    _responseMessage = await SendRequestAsync(payload);
                    return _responseMessage;
                }
                return null;
            }
            catch (Exception Ex)
            {
                Log.Error($"Could not save records into redcap.");
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }

        }

        /// <summary>
        /// This method allows you to import a set of records for a project.
        /// </summary>
        /// <param name="data">Object that contains the records to be saved.</param>
        /// <param name="returnContent">ids - a list of all record IDs that were imported, count [default] - the number of records imported</param>
        /// <param name="overwriteBehavior">0 = normal, 1 = overwrite</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="dateFormat">MDY, DMY, YMD [default] - the format of values being imported for dates or datetime fields (understood with M representing 'month', D as 'day', and Y as 'year') - NOTE: The default format is Y-M-D (with dashes), while MDY and DMY values should always be formatted as M/D/Y or D/M/Y (with slashes), respectively.</param>
        /// <returns>the content specified by returnContent</returns>
        public async Task<HttpResponseMessage> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                HttpResponseMessage _responseMessage;
                string _dateFormat = dateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await HandleReturnContent(returnContent);
                var _overWriteBehavior = await ExtractBehaviorAsync(overwriteBehavior);

                // Extract properties from object provided
                if (data != null)
                {
                    List<object> list = new List<object>
                    {
                        data
                    };
                    var formattedData = JsonConvert.SerializeObject(list);
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", _inputFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", _dateFormat },
                        { "returnFormat", _inputFormat },
                        { "returnContent", _returnContent },
                        { "data", formattedData }
                    };

                    // Execute send request
                    _responseMessage = await SendRequestAsync(payload);
                    return _responseMessage; 
                }
                return new HttpResponseMessage { };
                
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(new HttpResponseMessage { });
            }
        }

        /// <summary>
        /// Method allows for bulk import of records into redcap project.
        /// </summary>
        /// <param name="data">List of strings that contains the records to be saved.</param>
        /// <param name="returnContent">ids - a list of all record IDs that were imported, count [default] - the number of records imported</param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="dateFormat">MDY, DMY, YMD [default] - the format of values being imported for dates or datetime fields (understood with M representing 'month', D as 'day', and Y as 'year') - NOTE: The default format is Y-M-D (with dashes), while MDY and DMY values should always be formatted as M/D/Y or D/M/Y (with slashes), respectively.</param>
        /// <returns>the content specified by returnContent</returns>
        public async Task<HttpResponseMessage> SaveRecordsAsync(List<string> data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await HandleReturnContent(returnContent);
                var _overWriteBehavior = await ExtractBehaviorAsync(overwriteBehavior);

                var _response = String.Empty;
                var _dateFormat = dateFormat;
                // Handle optional parameters
                if (string.IsNullOrEmpty((string)_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                // Extract properties from object provided
                if (data != null)
                {
                    var _serializedData = JsonConvert.SerializeObject(data);
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", _inputFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", _dateFormat },
                        { "returnFormat", _returnFormat },
                        { "returnContent", _returnContent },
                        { "data", _serializedData }
                    };

                    // Execute send request
                    _responseMessage = await SendRequestAsync(payload);
                    return _responseMessage;
                }
                return new HttpResponseMessage { };
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(new HttpResponseMessage { });
            }

        }
        /// <summary>
        /// This method allows you to import a set of records for a project.
        /// </summary>
        /// <param name="data">Object that contains the records to be saved.</param>
        /// <param name="returnContent">ids - a list of all record IDs that were imported, count [default] - the number of records imported</param>
        /// <param name="overwriteBehavior">0 = normal, 1 = overwrite</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="dateFormat">MDY, DMY, YMD [default] - the format of values being imported for dates or datetime fields (understood with M representing 'month', D as 'day', and Y as 'year') - NOTE: The default format is Y-M-D (with dashes), while MDY and DMY values should always be formatted as M/D/Y or D/M/Y (with slashes), respectively.</param>
        /// <returns>Returns the content with format specified.</returns>
        public async Task<HttpResponseMessage> ImportRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                HttpResponseMessage _responseMessage;
                string _dateFormat = dateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await HandleReturnContent(returnContent);
                var _overWriteBehavior = await ExtractBehaviorAsync(overwriteBehavior);

                // Extract properties from object provided
                if (data != null)
                {
                    List<object> list = new List<object>
                    {
                        data
                    };
                    var formattedData = JsonConvert.SerializeObject(list);
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", _inputFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", _dateFormat },
                        { "returnFormat", _inputFormat },
                        { "returnContent", _returnContent },
                        { "data", formattedData }
                    };

                    // Execute send request
                    _responseMessage = await SendRequestAsync(payload);
                    return _responseMessage;
                }
                return new HttpResponseMessage { };
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(new HttpResponseMessage { });
            }
        }

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<HttpResponseMessage> ExportMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "metadata" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }

        }

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat"></param>
        /// <param name="delimiters"></param>
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<HttpResponseMessage> ExportMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _fields = "";
                var _forms = "";
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }

                var fieldsResult = await ExtractFieldsAsync(fields, delimiters);
                var formsResult = await ExtractFormsAsync(forms, delimiters);

                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);

                if (!String.IsNullOrEmpty(fields))
                {
                    // Convert Array List into string array
                    string[] fieldsArray = fieldsResult.ToArray();
                    // Convert string array into String
                    _fields = await ConvertStringArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    // Convert string array into String
                    _forms = await ConvertStringArraytoString(formsArray);
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "metadata" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        
        /// <summary>
        /// Method allows you to export events from redcap project.
        /// </summary>
        /// <param name="inputFormat">csv, json [default], xml </param>
        /// <param name="returnFormat"></param>
        /// <param name="arms">an array of arm numbers that you wish to pull events for (by default, all events are pulled)</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExportEventsAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json, int[] arms = null)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _arms = "";
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                if (arms.Length > 0)
                {
                    // Convert string array into String
                    _arms = await ConvertIntArraytoString(arms);
                }
                var payload = new Dictionary<string, string>
                {
                    {"arms", _arms },
                    { "token", _apiToken },
                    { "content", "event" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        
        /// <summary>
        /// Method allows you to export all events from redcap project.
        /// </summary>
        /// <param name="inputFormat">csv, json [default], xml </param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Events for the project in the format specified</returns>
        public async Task<HttpResponseMessage> ExportEventsAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "event" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportEvents(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteEvents(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportFields(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteFile(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportInstruments(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> CreateProject(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportProjectXml(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> GenerateNextRecordName(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method allows you to export a set of records for a project
        /// </summary>
        /// <param name="record">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExportRecordAsync(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                string _response;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordItems = await ExtractRecordsAsync(records: record, delimiters: delimiters);
                var fieldItems = await ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                // Required
                if (recordItems.Count == 0)
                {
                    Log.Error($"Missing required informaion.");
                    throw new InvalidOperationException($"Missing required informaion.");
                }
                else
                {
                    // Convert Array List into string array
                    var _inputRecords = recordItems.ToArray();
                    payload.Add("records", await ConvertStringArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await ConvertStringArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await ConvertStringArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await ConvertStringArraytoString(_events));
                }

                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        
        /// <summary>
        /// This method allows you to export a set of records for a project
        /// </summary>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExportRecordsAsync(string records, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordItems = await ExtractRecordsAsync(records: records, delimiters: delimiters);
                var fieldItems = await ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                // Required
                if (recordItems.Count == 0)
                {
                    Log.Error($"Missing required informaion.");
                    throw new InvalidOperationException($"Missing required informaion.");
                }
                else
                {
                    // Convert Array List into string array
                    var _inputRecords = recordItems.ToArray();
                    payload.Add("records", await ConvertStringArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await ConvertStringArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await ConvertStringArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await ConvertStringArraytoString(_events));
                }

                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// Method allows you to export multiple(all) records out of the redcap project.
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExportRecordsAsync(InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var fieldItems = await ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await ConvertStringArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await ConvertStringArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await ConvertStringArraytoString(_events));
                }

                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportRecords(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteRecords(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        public async Task<HttpResponseMessage> ExportRedcapVersionAsync(InputFormat inputFormat, RedcapDataType redcapDataType)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, ReturnFormat.json, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "version" },
                    { "format", _inputFormat },
                    { "type", _redcapDataType }
                };
                // Execute send request
                _responseMessage = await SendRequestAsync(payload);

                return await Task.FromResult(_responseMessage);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyLink(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyParticipants(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyQueueLink(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyReturnCode(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method exports redcap users for a specific project.
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>The method will return all the attributes below with regard to user privileges in the format specified. Please note that the 'forms' attribute is the only attribute that contains sub-elements (one for each data collection instrument), in which each form will have its own Form Rights value (see the key below to learn what each numerical value represents). Most user privilege attributes are boolean (0=No Access, 1=Access). Attributes returned:</returns>
        /// <example>
        /// username, email, firstname, lastname, expiration, data_access_group, design, user_rights, data_access_groups, data_export, reports, stats_and_charts, manage_survey_participants, calendar, data_import_tool, data_comparison_tool, logging, file_repository, data_quality_create, data_quality_execute, api_export, api_import, mobile_app, mobile_app_download_data, record_create, record_rename, record_delete, lock_records_customization, lock_records, lock_records_all_forms, forms
        /// KEY:Data Export: 0=No Access, 2=De-Identified, 1=Full Data Set
        /// Form Rights: 0=No Access, 2=Read Only, 1=View records/responses and edit records(survey responses are read-only), 3=Edit survey responses
        /// Other attribute values: 0=No Access, 1=Access.
        /// </example>
        public async Task<HttpResponseMessage> ExportUsersAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _inputFormat = inputFormat.ToString();
                var _returnFormat = returnFormat.ToString();
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "user" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };

                // Execute send request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }

        }
        
        /// <summary>
        /// Not Implimented
        /// </summary>
        /// <param name="arms"></param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportUsers(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method allows you to export multiple(all) records for a project.
        /// Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have "No Access" data export rights in the project, then the 
        /// API data export will fail and return an error. And if you have "De-Identified" 
        /// or "Remove all tagged Identifier fields" data export rights, then some data 
        /// fields *might* be removed and filtered out of the data set returned from the API. 
        /// To make sure that no data is unnecessarily filtered out of your API request, 
        /// you should have "Full Data Set" export rights in the project.
        /// 
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<HttpResponseMessage> GetRecordsAsync(InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// This method allows you to export the Arms for a project.
        /// NOTE: This only works for longitudinal projects. E.g. Arms are only available in longitudinal projects.
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default]</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Arms for the project in the format specified</returns>
        public async Task<HttpResponseMessage> ExportArmsAsync(InputFormat inputFormat, ReturnFormat returnFormat)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);

                var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "arm" },
                        { "format", _inputFormat },
                        { "returnFormat", _returnFormat },
                        { "arms", null}
                    };
                // Execute send request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }

        private async Task<List<string>> ExtractArmsAsync<T>(string arms, char[] delimiters)
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
        /// This method allows you to import Arms into a project or to rename existing Arms in a project. 
        /// You may use the parameter override=1 as a 'delete all + import' action in order to erase all existing Arms in the project while importing new Arms. 
        /// Notice: Because of the 'override' parameter's destructive nature, this method may only use override=1 for projects in Development status.
        /// NOTE: This only works for longitudinal projects. 
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="overRide"></param>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ImportArmsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                var _override = overRide.ToString();
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "arm" },
                        { "action", "import" },
                        { "format", _inputFormat },
                        { "type", _redcapDataType },
                        { "override", _override },
                        { "returnFormat", _returnFormat },
                        { "data", _serializedData }
                    };
                // Execute request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// This method allows you to delete Arms from a project. Notice: Because of this method's destructive nature, it is only available for use for projects in Development status. Additionally, please be aware that deleting an arm also automatically deletes all events that belong to that arm, and will also automatically delete any records/data that have been collected under that arm (this is non-reversible data loss).
        /// NOTE: This only works for longitudinal projects. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteArmsAsync<T>(T data)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "arm" },
                    { "action", "delete" },
                    { "arms", _serializedData }
                };
                // Execute request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }

        }

        /// <summary>
        /// This method allows you to import multile events into the specific project.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Contains the required attributes 'event_name' (referring to the name/label of the event) and 'arm_num' (referring to the arm number to which the event belongs - assumes '1' if project only contains one arm). In order to modify an existing event, you must provide the attribute 'unique_event_name' (referring to the auto-generated unique event name of the given event). If the project utilizes the Scheduling module, the you may optionally provide the following attributes, which must be numerical: day_offset, offset_min, offset_max. If the day_offset is not provided, then the events will be auto-numbered in the order in which they are provided in the API request. </param>
        /// <param name="overRide">0 - false [default], 1 - true — You may use override=1 as a 'delete all + import' action in order to erase all existing Events in the project while importing new Events. If override=0, then you can only add new Events or modify existing ones. </param>
        /// <param name="inputFormat">csv, json, xml [default]</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Number of Events imported</returns>
        public async Task<HttpResponseMessage> ImportEventsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await HandleFormat(inputFormat, returnFormat);
                var _override = overRide.ToString();
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "event" },
                    { "action", "import" },
                    { "format", _inputFormat },
                    { "type", _redcapDataType },
                    { "override", _override },
                    { "returnFormat", _returnFormat },
                    { "data", _serializedData }
                };
                // Execute request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return new HttpResponseMessage { };
            }

        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteEvents()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportFields()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method allows you to download a document that has been attached to an individual record for a File Upload field. Please note that this method may also be used for Signature fields (i.e. File Upload fields with 'signature' validation type).
        /// </summary>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param> 
        /// <param name="filePath">The directory where you want the download files to be saved.</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <example>
        /// The MIME type of the file, along with the name of the file and its extension, can be found in the header of the returned response. Thus in order to determine these attributes of the file being exported, you will need to parse the response header. Example: content-type = application/vnd.openxmlformats-officedocument.wordprocessingml.document; name='FILE_NAME.docx'
        /// </example>
        /// <returns>the contents of the file</returns>
        public async Task<HttpResponseMessage> ExportFileAsync(string record, string field, string eventName, string repeatInstance, string filePath = null, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _filePath = filePath;
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!Directory.Exists(_filePath))
                    {
                        Log.Information($"The directory does not exist!");
                        Directory.CreateDirectory(_filePath);
                    }
                }
                var _returnFormat = returnFormat.ToString();
                var _eventName = eventName;
                var _repeatInstance = repeatInstance;
                var _record = record;
                var _field = field;
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "file" },
                    { "action", "export" },
                    { "record", _record },
                    { "field", _field },
                    { "event", _eventName },
                    { "returnFormat", _returnFormat },
                    { "filePath", $@"{_filePath}" }
                };
                if (!string.IsNullOrEmpty(_repeatInstance))
                {
                    payload.Add("repeat_instance", _repeatInstance);
                }
                // Execute request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch(Exception Ex)
            {
                Log.Error(Ex.Message);
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        ///  This method allows you to upload a document that will be attached to an individual record for a File Upload field. Please note that this method may NOT be used for Signature fields (i.e. File Upload fields with 'signature' validation type) because a signature can only be captured and stored using the web interface. 
        /// </summary>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param> 
        /// <param name="fileName">The File you be imported, contents of the file</param>
        /// <param name="filePath">the path where the file is located</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</returns>
        public async Task<HttpResponseMessage> ImportFileAsync(string record, string field, string eventName, string repeatInstance, string fileName, string filePath, ReturnFormat returnFormat = ReturnFormat.json)
        {
            
            try
            {
                HttpResponseMessage _responseMessage;
                var _fileName = fileName;
                var _filePath = filePath;
                var _binaryFile = Path.Combine(_filePath, _fileName);
                ByteArrayContent _fileContent;
                var _returnFormat = returnFormat.ToString();
                var _eventName = eventName;
                var _repeatInstance = repeatInstance;
                var _record = record;
                var _field = field;
                var payload = new MultipartFormDataContent()
                {
                        {new StringContent(_apiToken), "token" },
                        {new StringContent("file") ,"content" },
                        {new StringContent("import"), "action" },
                        {new StringContent(_record), "record" },
                        {new StringContent(_field), "field" },
                        {new StringContent(_eventName),  "event" },
                        {new StringContent(_returnFormat), "returnFormat" }
                };
                if (!string.IsNullOrEmpty(_repeatInstance))
                {
                    // add repeat instrument params if available
                    payload.Add(new StringContent(_repeatInstance), "repeat_instance");
                }
                if (string.IsNullOrEmpty(_fileName) || string.IsNullOrEmpty(_filePath))
                {

                    throw new InvalidOperationException($"file can not be empty or null");
                }
                else
                {
                    // add the binary file in specific content type
                    _fileContent = new ByteArrayContent(File.ReadAllBytes(_binaryFile));
                    _fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    payload.Add(_fileContent, "file", _fileName);
                }
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// This method allows you to remove a document that has been attached to an individual record for a File Upload field. Please note that this method may also be used for Signature fields (i.e. File Upload fields with 'signature' validation type).
        /// </summary>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>String</returns>
        public async Task<HttpResponseMessage> DeleteFileAsync(string record, string field, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                HttpResponseMessage _responseMessage;
                var _returnFormat = returnFormat.ToString();
                var _eventName = eventName;
                var _repeatInstance = repeatInstance;
                var _record = record;
                var _field = field;
                var payload = new MultipartFormDataContent()
                {
                        {new StringContent(_apiToken), "token" },
                        {new StringContent("file") ,"content" },
                        {new StringContent("delete"), "action" },
                        {new StringContent(_record), "record" },
                        {new StringContent(_field), "field" },
                        {new StringContent(_eventName),  "event" },
                        {new StringContent(_returnFormat), "returnFormat" }
                };
                if (!string.IsNullOrEmpty(_repeatInstance))
                {
                    // add repeat instrument params if available
                    payload.Add(new StringContent(_repeatInstance), "repeat_instance");
                }
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return new HttpResponseMessage { };
            }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportInstruments()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportPdfInstrument()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportPdfInstrument()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> CreateProject()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportProjectInfo()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportProjectInfo()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportProjectXml()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> GenerateNextRecordName()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ImportRecords()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteRecords()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportRedcapVersion()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyLink()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyParticipants()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyQueueLink()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExportSurveyReturnCode()
        {
            throw new NotImplementedException();
        }
    }
}
