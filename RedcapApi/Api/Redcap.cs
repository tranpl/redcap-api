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
        public static string Version;
        public RedcapApi(string apiToken, string  redcapApiUrl)
        {
            _apiToken = apiToken?.ToString();
            _redcapApiUri = new Uri(redcapApiUrl.ToString());
        }
        public delegate Task<string> GetRedcapVersion(RedcapFormat format, RedcapDataType type);
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
                ///builder.Append('[');
                foreach (string v in inputArray)
                {

                    builder.Append(v);
                    /// We do not need to append the , if less than or equal to 1 record
                    if (inputArray.Length <= 1)
                    {
                        return await Task.FromResult(builder.ToString());
                    }
                    builder.Append(",");
                }
                /// We trim the comma from the string for clarity
                ///builder.Append(']');
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
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public Task<string> ExportMetaData(RedcapFormat? redcapFormat, ReturnFormat? returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="redcapFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat"></param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<string> GetMetaDataAsync(RedcapFormat? redcapFormat, ReturnFormat? returnFormat)
        {
            try
            {
                var response = String.Empty;
                // Handle optional parameters
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "metadata" },
                    { "format", _redcapFormat },
                    { "returnFormat", _returnFormat }
                };
                response = await SendRequest(payload);
                return await Task.FromResult(response);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }
        }
        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="redcapFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat"></param>
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<string> GetMetaDataAsync(RedcapFormat? redcapFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
        {
            try
            {
                var _fieldsParam = "";
                var _formsParam = "";
                var _response = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                
                var fieldsResult = await ExtractFields(fields, delimiters);
                var formsResult = await ExtractForms(forms, delimiters);

                // Handle optional parameters
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat);

                if (!String.IsNullOrEmpty(fields))
                {
                    /// Convert Array List into string array
                    string[] fieldsArray = fieldsResult.ToArray();
                    /// Convert string array into String
                    _fieldsParam = await ConvertStringArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    /// Convert string array into String
                    _formsParam = await ConvertStringArraytoString(formsArray);
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "metadata" },
                    { "format", _redcapFormat },
                    { "returnFormat", _returnFormat }
                };
                _response = await SendRequest(payload);
                return _response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }
        }
        ///
        /// <summary>
        ///The method hands the return content from a request, the response.
        /// The method allows the calling method to choose a return type.
        /// </summary>
        /// <param name="returnContent"></param>
        /// <returns></returns>
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
        /// Tuple that returns both redcapFormat and redcap returnFormat
        /// </summary>
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        private async Task<(string redcapFormat, string returnFormat, string redcapDataType)> HandleFormat(RedcapFormat? redcapFormat = RedcapFormat.json, ReturnFormat? returnFormat = ReturnFormat.json, RedcapDataType? redcapDataType = RedcapDataType.flat)
        {
            // default
            var _redcapFormat = RedcapFormat.json.ToString();
            var _returnFormat = ReturnFormat.json.ToString();
            var _redcapDataType = RedcapDataType.flat.ToString();

            try
            {

                switch (redcapFormat)
                {
                    case RedcapFormat.json:
                        _redcapFormat = RedcapFormat.json.ToString();
                        break;
                    case RedcapFormat.csv:
                        _redcapFormat = RedcapFormat.csv.ToString();
                        break;
                    case RedcapFormat.xml:
                        _redcapFormat = RedcapFormat.xml.ToString();
                        break;
                    default:
                        _redcapFormat = RedcapFormat.json.ToString();
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

                return await Task.FromResult((_redcapFormat, _returnFormat, _redcapDataType));
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return await Task.FromResult((_redcapFormat, _returnFormat, _redcapDataType));
            }
        }
        /// <summary>
        /// Method gets / extracts forms into list from string
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="delimiters"></param>
        /// <returns>List<string></returns>
        private async Task<List<string>> ExtractForms(string forms, char[] delimiters)
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
        /// <param name="delimiters"></param>
        /// <returns>List<string></returns>
        private async Task<List<string>> ExtractFields(string fields, char[] delimiters)
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
        /// <param name="delimiters"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <param name="records">string records e.g "1,2,3,4"</param>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="type">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<string> GetRecordAsync(string records, RedcapFormat redcapFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                string _response;
                var _records = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordResults = await ExtractRecordsAsync(records, delimiters);
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _redcapFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                if (recordResults.Count == 0)
                {
                    Log.Error($"Missing required informaion.");
                    return _records;
                }
                else
                {
                    /// Convert Array List into string array
                    var inputRecords = recordResults.ToArray();
                    /// Convert string array into String
                    _records = await ConvertStringArraytoString(inputRecords);
                    payload.Add("records", _records);
                }
                _response = await SendRequest(payload);
                return _response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }
        }
        /// <summary>
        /// This method allows you to export all records for a project.
        /// Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have "No Access" data export rights in the project, then the 
        /// API data export will fail and return an error. And if you have "De-Identified" 
        /// or "Remove all tagged Identifier fields" data export rights, then some data 
        /// fields *might* be removed and filtered out of the data set returned from the API. 
        /// To make sure that no data is unnecessarily filtered out of your API request, 
        /// you should have "Full Data Set" export rights in the project.
        /// 
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <returns></returns>
        public async Task<string> GetRecordsAsync(RedcapFormat redcapFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType)
        {
            try
            {
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat, redcapDataType);
                var response = String.Empty;
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "record" },
                    { "format", _redcapFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                response = await SendRequest(payload);
                return response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }
        }
        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="type">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        public async Task<string> GetRedcapVersionAsync(RedcapFormat redcapFormat, RedcapDataType redcapDataType)
        {
            try
            {
                var _response = String.Empty;
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, ReturnFormat.json, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "version" },
                    { "format", _redcapFormat },
                    { "type", _redcapDataType }
                };
                // Execute send request
                _response = await SendRequest(payload);

                return await Task.FromResult(_response);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }
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
            }catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(new Dictionary<string, string> { });
            }
        }
        /// <summary>
        /// This method allows you to import a set of records for a project
        /// </summary>
        /// <param name="data"></param>
        /// <param name="redcapFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <returns>ReturnFormat</returns>
        public async Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat)
        {
            try
            {
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat, redcapDataType);
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
                        { "format", _redcapFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", "MDY" },
                        { "returnFormat", _redcapFormat },
                        { "returnContent", "count" },
                        { "data", _serializedData }
                    };

                    // Execute send request
                    var results = await SendRequest(payload);
                    return results;
                }
                return null;
            }
            catch (Exception Ex)
            {
                Log.Error($"Could not save records into redcap.");
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }

        }
        /// <summary>
        /// Method sends the request using http.
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
                var content = new FormUrlEncodedContent(payload);
                var response = await client.PostAsync(client.BaseAddress, content);
                responseString = await response.Content.ReadAsStringAsync();

            }
            return await Task.FromResult(responseString);
        }
        /// <summary>
        /// This method allows you to import a set of records for a project.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="redcapApiKey"></param>
        /// <param name="redcapFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="DateFormat"></param>
        /// <returns>returnFormat</returns>
        public async Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                string _dateFormat = dateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat, redcapDataType);
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
                        { "format", _redcapFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", _dateFormat },
                        { "returnFormat", _redcapFormat },
                        { "returnContent", _returnContent },
                        { "data", formattedData }
                    };

                    // Execute send request
                    var results = await SendRequest(payload);
                    return results;
                }
                return String.Empty;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }

        }
        /// <summary>
        /// Method allows for bulk import of records into redcap project.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="redcapApiKey"></param>
        /// <param name="redcapFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="dateFormat"></param>
        /// <returns>string</returns>
        public async Task<string> SaveRecordsAsync(List<string> data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat, redcapDataType);
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
                        { "format", _redcapFormat },
                        { "type", _redcapDataType },
                        { "overwriteBehavior", _overWriteBehavior },
                        { "dateFormat", _dateFormat },
                        { "returnFormat", _returnFormat },
                        { "returnContent", _returnContent },
                        { "data", _serializedData }
                    };

                    // Execute send request
                    _response = await SendRequest(payload);
                }
                return _response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }

        }

        public Task<string> ExportEvents(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportEvents(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteEvents(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportFields(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportFile(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportFile(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteFile(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportInstruments(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateProject(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectXml(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateNextRecordName(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportRecords(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportRecords(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteRecords(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportRedcapVersion(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyLink(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyParticipants(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyQueueLink(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyReturnCode(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Method exports redcap users for a specific project.
        /// </summary>
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        public async Task<string> ExportUsers(RedcapFormat redcapFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                var _response = String.Empty;
                var _redcapFormat = redcapFormat.ToString();
                var _returnFormat = returnFormat.ToString();
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "user" },
                    { "format", _redcapFormat },
                    { "returnFormat", _returnFormat }
                };

                // Execute send request
                _response = await SendRequest(payload);
                return _response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }

        }

        public Task<string> ImportUsers(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> GetRecordsAsync(RedcapFormat redcapFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method allows you to export the Arms for a project.
        /// NOTE: This only works for longitudinal projects. E.g. Arms are only available in longitudinal projects.
        /// </summary>
        public async Task<string> ExportArms<T>(RedcapFormat redcapFormat, ReturnFormat returnFormat, List<T> arms = null)
        {
            try
            {
                var _response = String.Empty;
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat);
                var _serializedData = JsonConvert.SerializeObject(arms);
                var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "arm" },
                        { "format", _redcapFormat },
                        { "type", _redcapDataType },
                        { "returnFormat", _returnFormat },
                        { "arms", _serializedData }
                    };
                // Execute send request
                _response = await SendRequest(payload);
                return await Task.FromResult(_response);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }
        }
        /// <summary>
        /// This method allows you to import Arms into a project or to rename existing Arms in a project. 
        /// You may use the parameter override=1 as a 'delete all + import' action in order to erase all existing Arms in the project while importing new Arms. 
        /// Notice: Because of the 'override' parameter's destructive nature, this method may only use override=1 for projects in Development status.
        /// NOTE: This only works for longitudinal projects. 
        /// 
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="overRide"></param>
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns>Number of Arms imported</returns>
        public async Task<string> ImportArms<T>(List<T> data, Override overRide, RedcapFormat redcapFormat, ReturnFormat returnFormat)
        {
            try
            {
                var _response = String.Empty;
                var (_redcapFormat, _returnFormat, _redcapDataType) = await HandleFormat(redcapFormat, returnFormat);
                var _override = overRide.ToString();
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "arm" },
                        { "action", "import" },
                        { "format", _redcapFormat },
                        { "type", _redcapDataType },
                        { "override", _override },
                        { "returnFormat", _returnFormat },
                        { "data", _serializedData }
                    };
                // Execute request
                _response = await SendRequest(payload);
                return await Task.FromResult(_response);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }
        }
        /// <summary>
        /// This method allows you to delete Arms from a project. Notice: Because of this method's destructive nature, it is only available for use for projects in Development status. Additionally, please be aware that deleting an arm also automatically deletes all events that belong to that arm, and will also automatically delete any records/data that have been collected under that arm (this is non-reversible data loss).
        /// NOTE: This only works for longitudinal projects. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="overRide"></param>
        /// <param name="RedcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        public async Task<string> DeleteArms<T>(T data)
        {
            try
            {
                var _response = String.Empty;
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", _apiToken },
                    { "content", "arm" },
                    { "action", "delete" },
                    { "arms", _serializedData }
                };
                // Execute request
                _response = await SendRequest(payload);
                return await Task.FromResult(_response);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(String.Empty);
            }

        }

        public Task<string> DeleteArms<T>(List<T> data, Override overRide, RedcapFormat redcapFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
    }
}
