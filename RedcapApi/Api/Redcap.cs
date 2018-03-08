using Newtonsoft.Json;
using Redcap.Interfaces;
using Redcap.Models;
using Redcap.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
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

        public Task<string> ExportArmsAsync(string token, string content, InputFormat inputFormat = InputFormat.json, string[] arms = null, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportArmsAsync<T>(string token, string content, Override overrideBhavior, string action, InputFormat inputFormat, List<T> data, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteArmsAsync<T>(string token, string content, string action, string[] arms)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportEventsAsync(string token, string content, InputFormat inputFormat, int[] arms = null, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportEventsAsync<T>(string token, string content, string action, Override overRide, InputFormat inputFormat, List<T> data, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteEventsAsync<T>(string token, string content, string action, string[] events, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportFieldsAsync(string token, string content, InputFormat inputFormat, string field = null, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance, string fileName, string filePath, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportInstrumentsAsync(string token, string content, InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportPDFInstrumentsAsync(string token, string content, string recordId = null, string eventName = null, string instrument = null, bool allRecord = false, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportInstrumentMappingAsync(string token, string content, InputFormat inputFormat = InputFormat.json, string[] arms = null, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportInstrumentMappingAsync<T>(string token, string content, InputFormat inputFormat, List<T> data, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportMetaDataAsync(string token, string content, InputFormat inputFormat, string[] fields = null, string[] forms = null, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportMetaDataAsync<T>(string token, string content, InputFormat inputFormat, List<T> data, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateProjectAsync<T>(string token, string content, InputFormat inputFormat, List<T> data, ReturnFormat returnFormat = ReturnFormat.json, string odm = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportProjectInfoAsync<T>(string token, string content, InputFormat inputFormat, List<T> data)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectInfoAsync(string token, string content, InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportProjectXmlAsync(string token, string content, bool returnMetadataOnly = false, string[] records = null, string[] events = null, ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateNextRecordNameAsync(string token, string content)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Api Version 1.0.0
        /// Export Records
        /// This method allows you to export a set of records for a project.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API data export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="inputFormat">csv, json [default], xml, odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="rawOrLabel">raw [default], label - export the raw coded values or labels for the options of multiple choice fields</param>
        /// <param name="rawOrLabelHeaders">raw [default], label - (for 'csv' format 'flat' type only) for the CSV headers, export the variable/field names (raw) or the field labels (label)</param>
        /// <param name="exportCheckboxLabel">true, false [default] - specifies the format of checkbox field values specifically when exporting the data as labels (i.e., when rawOrLabel=label) in flat format (i.e., when type=flat). When exporting labels, by default (without providing the exportCheckboxLabel flag or if exportCheckboxLabel=false), all checkboxes will either have a value 'Checked' if they are checked or 'Unchecked' if not checked. But if exportCheckboxLabel is set to true, it will instead export the checkbox value as the checkbox option's label (e.g., 'Choice 1') if checked or it will be blank/empty (no value) if not checked. If rawOrLabel=false or if type=eav, then the exportCheckboxLabel flag is ignored. (The exportCheckboxLabel parameter is ignored for type=eav because 'eav' type always exports checkboxes differently anyway, in which checkboxes are exported with their true variable name (whereas the 'flat' type exports them as variable___code format), and another difference is that 'eav' type *always* exports checkbox values as the choice label for labels export, or as 0 or 1 (if unchecked or checked, respectively) for raw export.)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<string> ExportRecordsAsync<T>(string token, string content, InputFormat inputFormat, RedcapDataType redcapDataType, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token) || IsNullOrEmpty(content) || IsNullOrEmpty(inputFormat.ToString()))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }

                /*
                 * Set content type
                 * content = record
                 */
                content = "record";
                var _format = inputFormat.ToString();
                var _returnFormat = returnFormat.ToString();
                var _redcapDataType = redcapDataType.ToString();
                /*
                 * Create a payload container to hold all the pieces Redcap needs
                 * to export records for us. We'll need to convert all our arguments
                 * as string because dictionary is created as such.
                 */
                var requestPayload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _returnFormat },
                    { "type", _redcapDataType },
                    { "rawOrLabel", rawOrLabel.ToString()},
                    { "rawOrLabelHeaders", rawOrLabelHeaders.ToString() },
                    { "filterLogic", filterLogic }

                };

                // Optional
                if (records.Count() == 0)
                {
                    // User wants all records
                    requestPayload.Add("records", "");
                }
                else
                {
                    /*
                     * Convert Array to string
                    /* 
                     */
                    requestPayload.Add("records", await this.ConvertStringArraytoString(records));
                }

                // Optional
                if (fields.Count() > 0)
                {
                    requestPayload.Add("fields", await this.ConvertStringArraytoString(fields));
                }

                // Optional
                if (forms.Count() > 0)
                {
                    requestPayload.Add("forms", await this.ConvertStringArraytoString(forms));
                }

                // Optional
                if (events.Count() > 0)
                {
                    requestPayload.Add("events", await this.ConvertStringArraytoString(events));
                }

                // Optional (defaults to false)
                if (exportCheckboxLabel)
                {
                    requestPayload.Add("exportCheckboxLabel", exportCheckboxLabel.ToString());
                }

                // Optional (defaults to false)
                if (exportSurveyFields)
                {
                    requestPayload.Add("exportSurveyFields", exportSurveyFields.ToString());
                }

                // Optional (defaults to false)
                if (exportDataAccessGroups)
                {
                    requestPayload.Add("exportDataAccessGroups", exportDataAccessGroups.ToString());
                }
                /*
                 * We return what the response provides, no opinions here.
                 */ 
                return await this.SendRequestAsync(payload: requestPayload, uri: _uri, isLargeDataset: false);
            }
            catch (Exception Ex)
            {
                /*
                 * We'll just log the error and return the error message.
                 */ 
                Log.Error($"{Ex.Message}");
                return Ex.Message;
            }

        }

        public Task<string> ImportRecordsAsync<T>(string token, string content, InputFormat inputFormat, RedcapDataType redcapDataType, OverwriteBehavior overwriteBehavior, bool forceAutoNumber, List<T> data, string dateFormat, ReturnContent returnContent = ReturnContent.count, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteRecordsAsync(string token, string content, string action, string[] records, int? arm)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportReportsAsync(string token, string content, int reportId, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportRedcapVersionAsync(string token, string content, InputFormat inputFormat)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyLinkAsync(string token, string content, string record, string instrument, string eventName, int repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyParticipantsAsync(string token, string content, string instrument, string eventName, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyQueueLinkAsync(string token, string content, string record, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportSurveyReturnCodeAsync(string token, string content, string record, string instrument, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportUsersAsync(string token, string content, InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImportUsersAsync<T>(string token, string content, List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json)
        {
            throw new NotImplementedException();
        }

        #region deprecated methods < version 1.0.0
        /// <summary>
        /// Export Arms
        /// </summary>
        /// <param name="inputFormat">test</param>
        /// <param name="returnFormat">test</param>
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }

        }
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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

                return await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                    _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                    _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                    _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                    _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                    _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        /// <summary>
        /// Alias /test/compatibility
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <returns>string</returns>
        public delegate Task<string> GetRedcapVersion(InputFormat inputFormat, RedcapDataType redcapDataType);
        /// <summary>
        /// Alias /test/compatibility
        /// </summary>
        /// <param name="record"></param>
        /// <param name="inputFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="delimiters"></param>
        /// <param name="forms"></param>
        /// <param name="events"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public delegate Task<string> ExportRecord(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
        /// <summary>
        /// Alias /test/compatibility
        /// </summary>
        /// <param name="record"></param>
        /// <param name="inputFormat"></param>
        /// <param name="redcapDataType"></param>
        /// <param name="returnFormat"></param>
        /// <param name="delimiters"></param>
        /// <param name="forms"></param>
        /// <param name="events"></param>
        /// <param name="fields"></param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public delegate Task<string> ExportRecords(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
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

                return await this.SendRequestAsync(payload, _uri);
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

                _responseMessage = await this.SendRequestAsync(payload, _uri);
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

                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);

                return await Task.FromResult(_responseMessage);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }

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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }

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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
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
                _responseMessage = await this.SendRequestAsync(payload, _uri);
                return _responseMessage;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
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
                return await this.SendRequestAsync(payload, _uri);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        #endregion deprecated
    }
}
