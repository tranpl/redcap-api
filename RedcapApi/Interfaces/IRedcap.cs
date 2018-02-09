using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redcap.Models;

namespace Redcap.Interfaces
{
    /// <summary>
    /// REDCAP VERSION 7.4.11
    /// This interface defines available methods for REDCap API.
    /// The REDCap API is an interface that allows external applications 
    /// to connect to REDCap remotely, and is used for programmatically 
    /// retrieving or modifying data or settings within REDCap, such as performing 
    /// automated data imports/exports for a specified REDCap project. 
    /// Programmers can use the REDCap API to make applications, websites, apps, widgets, 
    /// and other projects that interact with REDCap. Programs talk to the REDCap API 
    /// over HTTP, the same protocol that your browser uses to visit and interact with web pages.
    /// 
    /// Virginia Commonwealth University
    /// Author: Michael Tran tranpl@outlook.com, tranpl@vcu.edu
    /// </summary>
    public interface IRedcap
    {
       
        /// <summary>
        /// This method allows you to export the Arms for a project.
        /// NOTE: This only works for longitudinal projects. E.g. Arms are only available in longitudinal projects.
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default]</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Arms for the project in the format specified</returns>
        Task<string> ExportArmsAsync(InputFormat inputFormat, ReturnFormat returnFormat);

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
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns>Number of Arms imported</returns>
        Task<string> ImportArmsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat);
        
        /// <summary>
        /// This method allows you to delete Arms from a project. Notice: Because of this method's destructive nature, it is only available for use for projects in Development status. Additionally, please be aware that deleting an arm also automatically deletes all events that belong to that arm, and will also automatically delete any records/data that have been collected under that arm (this is non-reversible data loss).
        /// NOTE: This only works for longitudinal projects. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> DeleteArmsAsync<T>(T data);
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <param name="arms"></param>
        /// <returns></returns>
        Task<string> ExportEventsAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json, int[] arms = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="overRide"></param>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        Task<string> ImportEventsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json);
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> DeleteEvents();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportFields();
        /// <summary>
        /// Method export a single file from a record within a project
        /// </summary>
        /// <param name="record"></param>
        /// <param name="field"></param>
        /// <param name="eventName"></param>
        /// <param name="repeatInstance"></param>
        /// <param name="filePath"></param>
        /// <param name="returnFormat"></param>
        /// <example>
        /// The MIME type of the file, along with the name of the file and its extension, can be found in the header of the returned response. Thus in order to determine these attributes of the file being exported, you will need to parse the response header. Example: content-type = application/vnd.openxmlformats-officedocument.wordprocessingml.document; name='FILE_NAME.docx'
        /// </example>
        /// <returns>the contents of the file</returns>
        Task<string> ExportFileAsync(string record, string field, string eventName, string repeatInstance, string filePath, ReturnFormat returnFormat = ReturnFormat.json);
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
        Task<string> ImportFileAsync(string record, string field, string eventName, string repeatInstance, string fileName, string filePath, ReturnFormat returnFormat = ReturnFormat.json);
        /// <summary>
        /// This method allows you to remove a document that has been attached to an individual record for a File Upload field. Please note that this method may also be used for Signature fields (i.e. File Upload fields with 'signature' validation type).
        /// </summary>
        /// <param name="record">the record ID</param>
        /// <param name="field">the name of the field that contains the file</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>String</returns>
        Task<string> DeleteFileAsync(string record, string field, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json);
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportInstruments();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportPdfInstrument();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ImportPdfInstrument();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> CreateProject();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ImportProjectInfo();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportProjectInfo();

        /// <summary>
        /// Export Entire Project as REDCap XML File (containing metadata & data)
        /// </summary>
        /// <remarks>
        /// The entire project(all records, events, arms, instruments, fields, and project attributes) can be downloaded as a single XML file, which is in CDISC ODM format(ODM version 1.3.1). This XML file can be used to create a clone of the project(including its data, optionally) on this REDCap server or on another REDCap server (it can be uploaded on the Create New Project page). Because it is in CDISC ODM format, it can also be used to import the project into another ODM-compatible system. NOTE: All the option paramters listed below ONLY apply to data returned if the 'returnMetadataOnly' parameter is set to FALSE (default). For this API method, ALL metadata (all fields, forms, events, and arms) will always be exported.Only the data returned can be filtered using the optional parameters.
        /// Note about export rights: If the 'returnMetadataOnly' parameter is set to FALSE, then please be aware that Data Export user rights will be applied to any data returned from this API request. For example, if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project. 
        /// </remarks>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <param name="exportFiles">true, false [default] - TRUE will cause the XML returned to include all files uploaded for File Upload and Signature fields for all records in the project, whereas FALSE will cause all such fields not to be included. NOTE: Setting this option to TRUE can make the export very large and may prevent it from completing if the project contains many files or very large files. </param>
        /// <param name="returnMetaDataOnly">true, false [default] - TRUE returns only metadata (all fields, forms, events, and arms), whereas FALSE returns all metadata and also data (and optionally filters the data according to any of the optional parameters provided in the request) </param>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <returns>The entire REDCap project's metadata (and data, if specified) will be returned in CDISC ODM format as a single XML string.</returns>
        Task<string> ExportProjectXml(ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false,  bool returnMetaDataOnly = false, string[] records = null, string[] fields = null, string[] events = null);

        /// <summary>
        /// Method generates the next record name
        /// </summary>
        /// 
        /// <remarks>
        /// To be used by projects with record auto-numbering enabled, this method exports the next potential record ID for a project. It generates the next record name by determining the current maximum numerical record ID and then incrementing it by one.
        /// Note: This method does not create a new record, but merely determines what the next record name would be.
        /// If using Data Access Groups (DAGs) in the project, this method accounts for the special formatting of the record name for users in DAGs (e.g., DAG-ID); in this case, it only assigns the next value for ID for all numbers inside a DAG. For example, if a DAG has a corresponding DAG number of 223 wherein records 223-1 and 223-2 already exist, then the next record will be 223-3 if the API user belongs to the DAG that has DAG number 223. (The DAG number is auto-assigned by REDCap for each DAG when the DAG is first created.) When generating a new record name in a DAG, the method considers all records in the entire project when determining the maximum record ID, including those that might have been originally created in that DAG but then later reassigned to another DAG.
        /// Note: This method functions the same even for projects that do not have record auto-numbering enabled.
        /// </remarks>
        /// <returns>The maximum integer record ID + 1.</returns>
        Task<string> GenerateNextRecordName();

        /// <summary>
        /// This method allows you to export a set of records for a project
        /// </summary>
        /// <remarks>
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request. For example, if you have 'No Access' data export rights in the project, then the API data export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// </remarks>
        /// 
        /// <param name="record">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> ExportRecordAsync(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
        
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
        Task<string> ExportRecordsAsync(string records, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);
        /// <summary>
        /// Redcap version 8.x+
        /// This method allows you to export a set of records for a project.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API data export will fail and return an error. 
        /// And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// </summary>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="format">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDatatype">
        /// flat - output as one record per row[default] 
        /// eav - output as one data point per row
        /// Non-longitudinal: Will have the fields - record*, field_name, value
        /// Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="rawOrLabel">raw [default], label - export the raw coded values or labels for the options of multiple choice fields</param>
        /// <param name="rawOrLabelHeaders">raw [default], label - (for 'csv' format 'flat' type only) for the CSV headers, export the variable/field names (raw) or the field labels (label)</param>
        /// <param name="exportCheckboxLabel">true, false [default] - specifies the format of checkbox field values specifically when exporting the data as labels (i.e., when rawOrLabel=label) in flat format (i.e., when type=flat). When exporting labels, by default (without providing the exportCheckboxLabel flag or if exportCheckboxLabel=false), all checkboxes will either have a value 'Checked' if they are checked or 'Unchecked' if not checked. But if exportCheckboxLabel is set to true, it will instead export the checkbox value as the checkbox option's label (e.g., 'Choice 1') if checked or it will be blank/empty (no value) if not checked. If rawOrLabel=false or if type=eav, then the exportCheckboxLabel flag is ignored. (The exportCheckboxLabel parameter is ignored for type=eav because 'eav' type always exports checkboxes differently anyway, in which checkboxes are exported with their true variable name (whereas the 'flat' type exports them as variable___code format), and another difference is that 'eav' type *always* exports checkbox values as the choice label for labels export, or as 0 or 1 (if unchecked or checked, respectively) for raw export.)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <returns></returns>
        Task<string> ExportRecordsAsync(string token, string content, InputFormat format = InputFormat.json, RedcapDataType redcapDatatype = RedcapDataType.flat, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, string rawOrLabel = "raw", string rawOrLabelHeaders = "raw", bool exportCheckboxLabel = false, ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null);
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
        Task<string> ImportRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY");
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> DeleteRecords();
        
        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        Task<string> ExportRedcapVersionAsync(InputFormat inputFormat, RedcapDataType redcapDataType);
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportSurveyLink();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportSurveyParticipants();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportSurveyQueueLink();
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        Task<string> ExportSurveyReturnCode();
        
        /// <summary>
        /// Method exports redcap users for a specific project.
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        Task<string> ExportUsersAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json);
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="arms"></param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        Task<string> ImportUsers(int[] arms, OverwriteBehavior overwriteBehavior, InputFormat inputFormat, ReturnFormat returnFormat);

        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        Task<string> GetRedcapVersionAsync(InputFormat inputFormat, RedcapDataType redcapDataType);

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
        /// <param name="delimiters"></param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> GetRecordsAsync(InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters);

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
        Task<string> GetRecordAsync(string record, InputFormat inputFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters);
        
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
        Task<string> GetRecordAsync(string record, InputFormat inputFormat, RedcapDataType redcapDataType, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string forms = null, string events = null, string fields = null);

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> ExportMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat);

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters"></param>
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> ExportMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "");

        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters"></param>
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> GetMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "");
        
        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> GetMetaDataAsync(InputFormat? inputFormat, ReturnFormat? returnFormat);

        /// <summary>
        /// This method allows you to import a set of records for a project.
        /// </summary>
        /// <param name="data">Object that contains the records to be saved.</param>
        /// <param name="returnContent">ids - a list of all record IDs that were imported, count [default] - the number of records imported</param>
        /// <param name="overwriteBehavior">0 = normal, 1 = overwrite</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <returns></returns>
        Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat);

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
        Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY");

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
        Task<string> SaveRecordsAsync(List<string> data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, InputFormat? inputFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string dateFormat = "MDY");
    }
}
