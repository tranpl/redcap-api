using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redcap.Interfaces;
using System.Net.Http;
using Serilog;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Redcap
{
    /// <summary>
    /// This api interacts with redcap instances. https://project-redcap.org
    /// Go to your http://redcap_instance/api/help for Redcap Api documentations
    /// Author: Michael Tran tranpl@outlook.com
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
        /// <param name="array"></param>
        /// <returns>string[]</returns>
        private async Task<string> ConvertStringArraytoString(string[] array)
        {
            try {
                StringBuilder builder = new StringBuilder();
                ///builder.Append('[');
                foreach (string v in array)
                {

                    builder.Append(v);
                    /// We do not need to append the , if less than or equal to 1 record
                    if (array.Length <= 1)
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
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<string> GetMetaData(RedcapFormat? redcapFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
        {
            try
            {
                var fieldsParam = "";
                var formsParam = "";
                var response = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                
                var fieldsResult = await ExtractFields(fields, delimiters);
                var formsResult = await ExtractForms(forms, delimiters);
                // Handle optional parameters

                var (format, retFormat) = await HandleFormat(redcapFormat, returnFormat);

                if (!String.IsNullOrEmpty(fields))
                {
                    /// Convert Array List into string array
                    //string[] fieldsArray = fieldsContainer.ToArray(typeof(string)) as string[];
                    string[] fieldsArray = fieldsResult.ToArray();
                    /// Convert string array into String
                    fieldsParam = ConvertStringArraytoString(fieldsArray).Result;
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    //string[] formsArray = formsContainer.ToArray(typeof(string)) as string[];
                    string[] formsArray = formsResult.ToArray();
                    /// Convert string array into String
                    formsParam = await ConvertStringArraytoString(formsArray);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = _redcapApiUri;
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "metadata" },
                        { "format", format },
                        { "returnFormat", retFormat }
                    };
                    response = await SendRequest(payload);
                }
                return response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }
        }
        /// <summary>
        /// Tuple that returns both redcapFormat and redcap returnFormat
        /// </summary>
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        private async Task<(string redcapFormat, string returnFormat)> HandleFormat(RedcapFormat? redcapFormat, ReturnFormat? returnFormat)
        {
            // default formatters
            var rcFormat = RedcapFormat.json.ToString();
            var retFormat = ReturnFormat.json.ToString();
            try
            {

                if (redcapFormat == null || returnFormat == null)
                {
                    rcFormat = RedcapFormat.json.ToString();
                    retFormat = ReturnFormat.json.ToString();
                }
                else
                {
                    rcFormat = redcapFormat.ToString();
                    retFormat = returnFormat.ToString();
                }
                return await Task.FromResult((rcFormat, retFormat));
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return await Task.FromResult((rcFormat, retFormat));
            }
        }
        private async Task<(string redcapFormat, string redcapDataType)> HandleFormat(RedcapFormat? redcapFormat, RedcapDataType? redcapDataType)
        {
            // default formatters
            var rcFormat = RedcapFormat.json.ToString();
            var type = RedcapDataType.flat.ToString();
            try
            {

                if (redcapFormat == null || redcapDataType == null)
                {
                    rcFormat = RedcapFormat.json.ToString();
                    type = RedcapDataType.flat.ToString();
                }
                else
                {
                    rcFormat = redcapFormat.ToString();
                    type = redcapDataType.ToString();
                }
                return await Task.FromResult((rcFormat, type));
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult((rcFormat, type));
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
        private async Task<List<string>> ExtractRecords(string records, char[] delimiters)
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
        public async Task<string> GetRecordAsync(string records, RedcapFormat redcapFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                string response;
                var _records = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordResults = await ExtractRecords(records, delimiters);
                var (format, type) = await HandleFormat(redcapFormat, redcapDataType); 
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _redcapApiUri;
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", format },
                        { "type", type }
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
                    payload.Add("returnFormat", format);
                    response = await SendRequest(payload);
                }
                return response;
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
        /// </summary>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="type">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<string> GetRecordsAsync(RedcapFormat redcapFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                var (format, type) = await HandleFormat(redcapFormat, redcapDataType);
                var response = String.Empty;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _redcapApiUri;
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", format },
                        { "type", type },
                        { "returnFormat", format }
                    };
                    response = await SendRequest(payload);
                }
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
                var (format, type) = await HandleFormat(redcapFormat, redcapDataType);
                var response = String.Empty;
                using (var client = new HttpClient())
                {
                    // Set the base address 
                    client.BaseAddress = _redcapApiUri;
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "version" },
                        { "format", format },
                        { "type", type }
                    };
                    // Execute send request
                    response = await SendRequest(payload);
                }
                return await Task.FromResult(response);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }

        }
        /// <summary>
        /// This method allows you to export the list of users for a project, including their user privileges and also email address, first name, and last name. Note: If the user has been assigned to a user role, it will return the user with the role's defined privileges.
        /// </summary>
        /// <returns></returns>
        public Task<string> ExportUsers()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method extracts and converts an object's properties and associated values to redcap type and values.
        /// </summary>
        /// <param name="item">Object</param>
        /// <returns>Dictionary of key value pair.</returns>
        private async Task<Dictionary<string, string>> GetProperties(object item)
        {
            try
            {
                if (item != null)
                {
                    // Get the type
                    var type = item.GetType();
                    var obj = new Dictionary<string, string>();
                    // Get the properties
                    var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] properties = item.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        // get type of column
                        // The the type of the property
                        Type columnType = prop.PropertyType;

                        // We need to set lower case for REDCap's variable nameing convention (lower casing)
                        string propName = prop.Name.ToLower();
                        // We check for null values
                        var propValue = type.GetProperty(prop.Name).GetValue(item, null)?.ToString();
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
        public async Task<string> SaveRecordsAsync(object data, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat)
        {
            try
            {
                var (format, type) = await HandleFormat(redcapFormat, redcapDataType);
                if (data != null)
                {
                    List<object> dataList = new List<object>
                    {
                        data
                    };
                    var serializedData = JsonConvert.SerializeObject(dataList);
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", format },
                        { "type", type },
                        { "overwriteBehavior", "overwrite" },
                        { "dateFormat", "MDY" },
                        { "returnFormat", format },
                        { "returnContent", "count" },
                        { "data", serializedData }
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
        public async Task<string> SaveRecordsAsync(object data, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, OverwriteBehavior? overwriteBehavior, string DateFormat = "MDY")
        {
            try
            {
                var format = redcapFormat.ToString();
                var retFormat = returnFormat.ToString();
                var dataType = redcapDataType.ToString();
                var overwrite = overwriteBehavior.ToString();
                string dateFormat = DateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(dateFormat))
                {
                    dateFormat = "MDY";
                }
                if (redcapFormat == null || returnFormat == null || redcapDataType == null)
                {
                    // Defaults to JSON if not provided
                    format = RedcapFormat.json.ToString(); // Json Format
                    retFormat = ReturnFormat.json.ToString(); // Json Format
                    dataType = RedcapDataType.flat.ToString(); // Flat Type
                    overwrite = OverwriteBehavior.overwrite.ToString(); // overwrite
                }
                else
                {
                    format = redcapFormat.ToString();
                    retFormat = returnFormat.ToString();
                    dataType = redcapDataType.ToString();
                    overwrite = OverwriteBehavior.overwrite.ToString(); // overwrite
                }

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
                        { "format", format },
                        { "type", dataType },
                        { "overwriteBehavior", overwrite },
                        { "dateFormat", dateFormat },
                        { "returnFormat", format },
                        { "returnContent", "ids" },
                        { "data", formattedData }
                    };

                    // Execute send request
                    var results = await SendRequest(payload);
                    return results;
                }
                return null;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return null;
            }

        }
        /// <summary>
        /// Bulk Import
        /// </summary>
        /// <param name="data"></param>
        /// <param name="redcapApiKey"></param>
        /// <param name="redcapFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="dateFormat"></param>
        /// <returns>string</returns>
        public async Task<string> SaveRecordsAsync(List<string> data, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, OverwriteBehavior? overwriteBehavior, string dateFormat = "MDY")
        {
            try
            {
                var format = "";
                var retFormat = "";
                var dataType = "";
                var overwrite = "";
                var response = String.Empty;
                var _dateFormat = dateFormat;
                // Handle optional parameters
                if (string.IsNullOrEmpty((string)_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                if (redcapFormat == null || returnFormat == null || redcapDataType == null)
                {
                    // Defaults to JSON if not provided
                    format = RedcapFormat.json.ToString(); // Json Format
                    retFormat = ReturnFormat.json.ToString(); // Json Format
                    dataType = RedcapDataType.flat.ToString(); // Flat Type
                    overwrite = OverwriteBehavior.overwrite.ToString(); // overwrite
                }
                else
                {
                    format = redcapFormat.ToString();
                    retFormat = returnFormat.ToString();
                    dataType = redcapDataType.ToString();
                    overwrite = OverwriteBehavior.overwrite.ToString(); // overwrite
                }

                // Extract properties from object provided
                if (data != null)
                {
                    var serializedData = JsonConvert.SerializeObject(data);
                    var content = new Dictionary<string, string>
                    {
                        { "token", _apiToken },
                        { "content", "record" },
                        { "format", format },
                        { "type", dataType },
                        { "overwriteBehavior", overwrite },
                        { "dateFormat", _dateFormat },
                        { "returnFormat", retFormat },
                        { "returnContent", "ids" },
                        { "data", serializedData }
                    };

                    // Execute send request
                    response = await SendRequest(content);
                }
                return response;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return String.Empty;
            }

        }
    }
}
