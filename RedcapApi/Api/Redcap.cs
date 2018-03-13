using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redcap.Interfaces;
using Redcap.Models;
using Redcap.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static System.String;

namespace Redcap
{
    /// <summary>
    /// This api interacts with redcap instances. https://project-redcap.org
    /// Go to your http://redcap_instance/api/help for Redcap Api documentations
    /// Author: Michael Tran tranpl@outlook.com, tranpl@vcu.edu
    /// </summary>
    public class RedcapApi : IRedcap
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
        /// Constructor requires an api token and a valid url.
        /// </summary>
        /// <remarks>
        /// Token is required only for older APIs. As of version 1.0.0, token is required for each
        /// method used.
        /// </remarks>
        /// <param name="apiToken">Redcap Api Token can be obtained from redcap project or redcap administrators</param>
        /// <param name="redcapApiUrl">Redcap instance URI</param>
        public RedcapApi(string apiToken, string redcapApiUrl)
        {
            _token = apiToken?.ToString();
            _uri = new Uri(redcapApiUrl);
        }
        /// <summary>
        /// Constructor requires a valid url.
        /// </summary>
        /// <remarks>
        /// Overloaded constructor
        /// </remarks>
        /// 
        /// <param name="redcapApiUrl">Redcap instance URI</param>
        public RedcapApi(string redcapApiUrl)
        {
            _uri = new Uri(redcapApiUrl);
        }

        #region API Version 1.0.0 Begin
        /// <summary>
        /// API Version 1.0.0 **
        /// Export Arms
        /// This method allows you to export the Arms for a project
        /// NOTE: This only works for longitudinal projects.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">arm</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="arms">an array of arm numbers that you wish to pull events for (by default, all events are pulled)</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Arms for the project in the format specified(only ones with Events available)</returns>
        public async Task<string> ExportArmsAsync(string token, string content, ReturnFormat format = ReturnFormat.json, string[] arms = null, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                /*
                 * Get the formats
                 */
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);
                if (IsNullOrEmpty(content))
                {
                    content = "arm";
                }
                /*
                 * Request payload
                 */
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format }
                };
                // Optional
                if (arms?.Length > 0)
                {
                    payload.Add("arms", await this.ConvertArraytoString(arms));
                }
                var _onErrorFormatValue = _onErrorFormat.ToString();
                if (!IsNullOrEmpty(_onErrorFormatValue))
                {
                    // defaults to 'xml'
                    payload.Add("returnFormat", _onErrorFormatValue);
                }
                // Execute send request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import Arms
        /// This method allows you to import Arms into a project or to rename existing Arms in a project. 
        /// You may use the parameter override=1 as a 'delete all + import' action in order to erase all existing Arms in the project while importing new Arms. 
        /// Notice: Because of the 'override' parameter's destructive nature, this method may only use override=1 for projects in Development status.
        /// NOTE: This only works for longitudinal projects. 
        /// 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">arm</param>
        /// <param name="overrideBhavior">0 - false [default], 1 - true — You may use override=1 as a 'delete all + import' action in order to erase all existing Arms in the project while importing new Arms. If override=0, then you can only add new Arms or rename existing ones. </param>
        /// <param name="action">import</param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="data">Contains the attributes 'arm_num' (referring to the arm number) and 'name' (referring to the arm's name) of each arm to be created/modified, in which they are provided in the specified format. 
        /// [{"arm_num":"1","name":"Drug A"},
        /// {"arm_num":"2","name":"Drug B"},
        /// {"arm_num":"3","name":"Drug C"}]
        /// </param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Number of Arms imported</returns>
        public async Task<string> ImportArmsAsync<T>(string token, string content, Override overrideBhavior, string action, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, returnFormat);
                var _override = overrideBhavior.ToString();
                if (IsNullOrEmpty(content))
                {
                    content = "arm";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "import";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                    {
                        { "token", token },
                        { "content", content },
                        { "action", action },
                        { "format", _inputFormat },
                        { "type", _redcapDataType },
                        { "override", _override },
                        { "returnFormat", _returnFormat },
                        { "data", _serializedData }
                    };
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Delete Arms
        /// This method allows you to delete Arms from a project.
        /// Notice: Because of this method's destructive nature, it is only available for use for projects in Development status. Additionally, please be aware that deleting an arm also automatically deletes all events that belong to that arm, and will also automatically delete any records/data that have been collected under that arm (this is non-reversible data loss).        
        /// NOTE: This only works for longitudinal projects. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">arm</param>
        /// <param name="action">delete</param>
        /// <param name="arms">an array of arm numbers that you wish to delete</param>
        /// <returns>Number of Arms deleted</returns>
        public async Task<string> DeleteArmsAsync(string token, string content, string action, string[] arms)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                if (IsNullOrEmpty(content))
                {
                    content = "arm";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "delete";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "action", action }
                };
                // Required
                payload.Add("arms[0]", await this.ConvertArraytoString(arms));
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Events
        /// This method allows you to export the events for a project
        /// NOTE: This only works for longitudinal projects.
        /// 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">
        /// The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.
        /// </param>
        /// <param name="content">event</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="arms"></param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Events for the project in the format specified</returns>
        public async Task<string> ExportEventsAsync(string token, string content, ReturnFormat format, string[] arms = null, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle optional parameters
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat, null);
                if (IsNullOrEmpty(content))
                {
                    content = "event";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat }
                };
                // Optional
                if (arms?.Length > 0)
                {
                    payload.Add("arms", await this.ConvertArraytoString(arms));
                }
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import Events
        /// This method allows you to import Events into a project or to update existing Events' attributes, such as the event name, days offset, etc. The unique event name of an Event cannot be changed because it is auto-generated by REDCap. Please note that the only way to update an existing Event is to provide the unique_event_name attribute, and if the unique_event_name attribute is missing for an Event being imported (when override=0), it will assume it to be a new Event that should be created. Notice: Because of the 'override' parameter's destructive nature, this method may only use override=1 for projects in Development status.
        /// NOTE: This only works for longitudinal projects. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">event</param>
        /// <param name="action">import</param>
        /// <param name="overRideBehavior">0 - false [default], 1 - true — You may use override=1 as a 'delete all + import' action in order to erase all existing Events in the project while importing new Events. If override=0, then you can only add new Events or modify existing ones. </param>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Contains the required attributes 'event_name' (referring to the name/label of the event) and 'arm_num' (referring to the arm number to which the event belongs - assumes '1' if project only contains one arm). In order to modify an existing event, you must provide the attribute 'unique_event_name' (referring to the auto-generated unique event name of the given event). If the project utilizes the Scheduling module, the you may optionally provide the following attributes, which must be numerical: day_offset, offset_min, offset_max. If the day_offset is not provided, then the events will be auto-numbered in the order in which they are provided in the API request. 
        /// [{"event_name":"Baseline","arm_num":"1","day_offset":"1","offset_min":"0",
        /// "offset_max":"0","unique_event_name":"baseline_arm_1"},
        /// {"event_name":"Visit 1","arm_num":"1","day_offset":"2","offset_min":"0",
        /// "offset_max":"0","unique_event_name":"visit_1_arm_1"},
        /// {"event_name":"Visit 2","arm_num":"1","day_offset":"3","offset_min":"0",
        /// "offset_max":"0","unique_event_name":"visit_2_arm_1"}]
        /// </param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Number of Events imported</returns>
        public async Task<string> ImportEventsAsync<T>(string token, string content, string action, Override overRideBehavior, ReturnFormat format, List<T> data, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat, null);
                var _override = overRideBehavior.ToString();
                if (IsNullOrEmpty(content))
                {
                    content = "event";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "import";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "action", action },
                    { "format", _format },
                    { "override", _override },
                    { "returnFormat", _onErrorFormat },
                    { "data", _serializedData }
                };
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Delete Events
        /// This method allows you to delete Events from a project. 
        /// Notice: Because of this method's destructive nature, it is only available for use for projects in Development status. 
        /// Additionally, please be aware that deleting an event will automatically delete any records/data that have been collected under that event (this is non-reversible data loss).
        /// NOTE: This only works for longitudinal projects.
        /// </summary>
        /// <remarks>
        ///  
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <param name="action"></param>
        /// <param name="events">Array of unique event names</param>
        /// <returns>Number of Events deleted</returns>
        public async Task<string> DeleteEventsAsync(string token, string content, string action, string[] events)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                if (IsNullOrEmpty(content))
                {
                    content = "event";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "delete";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "action", action }
                };
                // Required
                if(events?.Length > 0)
                {
                    payload.Add("events[0]", await this.ConvertArraytoString(events));
                }
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// Export List of Export Field Names (i.e. variables used during exports and imports)
        /// 
        /// This method returns a list of the export/import-specific version of field names for all fields (or for one field, if desired) in a project. 
        /// This is mostly used for checkbox fields because during data exports and data imports, checkbox fields have a different variable name used than the exact one defined for them in the Online Designer and Data Dictionary, in which *each checkbox option* gets represented as its own export field name in the following format: field_name + triple underscore + converted coded value for the choice. 
        /// For non-checkbox fields, the export field name will be exactly the same as the original field name. 
        /// Note: The following field types will be automatically removed from the list returned by this method since they cannot be utilized during the data import process: 'calc', 'file', and 'descriptive'.
        /// 
        /// The list that is returned will contain the three following attributes for each field/choice: 'original_field_name', 'choice_value', and 'export_field_name'. 
        /// The choice_value attribute represents the raw coded value for a checkbox choice.For non-checkbox fields, the choice_value attribute will always be blank/empty.
        /// The export_field_name attribute represents the export/import-specific version of that field name.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">exportFieldNames</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="field">A field's variable name. By default, all fields are returned, but if field is provided, then it will only the export field name(s) for that field. If the field name provided is invalid, it will return an error.</param>
        /// <param name="onErrorFormat">csv, json [default], xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'. 
        /// The list that is returned will contain the original field name (variable) of the field and also the export field name(s) of that field.</param>
        /// <returns>Returns a list of the export/import-specific version of field names for all fields (or for one field, if desired) in a project in the format specified and ordered by their field order . 
        /// The list that is returned will contain the three following attributes for each field/choice: 'original_field_name', 'choice_value', and 'export_field_name'. The choice_value attribute represents the raw coded value for a checkbox choice. For non-checkbox fields, the choice_value attribute will always be blank/empty. The export_field_name attribute represents the export/import-specific version of that field name.
        /// </returns>
        public async Task<string> ExportFieldNamesAsync(string token, string content, ReturnFormat format, string field = null, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _returnFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "exportFieldNames";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _returnFormat }

                };
                if (!IsNullOrEmpty(field))
                {
                    payload.Add("field", field);
                }
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export a File
        /// This method allows you to download a document that has been attached to an individual record for a File Upload field. Please note that this method may also be used for Signature fields (i.e. File Upload fields with 'signature' validation type).
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API file export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then the API file export will fail and return an error *only if* the File Upload field has been tagged as an Identifier field.To make sure that your API request does not return an error, you should have 'Full Data Set' export rights in the project.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <example>
        /// The MIME type of the file, along with the name of the file and its extension, can be found in the header of the returned response. Thus in order to determine these attributes of the file being exported, you will need to parse the response header. Example: content-type = application/vnd.openxmlformats-officedocument.wordprocessingml.document; name='FILE_NAME.docx'
        /// </example>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">file</param>
        /// <param name="action">export</param>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>the contents of the file</returns>
        public async Task<string> ExportFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */ 
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "file";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "export";
                }
                var _repeatInstance = repeatInstance;
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "action", action },
                    { "record", record },
                    { "field", field },
                    { "event", eventName },
                    { "returnFormat", _onErrorFormat },
                    { "repeat_instance", repeatInstance  }
                };
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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
        /// <summary>
        /// API Version 1.0.0
        /// Export a File
        /// **Allows for file download to a path.**
        /// This method allows you to download a document that has been attached to an individual record for a File Upload field. Please note that this method may also be used for Signature fields (i.e. File Upload fields with 'signature' validation type).
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API file export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then the API file export will fail and return an error *only if* the File Upload field has been tagged as an Identifier field.To make sure that your API request does not return an error, you should have 'Full Data Set' export rights in the project.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <example>
        /// The MIME type of the file, along with the name of the file and its extension, can be found in the header of the returned response. Thus in order to determine these attributes of the file being exported, you will need to parse the response header. Example: content-type = application/vnd.openxmlformats-officedocument.wordprocessingml.document; name='FILE_NAME.docx'
        /// </example>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">file</param>
        /// <param name="action">export</param>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="filePath">File path which the file will be saved.</param>
        /// <returns>the file name that was exported</returns>
        public async Task<string> ExportFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance = "1", OnErrorFormat onErrorFormat = OnErrorFormat.json, string filePath = null)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "file";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "export";
                }
                /*
                 * FilePath check..
                 */ 
                if (!Directory.Exists(filePath) && !IsNullOrEmpty(filePath))
                {
                    Log.Warning($"The directory provided does not exist! Creating a folder for you.");
                    Directory.CreateDirectory(filePath);
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", _token },
                    { "content", content },
                    { "action", action },
                    { "record", record },
                    { "field", field },
                    { "event", eventName },
                    { "returnFormat", _onErrorFormat },
                    { "filePath", $@"{filePath}" }
                };
                // Optional
                if (!IsNullOrEmpty(repeatInstance))
                {
                    payload.Add("repeat_instance", repeatInstance);
                }
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import a File
        /// This method allows you to upload a document that will be attached to an individual record for a File Upload field. Please note that this method may NOT be used for Signature fields (i.e. File Upload fields with 'signature' validation type) because a signature can only be captured and stored using the web interface. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges in the project.
        /// If you pass in a record parameter that does not exist, Redcap will create it for you.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">file</param>
        /// <param name="action">import</param>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param> 
        /// <param name="fileName">The File you be imported, contents of the file</param>
        /// <param name="filePath">the path where the file is located</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</returns>
        public async Task<string> ImportFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance, string fileName, string filePath, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "file";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "import";
                }
                var _fileName = fileName;
                var _filePath = filePath;
                var _binaryFile = Path.Combine(_filePath, _fileName);
                ByteArrayContent _fileContent;
                var payload = new MultipartFormDataContent()
                {
                        {new StringContent(token), "token" },
                        {new StringContent(content) ,"content" },
                        {new StringContent(action), "action" },
                        {new StringContent(record), "record" },
                        {new StringContent(field), "field" },
                        {new StringContent(eventName),  "event" },
                        {new StringContent(_onErrorFormat), "returnFormat" }
                };
                if (!IsNullOrEmpty(repeatInstance))
                {
                    // add repeat instrument params if available
                    payload.Add(new StringContent(repeatInstance), "repeat_instance");
                }
                if (IsNullOrEmpty(_fileName) || IsNullOrEmpty(_filePath))
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
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Delete a File
        /// This method allows you to remove a document that has been attached to an individual record for a File Upload field. Please note that this method may also be used for Signature fields (i.e. File Upload fields with 'signature' validation type).
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">file</param>
        /// <param name="action">delete</param>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>String</returns>
        public async Task<string> DeleteFileAsync(string token, string content, string action, string record, string field, string eventName, string repeatInstance, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "file";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "delete";
                }
                var payload = new MultipartFormDataContent()
                {
                        {new StringContent(token), "token" },
                        {new StringContent(content) ,"content" },
                        {new StringContent(action), "action" },
                        {new StringContent(record), "record" },
                        {new StringContent(field), "field" },
                        {new StringContent(eventName),  "event" },
                        {new StringContent(_returnFormat), "returnFormat" }
                };
                if (!IsNullOrEmpty(repeatInstance))
                {
                    // add repeat instrument params if available
                    payload.Add(new StringContent(repeatInstance), "repeat_instance");
                }
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Instruments (Data Entry Forms)
        /// This method allows you to export a list of the data collection instruments for a project. 
        /// This includes their unique instrument name as seen in the second column of the Data Dictionary, as well as each instrument's corresponding instrument label, which is seen on a project's left-hand menu when entering data. The instruments will be ordered according to their order in the project.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">instrument</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <returns>Instruments for the project in the format specified and will be ordered according to their order in the project.</returns>
        public async Task<string> ExportInstrumentsAsync(string token, string content, ReturnFormat format)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, null);

                if (IsNullOrEmpty(content))
                {
                    content = "instrument";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format }
                };
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// Export PDF file of Data Collection Instruments (either as blank or with data)
        /// This method allows you to export a PDF file for any of the following: 1) a single data collection instrument (blank), 2) all instruments (blank), 3) a single instrument (with data from a single record), 4) all instruments (with data from a single record), or 5) all instruments (with data from ALL records). 
        /// This is the exact same PDF file that is downloadable from a project's data entry form in the web interface, and additionally, the user's privileges with regard to data exports will be applied here just like they are when downloading the PDF in the web interface (e.g., if they have de-identified data export rights, then it will remove data from certain fields in the PDF). 
        /// If the user has 'No Access' data export rights, they will not be able to use this method, and an error will be returned.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">pdf</param>
        /// <param name="recordId">the record ID. The value is blank by default. If record is blank, it will return the PDF as blank (i.e. with no data). If record is provided, it will return a single instrument or all instruments containing data from that record only.</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects. For a longitudinal project, if record is not blank and event is blank, it will return data for all events from that record. If record is not blank and event is not blank, it will return data only for the specified event from that record.</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. The value is blank by default, which returns all instruments. If record is not blank and instrument is blank, it will return all instruments for that record.</param>
        /// <param name="allRecord">[The value of this parameter does not matter and is ignored.] If this parameter is passed with any value, it will export all instruments (and all events, if longitudinal) with data from all records. Note: If this parameter is passed, the parameters record, event, and instrument will be ignored.</param>
        /// <param name="onErrorFormat">csv, json [default] , xml- The returnFormat is only used with regard to the format of any error messages that might be returned.</param>
        /// <returns>A PDF file containing one or all data collection instruments from the project, in which the instruments will be blank (no data), contain data from a single record, or contain data from all records in the project, depending on the parameters passed in the API request.</returns>
        public async Task<string> ExportPDFInstrumentsAsync(string token, string content, string recordId = null, string eventName = null, string instrument = null, bool allRecord = false, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "pdf";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "returnFormat", _onErrorFormat }
                };
                // Add all optional parameters
                if (!IsNullOrEmpty(recordId))
                {
                    payload.Add("record", recordId);
                }
                if (!IsNullOrEmpty(eventName))
                {
                    payload.Add("event", eventName);
                }
                if (!IsNullOrEmpty(instrument))
                {
                    payload.Add("instrument", instrument);
                }
                if (allRecord)
                {
                    payload.Add("allRecords", allRecord.ToString());
                }
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Instrument-Event Mappings
        /// This method allows you to export the instrument-event mappings for a project (i.e., how the data collection instruments are designated for certain events in a longitudinal project).
        /// NOTE: This only works for longitudinal projects.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">formEventMapping</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="arms">an array of arm numbers that you wish to pull events for (by default, all events are pulled)</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Instrument-event mappings for the project in the format specified</returns>
        public async Task<string> ExportInstrumentMappingAsync(string token, string content, ReturnFormat format = ReturnFormat.json, string[] arms = null, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "formEventMapping";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat }
                };
                // Add all optional parameters
                if (arms?.Length > 0)
                {
                    payload.Add("arms", await this.ConvertArraytoString(arms));
                }
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import Instrument-Event Mappings
        /// This method allows you to import Instrument-Event Mappings into a project (this corresponds to the 'Designate Instruments for My Events' page in the project). 
        /// NOTE: This only works for longitudinal projects.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">formEventMapping</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Contains the attributes 'arm_num' (referring to the arm number), 'unique_event_name' (referring to the auto-generated unique event name of the given event), and 'form' (referring to the unique form name of the given data collection instrument), in which they are provided in the specified format. 
        /// JSON Example:[{"arm_num":"1","unique_event_name":"baseline_arm_1","form":"demographics"},
        /// {"arm_num":"1","unique_event_name":"visit_1_arm_1","form":"day_3"},
        /// {"arm_num":"1","unique_event_name":"visit_1_arm_1","form":"other"},
        /// {"arm_num":"1","unique_event_name":"visit_2_arm_1","form":"other"}]
        /// </param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Number of Instrument-Event Mappings imported</returns>
        public async Task<string> ImportInstrumentMappingAsync<T>(string token, string content, ReturnFormat format, List<T> data, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "formEventMapping";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat },
                    { "data", _serializedData }
                };
                // Execute request
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Metadata (Data Dictionary)
        /// This method allows you to export the metadata for a project
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">metadata</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all metadata is pulled)</param>
        /// <param name="forms">an array of form names specifying specific data collection instruments for which you wish to pull metadata (by default, all metadata is pulled). NOTE: These 'forms' are not the form label values that are seen on the webpages, but instead they are the unique form names seen in Column B of the data dictionary.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        public async Task<string> ExportMetaDataAsync(string token, string content, ReturnFormat format, string[] fields = null, string[] forms = null, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);
                if (IsNullOrEmpty(content))
                {
                    content = "metadata";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat }
                };
                // Optional
                if (fields?.Length > 0)
                {
                    payload.Add("fields", await this.ConvertArraytoString(fields));
                }
                if (forms?.Length > 0)
                {
                    payload.Add("forms", await this.ConvertArraytoString(forms));
                }
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import Metadata (Data Dictionary)
        /// 
        /// This method allows you to import metadata (i.e., Data Dictionary) into a project. Notice: Because of this method's destructive nature, it is only available for use for projects in Development status.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">metadata</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="data">The formatted data to be imported.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Number of fields imported</returns>
        public async Task<string> ImportMetaDataAsync<T>(string token, string content, ReturnFormat format, List<T> data, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);
                if (IsNullOrEmpty(content))
                {
                    content = "metadata";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat },
                    { "data", _serializedData }
                };
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Create A New Project
        /// 
        /// This method allows you to create a new REDCap project. A 64-character Super API Token is required for this method (as opposed to project-level API methods that require a regular 32-character token associated with the project-user). In the API request, you must minimally provide the project attributes 'project_title' and 'purpose' (with numerical value 0=Practice/Just for fun, 1=Other, 2=Research, 3=Quality Improvement, 4=Operational Support) when creating a project. 
        /// When a project is created with this method, the project will automatically be given all the project-level defaults just as if you created a new empty project via the web user interface, such as a automatically creating a single data collection instrument seeded with a single Record ID field and Form Status field, as well as (for longitudinal projects) one arm with one event. And if you intend to create your own arms or events immediately after creating the project, it is recommended that you utilize the override=1 parameter in the 'Import Arms' or 'Import Events' method, respectively, so that the default arm and event are removed when you add your own.Also, the user creating the project will automatically be added to the project as a user with full user privileges and a project-level API token, which could then be used for subsequent project-level API requests.
        /// NOTE: Only users with Super API Tokens can utilize this method.Users can only be granted a super token by a REDCap administrator(using the API Tokens page in the REDCap Control Center). Please be advised that users with a Super API Token can create new REDCap projects via the API without any approval needed by a REDCap administrator.If you are interested in obtaining a super token, please contact your local REDCap administrator.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have a Super API Token.
        /// </remarks>
        /// To use this method, you must have a Super API Token.
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The Super API Token specific to a user</param>
        /// <param name="content">project</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="data">
        /// Contains the attributes of the project to be created, in which they are provided in the specified format. While the only required attributes are 'project_title' and 'purpose', the fields listed below are all the possible attributes that can be provided in the 'data' parameter. The 'purpose' attribute must have a numerical value (0=Practice/Just for fun, 1=Other, 2=Research, 3=Quality Improvement, 4=Operational Support), in which 'purpose_other' is only required to have a value (as a text string) if purpose=1. The attributes is_longitudinal (0=False, 1=True; Default=0), surveys_enabled (0=False, 1=True; Default=0), and record_autonumbering_enabled (0=False, 1=True; Default=1) are all boolean. Please note that either is_longitudinal=1 or surveys_enabled=1 does not add arms/events or surveys to the project, respectively, but it merely enables those settings which are seen at the top of the project's Project Setup page.
        /// All available attributes:
        /// project_title, purpose, purpose_other, project_notes, is_longitudinal, surveys_enabled, record_autonumbering_enabled
        /// JSON Example:
        /// [{"project_title":"My New REDCap Project","purpose":"0"}]
        /// </param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="odm">default: NULL - The 'odm' parameter must be an XML string in CDISC ODM XML format that contains project metadata (fields, forms, events, arms) and might optionally contain data to be imported as well. The XML contained in this parameter can come from a REDCap Project XML export file from REDCap itself, or may come from another system that is capable of exporting projects and data in CDISC ODM format. If the 'odm' parameter is included in the API request, it will use the XML to import its contents into the newly created project. This will allow you not only to create the project with the API request, but also to import all fields, forms, and project attributes (and events and arms, if longitudinal) as well as record data all at the same time.</param>
        /// <returns>When a project is created, a 32-character project-level API Token is returned (associated with both the project and user creating the project). This token could then ostensibly be used to make subsequent API calls to this project, such as for adding new events, fields, records, etc.</returns>
        public async Task<string> CreateProjectAsync<T>(string token, string content, ReturnFormat format, List<T> data, OnErrorFormat onErrorFormat = OnErrorFormat.json, string odm = null)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);
                if (IsNullOrEmpty(content))
                {
                    content = "project";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat },
                    { "data", _serializedData }
                };
                if (!IsNullOrEmpty(odm))
                {
                    payload.Add("odm", odm);
                }
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import Project Information
        /// This method allows you to update some of the basic attributes of a given REDCap project, such as the project's title, if it is longitudinal, if surveys are enabled, etc. Its data format corresponds to the format in the API method Export Project Information. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges in the project.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">project_settings</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="data">Contains some or all of the attributes from Export Project Information in the same data format as in the export. These attributes will change the project information.
        /// Attributes for the project in the format specified. For any values that are boolean, they should be represented as either a '0' (no/false) or '1' (yes/true). The following project attributes can be udpated:
        /// project_title, project_language, purpose, purpose_other, project_notes, custom_record_label, secondary_unique_field, is_longitudinal, surveys_enabled, scheduling_enabled, record_autonumbering_enabled, randomization_enabled, project_irb_number, project_grant_number, project_pi_firstname, project_pi_lastname, display_today_now_button
        /// </param>
        /// <returns>Returns the number of values accepted to be updated in the project settings (including values which remained the same before and after the import).</returns>
        public async Task<string> ImportProjectInfoAsync<T>(string token, string content, ReturnFormat format, List<T> data)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, null);
                if (IsNullOrEmpty(content))
                {
                    content = "project_settings";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat },
                    { "data", _serializedData }
                };
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Project Information
        /// This method allows you to export some of the basic attributes of a given REDCap project, such as the project's title, if it is longitudinal, if surveys are enabled, the time the project was created and moved to production, etc.
        /// </summary>
        /// <remarks>
        /// 
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">project</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>
        /// Attributes for the project in the format specified. For any values that are boolean, they will be represented as either a '0' (no/false) or '1' (yes/true). Also, all date/time values will be returned in Y-M-D H:M:S format. The following attributes will be returned:
        /// project_id, project_title, creation_time, production_time, in_production, project_language, purpose, purpose_other, project_notes, custom_record_label, secondary_unique_field, is_longitudinal, surveys_enabled, scheduling_enabled, record_autonumbering_enabled, randomization_enabled, ddp_enabled, project_irb_number, project_grant_number, project_pi_firstname, project_pi_lastname, display_today_now_button
        /// </returns>
        public async Task<string> ExportProjectInfoAsync(string token, string content, ReturnFormat format, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);
                if (IsNullOrEmpty(content))
                {
                    content = "project";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat }
                };
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Entire Project as REDCap XML File (containing metadata and data)
        /// The entire project(all records, events, arms, instruments, fields, and project attributes) can be downloaded as a single XML file, which is in CDISC ODM format(ODM version 1.3.1). This XML file can be used to create a clone of the project(including its data, optionally) on this REDCap server or on another REDCap server (it can be uploaded on the Create New Project page). Because it is in CDISC ODM format, it can also be used to import the project into another ODM-compatible system. NOTE: All the option paramters listed below ONLY apply to data returned if the 'returnMetadataOnly' parameter is set to FALSE (default). For this API method, ALL metadata (all fields, forms, events, and arms) will always be exported.Only the data returned can be filtered using the optional parameters.
        /// Note about export rights: If the 'returnMetadataOnly' parameter is set to FALSE, then please be aware that Data Export user rights will be applied to any data returned from this API request. For example, if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project. 
        /// </summary>
        /// <remarks>
        /// 
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">project_xml</param>
        /// <param name="returnMetadataOnly">true, false [default] - TRUE returns only metadata (all fields, forms, events, and arms), whereas FALSE returns all metadata and also data (and optionally filters the data according to any of the optional parameters provided in the request) </param>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <param name="exportFiles">true, false [default] - TRUE will cause the XML returned to include all files uploaded for File Upload and Signature fields for all records in the project, whereas FALSE will cause all such fields not to be included. NOTE: Setting this option to TRUE can make the export very large and may prevent it from completing if the project contains many files or very large files. </param>
        /// <returns>The entire REDCap project's metadata (and data, if specified) will be returned in CDISC ODM format as a single XML string.</returns>
        public async Task<string> ExportProjectXmlAsync(string token, string content, bool returnMetadataOnly = false, string[] records = null, string[] fields = null, string[] events = null, OnErrorFormat onErrorFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false)
        {
            try
            {
                /*
                 * Check for presence of token
                 */
                this.CheckToken(token);
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);
                if (IsNullOrEmpty(content))
                {
                    content = "project_xml";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "returnFormat", _onErrorFormat }
                };
                // Optional
                if (returnMetadataOnly)
                {
                    payload.Add("returnMetadataOnly", returnMetadataOnly.ToString());
                }
                if (records?.Length > 0)
                {
                    payload.Add("records", await this.ConvertArraytoString(records));
                }
                if (events?.Length > 0)
                {
                    payload.Add("events", await this.ConvertArraytoString(events));
                }
                if (exportSurveyFields)
                {
                    payload.Add("exportSurveyFields", exportSurveyFields.ToString());
                }
                if (exportDataAccessGroups)
                {
                    payload.Add("exportDataAccessGroups", exportDataAccessGroups.ToString());
                }
                if (!IsNullOrEmpty(filterLogic))
                {
                    payload.Add("filterLogic", filterLogic);
                }
                if (exportFiles)
                {
                    payload.Add("exportFiles", exportFiles.ToString());
                }
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Generate Next Record Name
        /// To be used by projects with record auto-numbering enabled, this method exports the next potential record ID for a project. It generates the next record name by determining the current maximum numerical record ID and then incrementing it by one.
        /// Note: This method does not create a new record, but merely determines what the next record name would be.
        /// If using Data Access Groups (DAGs) in the project, this method accounts for the special formatting of the record name for users in DAGs (e.g., DAG-ID); in this case, it only assigns the next value for ID for all numbers inside a DAG. For example, if a DAG has a corresponding DAG number of 223 wherein records 223-1 and 223-2 already exist, then the next record will be 223-3 if the API user belongs to the DAG that has DAG number 223. (The DAG number is auto-assigned by REDCap for each DAG when the DAG is first created.) When generating a new record name in a DAG, the method considers all records in the entire project when determining the maximum record ID, including those that might have been originally created in that DAG but then later reassigned to another DAG.
        /// Note: This method functions the same even for projects that do not have record auto-numbering enabled.
        /// </summary>
        /// 
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">generateNextRecordName</param>
        /// <returns>The maximum integer record ID + 1.</returns>
        public async Task<string> GenerateNextRecordNameAsync(string token, string content)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                if (IsNullOrEmpty(content))
                {
                    content = "generateNextRecordName";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                };
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Records
        /// This method allows you to export a set of records for a project.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API data export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="format">csv, json [default], xml, odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="rawOrLabel">raw [default], label - export the raw coded values or labels for the options of multiple choice fields</param>
        /// <param name="rawOrLabelHeaders">raw [default], label - (for 'csv' format 'flat' type only) for the CSV headers, export the variable/field names (raw) or the field labels (label)</param>
        /// <param name="exportCheckboxLabel">true, false [default] - specifies the format of checkbox field values specifically when exporting the data as labels (i.e., when rawOrLabel=label) in flat format (i.e., when type=flat). When exporting labels, by default (without providing the exportCheckboxLabel flag or if exportCheckboxLabel=false), all checkboxes will either have a value 'Checked' if they are checked or 'Unchecked' if not checked. But if exportCheckboxLabel is set to true, it will instead export the checkbox value as the checkbox option's label (e.g., 'Choice 1') if checked or it will be blank/empty (no value) if not checked. If rawOrLabel=false or if type=eav, then the exportCheckboxLabel flag is ignored. (The exportCheckboxLabel parameter is ignored for type=eav because 'eav' type always exports checkboxes differently anyway, in which checkboxes are exported with their true variable name (whereas the 'flat' type exports them as variable___code format), and another difference is that 'eav' type *always* exports checkbox values as the choice label for labels export, or as 0 or 1 (if unchecked or checked, respectively) for raw export.)</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<string> ExportRecordsAsync(string token, string content, ReturnFormat format, RedcapDataType redcapDataType, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, OnErrorFormat onErrorFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat, redcapDataType);

                // Handle formats
                if (IsNullOrEmpty(content))
                {
                    content = "record";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat },
                    { "type", _redcapDataType }
                };

                // Optional
                if (records?.Length > 0)
                {
                    payload.Add("records", await this.ConvertArraytoString(records));
                }
                if (fields?.Length > 0)
                {
                    payload.Add("fields", await this.ConvertArraytoString(fields));
                }
                if (forms?.Length > 0)
                {
                    payload.Add("forms", await this.ConvertArraytoString(forms));
                }
                if (events?.Length > 0)
                {
                    payload.Add("events", await this.ConvertArraytoString(events));
                }
                /*
                 * Pertains to CSV data only
                 */
                var _rawOrLabelValue = rawOrLabelHeaders.ToString();
                if (!IsNullOrEmpty(_rawOrLabelValue))
                {
                    payload.Add("rawOrLabel", _rawOrLabelValue);
                }
                // Optional (defaults to false)
                if (exportCheckboxLabel)
                {
                    payload.Add("exportCheckboxLabel", exportCheckboxLabel.ToString());
                }
                // Optional (defaults to false)
                if (exportSurveyFields)
                {
                    payload.Add("exportSurveyFields", exportSurveyFields.ToString());
                }
                // Optional (defaults to false)
                if (exportDataAccessGroups)
                {
                    payload.Add("exportDataAccessGroups", exportDataAccessGroups.ToString());
                }
                if (!IsNullOrEmpty(filterLogic))
                {
                    payload.Add("filterLogic", filterLogic);
                }
                return await this.SendRequestAsync(payload, _uri);

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

        /// <summary>
        /// API Version 1.0.0
        /// Import Records
        /// This method allows you to import a set of records for a project
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges in the project.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="format">csv, json [default], xml, odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default]
        /// eav - input as one data point per row
        /// Non-longitudinal: Will have the fields - record*, field_name, value
        /// Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name
        /// </param>
        /// <param name="overwriteBehavior">
        /// normal - blank/empty values will be ignored [default]
        /// overwrite - blank/empty values are valid and will overwrite data</param>
        /// <param name="forceAutoNumber">If record auto-numbering has been enabled in the project, it may be desirable to import records where each record's record name is automatically determined by REDCap (just as it does in the user interface). If this parameter is set to 'true', the record names provided in the request will not be used (although they are still required in order to associate multiple rows of data to an individual record in the request), but instead those records in the request will receive new record names during the import process. NOTE: To see how the provided record names get translated into new auto record names, the returnContent parameter should be set to 'auto_ids', which will return a record list similar to 'ids' value, but it will have the new record name followed by the provided record name in the request, in which the two are comma-delimited. For example, if 
        /// false (or 'false') - The record names provided in the request will be used. [default]
        /// true (or 'true') - New record names will be automatically determined.</param>
        /// <param name="data">The formatted data to be imported.
        /// NOTE: When importing data in EAV type format, please be aware that checkbox fields must have their field_name listed as variable+'___'+optionCode and its value as either '0' or '1' (unchecked or checked, respectively). For example, for a checkbox field with variable name 'icecream', it would be imported as EAV with the field_name as 'icecream___4' having a value of '1' in order to set the option coded with '4' (which might be 'Chocolate') as 'checked'.</param>
        /// <param name="dateFormat">MDY, DMY, YMD [default] - the format of values being imported for dates or datetime fields (understood with M representing 'month', D as 'day', and Y as 'year') - NOTE: The default format is Y-M-D (with dashes), while MDY and DMY values should always be formatted as M/D/Y or D/M/Y (with slashes), respectively.</param>
        /// <param name="returnContent">count [default] - the number of records imported, ids - a list of all record IDs that were imported, auto_ids = (used only when forceAutoNumber=true) a list of pairs of all record IDs that were imported, includes the new ID created and the ID value that was sent in the API request (e.g., 323,10). </param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>the content specified by returnContent</returns>
        public async Task<string> ImportRecordsAsync<T>(string token, string content, ReturnFormat format, RedcapDataType redcapDataType, OverwriteBehavior overwriteBehavior, bool forceAutoNumber, List<T> data, string dateFormat, ReturnContent returnContent = ReturnContent.count, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat, redcapDataType);

                if (IsNullOrEmpty(content))
                {
                    content = "record";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "type", _redcapDataType },
                    { "overwriteBehavior", overwriteBehavior.ToString() },
                    { "forceAutoNumber", forceAutoNumber.ToString()},
                    { "data", _serializedData},
                    { "returnFormat", _onErrorFormat }
                };
                // Optional
                if (!IsNullOrEmpty(dateFormat))
                {
                    payload.Add("dateFormat", dateFormat);
                }
                if (!IsNullOrEmpty(returnContent.ToString()))
                {
                    payload.Add("returnContent", returnContent.ToString());
                }
                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Delete Records
        /// This method allows you to delete one or more records from a project in a single API request.
        /// </summary>
        /// <remarks>
        /// 
        /// To use this method, you must have 'Delete Record' user privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="action">delete</param>
        /// <param name="records">an array of record names specifying specific records you wish to delete</param>
        /// <param name="arm">the arm number of the arm in which the record(s) should be deleted. 
        /// (This can only be used if the project is longitudinal with more than one arm.) NOTE: If the arm parameter is not provided, the specified records will be deleted from all arms in which they exist. Whereas, if arm is provided, they will only be deleted from the specified arm. </param>
        /// <returns>the number of records deleted.</returns>
        public async Task<string> DeleteRecordsAsync(string token, string content, string action, string[] records, int? arm)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                if (IsNullOrEmpty(content))
                {
                    content = "record";
                }
                if (IsNullOrEmpty(action))
                {
                    action = "delete";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "action",  action}
                };
                // Required
                payload.Add("records", await this.ConvertArraytoString(records));

                // Optional
                payload.Add("arm", arm?.ToString());

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// Export Reports
        /// This method allows you to export the data set of a report created on a project's 'Data Exports, Reports, and Stats' page.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API report export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// Also, please note the the 'Export Reports' method does *not* make use of the 'type' (flat/eav) parameter, which can be used in the 'Export Records' method.All data for the 'Export Reports' method is thus exported in flat format.If the 'type' parameter is supplied in the API request, it will be ignored.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">report</param>
        /// <param name="reportId">the report ID number provided next to the report name on the report list page</param>
        /// <param name="format">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="rawOrLabel">raw [default], label - export the raw coded values or labels for the options of multiple choice fields</param>
        /// <param name="rawOrLabelHeaders">raw [default], label - (for 'csv' format 'flat' type only) for the CSV headers, export the variable/field names (raw) or the field labels (label)</param>
        /// <param name="exportCheckboxLabel">true, false [default] - specifies the format of checkbox field values specifically when exporting the data as labels (i.e., when rawOrLabel=label). When exporting labels, by default (without providing the exportCheckboxLabel flag or if exportCheckboxLabel=false), all checkboxes will either have a value 'Checked' if they are checked or 'Unchecked' if not checked. But if exportCheckboxLabel is set to true, it will instead export the checkbox value as the checkbox option's label (e.g., 'Choice 1') if checked or it will be blank/empty (no value) if not checked. If rawOrLabel=false, then the exportCheckboxLabel flag is ignored.</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        public async Task<string> ExportReportsAsync(string token, string content, int reportId, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "report";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "report_id", reportId.ToString() },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat }
                };
                // Optional
                var _rawOrLabel = rawOrLabel.ToString();
                if (!IsNullOrEmpty(_rawOrLabel))
                {
                    payload.Add("rawOrLabel", _rawOrLabel);
                }
                var _rawOrLabelHeaders = rawOrLabelHeaders.ToString();
                if (!IsNullOrEmpty(_rawOrLabelHeaders))
                {
                    payload.Add("rawOrLabelHeaders", _rawOrLabelHeaders);
                }
                if (exportCheckboxLabel)
                {
                    payload.Add("exportCheckboxLabel", exportCheckboxLabel.ToString());
                }

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export REDCap Version
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <remarks>
        /// 
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">version</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        public async Task<string> ExportRedcapVersionAsync(string token, string content, ReturnFormat format)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, null);

                if (IsNullOrEmpty(content))
                {
                    content = "version";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format }
                };

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export a Survey Link for a Participant
        /// This method returns a unique survey link (i.e., a URL) in plain text format for a specified record and data collection instrument (and event, if longitudinal) in a project. If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the specified data collection instrument has not been enabled as a survey in the project, an error will be returned.
        /// </summary>
        /// <remarks>
        /// 
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">surveyLink</param>
        /// <param name="record">the record ID. The name of the record in the project.</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. This instrument must be enabled as a survey in the project.</param>
        /// <param name="eventName">the unique event name (for longitudinal projects only).</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="onErrorFormat">csv, json [default], xml - The returnFormat is only used with regard to the format of any error messages that might be returned.</param>
        /// <returns>Returns a unique survey link (i.e., a URL) in plain text format for the specified record and instrument (and event, if longitudinal).</returns>
        public async Task<string> ExportSurveyLinkAsync(string token, string content, string record, string instrument, string eventName, int repeatInstance, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "surveyLink";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "record", record },
                    { "instrument", instrument },
                    { "event", eventName },
                    { "repeat_instance", repeatInstance.ToString() },
                    { "returnFormat", _onErrorFormat }
                };

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export a Survey Participant List
        /// This method returns the list of all participants for a specific survey instrument (and for a specific event, if a longitudinal project). If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the specified data collection instrument has not been enabled as a survey in the project, an error will be returned.
        /// </summary>
        /// <remarks>
        /// 
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">participantList</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. This instrument must be enabled as a survey in the project.</param>
        /// <param name="eventName">the unique event name (for longitudinal projects only).</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Returns the list of all participants for the specified survey instrument [and event] in the desired format. The following fields are returned: email, email_occurrence, identifier, invitation_sent_status, invitation_send_time, response_status, survey_access_code, survey_link. The attribute 'email_occurrence' represents the current count that the email address has appeared in the list (because emails can be used more than once), thus email + email_occurrence represent a unique value pair. 'invitation_sent_status' is '0' if an invitation has not yet been sent to the participant, and is '1' if it has. 'invitation_send_time' is the date/time in which the next invitation will be sent, and is blank if there is no invitation that is scheduled to be sent. 'response_status' represents whether the participant has responded to the survey, in which its value is 0, 1, or 2 for 'No response', 'Partial', or 'Completed', respectively. Note: If an incorrect event_id or instrument name is used or if the instrument has not been enabled as a survey, then an error will be returned.</returns>
        public async Task<string> ExportSurveyParticipantsAsync(string token, string content, string instrument, string eventName, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "participantList";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "instrument", instrument },
                    { "event", eventName },
                    { "returnFormat", _onErrorFormat }
                };

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export a Survey Queue Link for a Participant
        /// This method returns a unique Survey Queue link (i.e., a URL) in plain text format for the specified record in a project that is utilizing the Survey Queue feature. If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the Survey Queue feature has not been enabled in the project, an error will be
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">surveyQueueLink</param>
        /// <param name="record">the record ID. The name of the record in the project.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Returns a unique Survey Queue link (i.e., a URL) in plain text format for the specified record in the project.</returns>
        public async Task<string> ExportSurveyQueueLinkAsync(string token, string content, string record, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "surveyQueueLink";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "record", record },
                    { "returnFormat", _onErrorFormat }
                };

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// Export a Survey Return Code for a Participant
        /// This method returns a unique Return Code in plain text format for a specified record and data collection instrument (and event, if longitudinal) in a project. If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the specified data collection instrument has not been enabled as a survey in the project or does not have the 'Save & Return Later' feature enabled, an error will be returned.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">surveyReturnCode</param>
        /// <param name="record">the record ID. The name of the record in the project.</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. This instrument must be enabled as a survey in the project.</param>
        /// <param name="eventName">the unique event name (for longitudinal projects only).</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Returns a unique Return Code in plain text format for the specified record and instrument (and event, if longitudinal).</returns>
        public async Task<string> ExportSurveyReturnCodeAsync(string token, string content, string record, string instrument, string eventName, string repeatInstance, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(null, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "surveyReturnCode";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "record", record },
                    { "instrument", instrument },
                    { "event", eventName },
                    { "repeat_instance", repeatInstance.ToString()},
                    { "returnFormat", _onErrorFormat }
                };

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Export Users
        /// This method allows you to export the list of users for a project, including their user privileges and also email address, first name, and last name. Note: If the user has been assigned to a user role, it will return the user with the role's defined privileges. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">user</param>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>The method will return all the attributes below with regard to user privileges in the format specified. Please note that the 'forms' attribute is the only attribute that contains sub-elements (one for each data collection instrument), in which each form will have its own Form Rights value (see the key below to learn what each numerical value represents). Most user privilege attributes are boolean (0=No Access, 1=Access). Attributes returned:
        /// username, email, firstname, lastname, expiration, data_access_group, design, user_rights, data_access_groups, data_export, reports, stats_and_charts, manage_survey_participants, calendar, data_import_tool, data_comparison_tool, logging, file_repository, data_quality_create, data_quality_execute, api_export, api_import, mobile_app, mobile_app_download_data, record_create, record_rename, record_delete, lock_records_customization, lock_records, lock_records_all_forms, forms</returns>
        /// <example>
        /// KEY: Data Export: 0=No Access, 2=De-Identified, 1=Full Data Set
        /// Form Rights: 0=No Access, 2=Read Only, 1=View records/responses and edit records(survey responses are read-only), 3=Edit survey responses
        /// Other attribute values: 0=No Access, 1=Access.
        /// </example>
        public async Task<string> ExportUsersAsync(string token, string content, ReturnFormat format, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "user";
                }
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat }
                };

                return await this.SendRequestAsync(payload, _uri);
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

        /// <summary>
        /// API Version 1.0.0
        /// Import Users
        /// This method allows you to import new users into a project while setting their user privileges, or update the privileges of existing users in the project. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Import/Update privileges *and* User Rights privileges in the project.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">user</param>
        /// <param name="data">
        /// Contains the attributes of the user to be added to the project or whose privileges in the project are being updated, in which they are provided in the specified format. All values should be numerical with the exception of username, expiration, data_access_group, and forms. Please note that the 'forms' attribute is the only attribute that contains sub-elements (one for each data collection instrument), in which each form will have its own Form Rights value (see the key below to learn what each numerical value represents). Most user privilege attributes are boolean (0=No Access, 1=Access). Please notice the distinction between data_access_group (contains the unique DAG name for a user) and data_access_groups (denotes whether the user has access to the Data Access Groups page).
        /// Missing attributes: If a user is being added to a project in the API request, then any attributes not provided for a user in the request(including form-level rights) will automatically be given the minimum privileges(typically 0=No Access) for the attribute/privilege.However, if an existing user's privileges are being modified in the API request, then any attributes not provided will not be modified from their current value but only the attributes provided in the request will be modified.
        /// Data Export: 0=No Access, 2=De-Identified, 1=Full Data Set
        /// Form Rights: 0=No Access, 2=Read Only, 1=View records/responses and edit records(survey responses are read-only), 3=Edit survey responses
        /// Other attribute values: 0=No Access, 1=Access.
        /// All available attributes:
        /// username, expiration, data_access_group, design, user_rights, data_access_groups, data_export, reports, stats_and_charts, manage_survey_participants, calendar, data_import_tool, data_comparison_tool, logging, file_repository, data_quality_create, data_quality_execute, api_export, api_import, mobile_app, mobile_app_download_data, record_create, record_rename, record_delete, lock_records_customization, lock_records, lock_records_all_forms, forms
        /// </param>
        /// <example>
        /// JSON Example:
        /// [{"username":"harrispa","expiration":"","data_access_group":"","design":"1","user_rights":"1","data_access_groups":"1","data_export":"1","reports":"1","stats_and_charts":"1",
        /// "manage_survey_participants":"1","calendar":"1","data_import_tool":"1","data_comparison_tool":"1",
        /// "logging":"1","file_repository":"1","data_quality_create":"1","data_quality_execute":"1",
        /// "api_export":"1","api_import":"1","mobile_app":"1","mobile_app_download_data":"0","record_create":"1",
        /// "record_rename":"0","record_delete":"0","lock_records_all_forms":"0","lock_records":"0",
        /// "lock_records_customization":"0","forms":{"demographics":"1","day_3":"1","other":"1"}
        /// },{"username":"taylorr4","expiration":"2015-12-07","data_access_group":"","design":"0",
        /// "user_rights":"0","data_access_groups":"0","data_export":"2","reports":"1","stats_and_charts":"1",
        /// "manage_survey_participants":"1","calendar":"1","data_import_tool":"0",
        /// "data_comparison_tool":"0","logging":"0","file_repository":"1","data_quality_create":"0",
        /// "data_quality_execute":"0","api_export":"0","api_import":"0","mobile_app":"0",
        /// "mobile_app_download_data":"0","record_create":"1","record_rename":"0","record_delete":"0",
        /// "lock_records_all_forms":"0","lock_records":"0","lock_records_customization":"0",
        /// "forms":{"demographics":"1","day_3":"2","other":"0"}}]
        /// </example>
        /// <param name="format">csv, json [default], xml</param>
        /// <param name="onErrorFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Number of users added or updated</returns>
        public async Task<string> ImportUsersAsync<T>(string token, string content, List<T> data, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json)
        {
            try
            {
                /*
                 * Check the required parameters for empty or null
                 */
                if (IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Please provide a valid Redcap token.");
                }
                // Handle formats
                var (_format, _onErrorFormat, _redcapDataType) = await this.HandleFormat(format, onErrorFormat);

                if (IsNullOrEmpty(content))
                {
                    content = "user";
                }
                var _serializedData = JsonConvert.SerializeObject(data);
                var payload = new Dictionary<string, string>
                {
                    { "token", token },
                    { "content", content },
                    { "format", _format },
                    { "returnFormat", _onErrorFormat },
                    { "data", _serializedData }
                };

                return await this.SendRequestAsync(payload, _uri);
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
        #endregion API Version 1.0.0 End

        #region deprecated methods < version 1.0.0
        /// <summary>
        /// Export Arms
        /// </summary>
        /// <param name="inputFormat">test</param>
        /// <param name="returnFormat">test</param>
        public async Task<string> ExportArmsAsync(ReturnFormat inputFormat, OnErrorFormat returnFormat)
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
        public async Task<string> GetMetaDataAsync(ReturnFormat? inputFormat, OnErrorFormat? returnFormat)
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
        public async Task<string> GetMetaDataAsync(ReturnFormat? inputFormat, OnErrorFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
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
                    _fields = await this.ConvertArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    // Convert string array into String
                    _forms = await this.ConvertArraytoString(formsArray);
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
        public async Task<string> GetRecordAsync(string record, ReturnFormat inputFormat, OnErrorFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
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
                    _records = await this.ConvertArraytoString(inputRecords);
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
        public async Task<string> GetRecordAsync(string record, ReturnFormat inputFormat, RedcapDataType redcapDataType, OnErrorFormat returnFormat = OnErrorFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
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
                    payload.Add("records", await this.ConvertArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertArraytoString(_events));
                }

                return await this.SendRequestAsync(payload, _uri);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        public async Task<string> GetRecordsAsync(ReturnFormat inputFormat, OnErrorFormat returnFormat, RedcapDataType redcapDataType)
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
        public async Task<string> GetRedcapVersionAsync(ReturnFormat inputFormat, RedcapDataType redcapDataType)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, OnErrorFormat.json, redcapDataType);
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
        public async Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, ReturnFormat? inputFormat, RedcapDataType? redcapDataType, OnErrorFormat? returnFormat)
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
        public async Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, ReturnFormat? inputFormat, RedcapDataType? redcapDataType, OnErrorFormat? returnFormat, string dateFormat = "MDY")
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
        public async Task<string> SaveRecordsAsync(List<string> data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, ReturnFormat? inputFormat, RedcapDataType? redcapDataType, OnErrorFormat? returnFormat, string dateFormat = "MDY")
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
        public async Task<string> ImportRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, ReturnFormat? inputFormat, RedcapDataType? redcapDataType, OnErrorFormat? returnFormat, string dateFormat = "MDY")
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
        public async Task<string> ImportRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, ReturnFormat? inputFormat, RedcapDataType? redcapDataType, OnErrorFormat? returnFormat, string apiToken, string dateFormat = "MDY")
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
        public async Task<string> ExportMetaDataAsync(ReturnFormat? inputFormat, OnErrorFormat? returnFormat)
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
        public async Task<string> ExportMetaDataAsync(ReturnFormat? inputFormat, OnErrorFormat? returnFormat, char[] delimiters, string fields = "", string forms = "")
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
                    _fields = await this.ConvertArraytoString(fieldsArray);
                }
                if (!String.IsNullOrEmpty(forms))
                {
                    string[] formsArray = formsResult.ToArray();
                    // Convert string array into String
                    _forms = await this.ConvertArraytoString(formsArray);
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
        public async Task<string> ExportEventsAsync(ReturnFormat inputFormat, OnErrorFormat returnFormat = OnErrorFormat.json, int[] arms = null)
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
        public delegate Task<string> GetRedcapVersion(ReturnFormat inputFormat, RedcapDataType redcapDataType);
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
        public delegate Task<string> ExportRecord(string record, ReturnFormat inputFormat, RedcapDataType redcapDataType, OnErrorFormat returnFormat = OnErrorFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
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
        public delegate Task<string> ExportRecords(string record, ReturnFormat inputFormat, RedcapDataType redcapDataType, OnErrorFormat returnFormat = OnErrorFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
        public async Task<string> ImportArmsAsync<T>(List<T> data, Override overRide, ReturnFormat inputFormat, OnErrorFormat returnFormat)
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
        public async Task<string> ExportRecordAsync(string record, ReturnFormat inputFormat, RedcapDataType redcapDataType, OnErrorFormat returnFormat = OnErrorFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
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
                    payload.Add("records", await this.ConvertArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertArraytoString(_events));
                }

                return await this.SendRequestAsync(payload, _uri);
            }
            catch (Exception Ex)
            {
                Log.Error($"{Ex.Message}");
                return string.Empty;
            }
        }
        public async Task<string> ExportRecordsAsync(string records, ReturnFormat inputFormat, RedcapDataType redcapDataType, OnErrorFormat returnFormat = OnErrorFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
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
                    payload.Add("records", await this.ConvertArraytoString(_inputRecords));
                }
                // Optional
                if (fieldItems.Count > 0)
                {
                    var _fields = fieldItems.ToArray();
                    payload.Add("fields", await this.ConvertArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertArraytoString(_events));
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
        public async Task<string> ExportRecordsAsync(ReturnFormat inputFormat, RedcapDataType redcapDataType, OnErrorFormat returnFormat = OnErrorFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null)
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
                    payload.Add("fields", await this.ConvertArraytoString(_fields));
                }

                // Optional
                if (formItems.Count > 0)
                {
                    var _forms = formItems.ToArray();
                    payload.Add("forms", await this.ConvertArraytoString(_forms));
                }

                // Optional
                if (eventItems.Count > 0)
                {
                    var _events = eventItems.ToArray();
                    payload.Add("events", await this.ConvertArraytoString(_events));
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
        public async Task<string> ExportRedcapVersionAsync(ReturnFormat inputFormat, RedcapDataType redcapDataType)
        {
            try
            {
                string _responseMessage;
                var (_inputFormat, _returnFormat, _redcapDataType) = await this.HandleFormat(inputFormat, OnErrorFormat.json, redcapDataType);
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
        public async Task<string> ExportUsersAsync(ReturnFormat inputFormat, OnErrorFormat returnFormat = OnErrorFormat.json)
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
        public async Task<string> GetRecordsAsync(ReturnFormat inputFormat, OnErrorFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters)
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
        public async Task<string> ImportEventsAsync<T>(List<T> data, Override overRide, ReturnFormat inputFormat, OnErrorFormat returnFormat = OnErrorFormat.json)
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
        public async Task<string> ExportFileAsync(string record, string field, string eventName, string repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json)
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
        public async Task<string> ExportFileAsync(string record, string field, string eventName, string repeatInstance, string filePath = null, OnErrorFormat returnFormat = OnErrorFormat.json)
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
        public async Task<string> ImportFileAsync(string record, string field, string eventName, string repeatInstance, string fileName, string filePath, OnErrorFormat returnFormat = OnErrorFormat.json)
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
        public async Task<string> DeleteFileAsync(string record, string field, string eventName, string repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json)
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
        public async Task<string> ExportEventsAsync(ReturnFormat inputFormat, OnErrorFormat returnFormat = OnErrorFormat.json)
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