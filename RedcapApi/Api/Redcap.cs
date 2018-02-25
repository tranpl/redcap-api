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
using static System.String;

namespace Redcap
{
    /// <summary>
    /// This api interacts with redcap instances. https://project-redcap.org
    /// Go to your http://redcap_instance/api/help for Redcap Api documentations
    /// Author: Michael Tran tranpl@outlook.com, tranpl@vcu.edu
    /// </summary>
    public class RedcapApi: IRedcap
    {
        /// <summary>
        /// Redcap Api Token
        /// The token can be obtained from your redcap project.
        /// </summary>
        /// <example>
        /// 4AAE216218B33700456A30898F2D6417
        /// </example>
        private static string _token;
        /// <summary>
        /// Redcap API Uri
        /// Location of your redcap instance
        /// </summary>
        /// <example>
        /// https://localhost/redcap/api
        /// </example>
        private static Uri _uri;
        /// <summary>
        /// The version of redcap that the api is currently interacting with.
        /// </summary>
        public static string Version;
        /// <summary>
        /// Constructor requires an api token and a valid uri.
        /// </summary>
        /// <param name="apiToken">Redcap Api Token can be obtained from redcap project or redcap administrators</param>
        /// <param name="redcapApiUrl">Redcap instance URI</param>
        public RedcapApi(string apiToken, string redcapApiUrl)
        {
            _token = apiToken?.ToString();
            _uri = new Uri(redcapApiUrl.ToString());
        }
        private async Task<string> SendRequestAsync(MultipartFormDataContent payload)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uri;
                    using (var response = await client.PostAsync(client.BaseAddress, payload))
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
        private async Task<string> SendRequestAsync(Dictionary<string, string> payload, bool isLargeDataset = false)
        {
            try
            {
                string _responseMessage = String.Empty;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uri;
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
                Log.Error(Ex.Message);
                return string.Empty;
            }
        }
        private async Task<string> SendRequest(Dictionary<string, string> payload)
        {
            string responseString;
            using (var client = new HttpClient())
            {
                client.BaseAddress = _uri;
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
        public async Task<Stream> GetStreamContentAsync(Dictionary<string, string> payload)
        {
            try
            {
                Stream stream = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uri;
                    // Encode the values for payload
                    var content = new FormUrlEncodedContent(payload);
                    using (var response = await client.PostAsync(client.BaseAddress, content))
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
        public delegate Task<string> GetRedcapVersion(InputFormat inputFormat, RedcapDataType redcapDataType);
       
        public delegate Task<string> ExportRecord(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
        public delegate Task<string> ExportRecords(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);

        public async Task<string> GetMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat)
        {
            try
            {
                string _responseMessage;
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }

        public async Task<string> GetMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
        {
            try
            {
                string _responseMessage;
                var _fields = "";
                var _forms = "";
                var _response = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                
                var fieldsResult = await this.ExtractFieldsAsync(fields, delimiters);
                var formsResult = await this.ExtractFormsAsync(forms, delimiters);

                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);

                if (!String.IsNullOrEmpty(fields))
                {
                    // Convert Array List into string array
                    string[] fieldsArray = fieldsResult.ToArray();
                    // Convert string array into String
                    _fields = await this.ConvertStringArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    // Convert string array into String
                    _forms = await this.ConvertStringArraytoString(formsArray);
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }
        
        public async Task<string> GetRecordAsync(string record, InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                string _responseMessage;
                var _records = String.Empty;
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordResults = await this.ExtractRecordsAsync(record, delimiters);
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                    _records = await this.ConvertStringArraytoString(inputRecords);
                    payload.Add("records", _records);
                }
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }

        public async Task<string> GetRecordAsync(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordItems = await this.ExtractRecordsAsync(records: record, delimiters: delimiters);
                var fieldItems = await this.ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await this.ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await this.ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                    payload.Add("records", await this.ConvertStringArraytoString(_inputRecords));
                }
                // Optional
                if(fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertStringArraytoString(_fields));
                }

                // Optional
                if(formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertStringArraytoString(_forms));
                }

                // Optional
                if(eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertStringArraytoString(_events));
                }

                return await SendRequestAsync(payload);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }

        public async Task<string> GetRecordsAsync(InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType)
        {
            string _responseMessage;
            try
            {
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                var response = String.Empty;
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> GetRedcapVersionAsync(InputFormat inputFormat, RedcapDataType redcapDataType)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, ReturnFormat.json, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                if (data != null)
                {
                    List<object> dataList = new List<object>
                    {
                        data
                    };
                    var _serializedData = JsonConvert.SerializeObject(dataList);
                    var _overWriteBehavior = await this.ExtractBehaviorAsync(overwriteBehavior);
                    var payload = new Dictionary<string, string>
                    {
                        { "token", _token },
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
                return string.Empty;
            }

        }

        public async Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                string _responseMessage;
                string _dateFormat = dateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await this.HandleReturnContent(returnContent);
                var _overWriteBehavior = await this.ExtractBehaviorAsync(overwriteBehavior);

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
                        { "token", _token },
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
                return string.Empty;
                
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(string.Empty);
            }
        }

        public async Task<string> SaveRecordsAsync(List<string> data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await this.HandleReturnContent(returnContent);
                var _overWriteBehavior = await this.ExtractBehaviorAsync(overwriteBehavior);

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
                        { "token", _token },
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
                return string.Empty;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(string.Empty);
            }

        }
        public async Task<string> ImportRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY")
        {
            try
            {
                string _responseMessage;
                string _dateFormat = dateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await this.HandleReturnContent(returnContent);
                var _overWriteBehavior = await this.ExtractBehaviorAsync(overwriteBehavior);

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
                        { "token", _token },
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
                return string.Empty;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(string.Empty);
            }
        }
        public async Task<string> ImportRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string apiToken, string dateFormat = "MDY")
        {
            try
            {
                string _apiToken = apiToken;
                string _responseMessage;
                string _dateFormat = dateFormat;
                // Handle optional parameters
                if (String.IsNullOrEmpty(_dateFormat))
                {
                    _dateFormat = "MDY";
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);
                var _returnContent = await this.HandleReturnContent(returnContent);
                var _overWriteBehavior = await this.ExtractBehaviorAsync(overwriteBehavior);

                // Extract properties from object provided
                if (data != null)
                {
                    //List<object> list = new List<object>
                    //{
                    //    data
                    //};
                    var formattedData = JsonConvert.SerializeObject(data);
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
                return string.Empty;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return await Task.FromResult(string.Empty);
            }
        }
        public async Task<string> ExportMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat)
        {
            try
            {
                string _responseMessage;
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }

        }
        public async Task<string> ExportMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
        {
            try
            {
                string _responseMessage;
                var _fields = "";
                var _forms = "";
                if (delimiters.Length == 0)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }

                var fieldsResult = await this.ExtractFieldsAsync(fields, delimiters);
                var formsResult = await this.ExtractFormsAsync(forms, delimiters);

                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);

                if (!String.IsNullOrEmpty(fields))
                {
                    // Convert Array List into string array
                    string[] fieldsArray = fieldsResult.ToArray();
                    // Convert string array into String
                    _fields = await this.ConvertStringArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    // Convert string array into String
                    _forms = await this.ConvertStringArraytoString(formsArray);
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> ExportEventsAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json, int[] arms = null)
        {
            try
            {
                string _responseMessage;
                var _arms = "";
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                if (arms.Length > 0)
                {
                    // Convert string array into String
                    _arms = await this.ConvertIntArraytoString(arms);
                }
                var payload = new Dictionary<string, string>
                {
                    {"arms", _arms },
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> ExportEventsAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                // Handle optional parameters
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
                    { "content", "event" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat }
                };
                return await SendRequestAsync(payload);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        public Task<HttpResponseMessage> ImportEvents(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> DeleteEvents(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportFields(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> DeleteFile(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportInstruments(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ImportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> CreateProject(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ImportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportProjectXml(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> GenerateNextRecordName(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public async Task<string> ExportRecordAsync(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordItems = await this.ExtractRecordsAsync(records: record, delimiters: delimiters);
                var fieldItems = await this.ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await this.ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await this.ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                    payload.Add("records", await this.ConvertStringArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertStringArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertStringArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertStringArraytoString(_events));
                }

                return await SendRequestAsync(payload);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        public async Task<string> ExportRecordsAsync(string records, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                string _responseMessage;
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var recordItems = await this.ExtractRecordsAsync(records: records, delimiters: delimiters);
                var fieldItems = await this.ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await this.ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await this.ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                    payload.Add("records", await this.ConvertStringArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertStringArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertStringArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertStringArraytoString(_events));
                }

                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        public async Task<string> ExportRecordsAsync(InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
        {
            try
            {
                string _responseMessage;
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var fieldItems = await this.ExtractFieldsAsync(fields: fields, delimiters: delimiters);
                var formItems = await this.ExtractFormsAsync(forms: forms, delimiters: delimiters);
                var eventItems = await this.ExtractEventsAsync(events: events, delimiters: delimiters);

                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat, redcapDataType);

                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
                    { "content", "record" },
                    { "format", _inputFormat },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType }
                };
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertStringArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertStringArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertStringArraytoString(_events));
                }

                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        public Task<string> ImportRecords(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> DeleteRecords(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ExportRedcapVersionAsync(InputFormat inputFormat, RedcapDataType redcapDataType)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, ReturnFormat.json, redcapDataType);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public Task<string> ExportSurveyLink(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyParticipants(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyQueueLink(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyReturnCode(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public async Task<string> ExportUsersAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                string _responseMessage;
                var _inputFormat = inputFormat.ToString();
                var _returnFormat = returnFormat.ToString();
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }

        }
        
        public Task<string> ImportUsers(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            throw new NotImplementedException();
        }
        public async Task<string> GetRecordsAsync(InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
        {
            try
            {
                string _responseMessage;
                var _records = String.Empty;
                if (delimiters == null)
                {
                    // Provide some default delimiters, mostly comma and spaces for redcap
                    delimiters = new char[] { ',', ' ' };
                }
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> ExportArmsAsync(InputFormat inputFormat, ReturnFormat returnFormat)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);

                var payload = new Dictionary<string, string>
                    {
                        { "token", _token },
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
                return string.Empty;
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

        public async Task<string> ImportArmsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var _override = overRide.ToString();
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                    {
                        { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> DeleteArmsAsync<T>(T data)
        {
            try
            {
                string _responseMessage;
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }

        }
        public async Task<string> ImportEventsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var _override = overRide.ToString();
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
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
                return string.Empty;
            }

        }
        public Task<string> DeleteEvents()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportFields()
        {
            throw new NotImplementedException();
        }
        public async Task<string> ExportFileAsync(string record, string field, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                string _responseMessage;
                var _returnFormat = returnFormat.ToString();
                var _eventName = eventName;
                var _repeatInstance = repeatInstance;
                var _record = record;
                var _field = field;
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
                    { "content", "file" },
                    { "action", "export" },
                    { "record", _record },
                    { "field", _field },
                    { "event", _eventName },
                    { "returnFormat", _returnFormat }
                };
                if (!string.IsNullOrEmpty(_repeatInstance))
                {
                    payload.Add("repeat_instance", _repeatInstance);
                }
                // Execute request
                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
                return string.Empty;
            }
        }
        public async Task<string> ExportFileAsync(string record, string field, string eventName, string repeatInstance, string filePath = null, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                string _responseMessage;
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
                    { "token", _token },
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
                return string.Empty;
            }
        }
        public async Task<string> ImportFileAsync(string record, string field, string eventName, string repeatInstance, string fileName, string filePath, ReturnFormat returnFormat = ReturnFormat.json)
        {
            
            try
            {
                string _responseMessage;
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
                        {new StringContent(_token), "token" },
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
                return string.Empty;
            }
        }
        public async Task<string> DeleteFileAsync(string record, string field, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            try
            {
                string _responseMessage;
                var _returnFormat = returnFormat.ToString();
                var _eventName = eventName;
                var _repeatInstance = repeatInstance;
                var _record = record;
                var _field = field;
                var payload = new MultipartFormDataContent()
                {
                        {new StringContent(_token), "token" },
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
                return string.Empty;
            }
        }
        public Task<string> ExportInstruments()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportPDFInstruments()
        {
            throw new NotImplementedException();
        }
        public Task<string> ImportPDFInstrument()
        {
            throw new NotImplementedException();
        }
        public Task<string> CreateProject()
        {
            throw new NotImplementedException();
        }
        public Task<string> ImportProjectInfo()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportProjectInfo()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportProjectXml()
        {
            throw new NotImplementedException();
        }
        public Task<string> GenerateNextRecordName()
        {
            throw new NotImplementedException();
        }
        public Task<string> ImportRecords()
        {
            throw new NotImplementedException();
        }
        public Task<string> DeleteRecords()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportRedcapVersion()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyLink()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyParticipants()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyQueueLink()
        {
            throw new NotImplementedException();
        }
        public Task<string> ExportSurveyReturnCode()
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectXml(ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false, bool returnMetaDataOnly = false, string[] records = null, string[] fields = null, string[] events = null)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ExportRecordsAsync(string token, string content, InputFormat format = InputFormat.json, RedcapDataType redcapDatatype = RedcapDataType.flat, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, string rawOrLabel = "raw", string rawOrLabelHeaders = "raw", bool exportCheckboxLabel = false, ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null)
        {
            try
            {

                string _responseMessage;
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token) || IsNullOrEmpty(content) || IsNullOrEmpty(format.ToString()))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                
                /*
                 * If we are Exporting Records, content = record
                 */
                content = "record";
                var _format = format.ToString();
                var _returnFormat = returnFormat.ToString();
                var _redcapDataType = redcapDatatype.ToString();
                /*
                 * Create a payload container to hold all the pieces Redcap needs
                 * to export records for us.
                 */ 
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType },
                    { "rawOrLabel", rawOrLabel},
                    { "rawOrLabelHeaders", rawOrLabelHeaders },
                    { "filterLogic", filterLogic }

                };

                // Optional
                if (records.Count() == 0)
                {
                    // User wants all records
                    payload.Add("records", "");
                }
                else
                {
                    /*
                     * Convert Array List into string array
                    /* User wants certain records
                     */
                    payload.Add("records", await this.ConvertStringArraytoString(records));
                }

                // Optional
                if (fields.Count() > 0)
                {
                    payload.Add("fields", await this.ConvertStringArraytoString(fields));
                }

                // Optional
                if (forms.Count() > 0)
                {
                    payload.Add("forms", await this.ConvertStringArraytoString(forms));
                }

                // Optional
                if (events.Count() > 0)
                {
                    payload.Add("events", await this.ConvertStringArraytoString(events));
                }

                // Optional (defaults to false)
                if (exportCheckboxLabel)
                {
                    payload.Add("exportCheckboxLabel", "true");
                }

                // Optional (defaults to false)
                if (exportSurveyFields)
                {
                    payload.Add("exportSurveyFields", "true");
                }

                // Optional (defaults to false)
                if (exportDataAccessGroups)
                {
                    payload.Add("exportDataAccessGroups", "true");
                }

                _responseMessage = await SendRequestAsync(payload);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }

        }

        public Task<string> DeleteEventsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportFields(ReturnFormat returnFormat, string field = null, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportInstruments(ReturnFormat returnFormat, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportPDFInstruments(string recordId = null, string eventName = null, string instrument = null, bool allRecord = false, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportInstrumentMapping(InputFormat inputFormat = InputFormat.json, string[] arms = null, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportInstrumentMapping<T>(List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateProject<T>(List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, string odm = null, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportProjectInfo<T>(List<T> data, InputFormat inputFormat = InputFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectInfo(InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectXml(ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false, bool returnMetaDataOnly = false, string[] records = null, string[] fields = null, string[] events = null, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateNextRecordName(string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportRecordsAsync(string[] records, InputFormat inputFormat = InputFormat.json, RedcapDataType redcapDataType = RedcapDataType.flat, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string[] forms = null, string[] events = null, string[] fields = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportRecordsAsync(string token, string content, InputFormat format = InputFormat.json, RedcapDataType redcapDatatype = RedcapDataType.flat, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteRecords(string token, string content, string action, string[] records, int arm)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportReports(string token, string content, int reportId, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportRedcapVersionAsync(string token, string content, InputFormat inputFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyLink(string token, string content, string record, string instrument, string eventName, int repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyParticipants(string token, string content, string instrument, string eventName, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyQueueLink(string token, string content, string record, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyReturnCode(string token, string content, string record, string instrument, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportUsersAsync(string token, string content, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportUsers<T>(string token, string content, List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }
    }
}
