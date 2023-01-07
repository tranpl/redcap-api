using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Redcap.Models;

namespace Redcap.Interfaces
{
    /// <summary>
    /// The REDCap API is a set of interfaces that allows applications 
    /// to connect to REDCap. Please read the provided documentations from REDCap for more details
    /// on what specific set of API are allowed to be executed.
    /// 
    /// Virginia Commonwealth University
    /// Author: Michael Tran tranpl@outlook.com, tranpl@vcu.edu
    /// </summary>
    public interface IRedcap
    {
        #region Arms
        Task<string> ExportArmsAsync(string token, ReturnFormat format = ReturnFormat.json, string[] arms = null, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> ExportArmsAsync(string token, Content content = Content.Arm, ReturnFormat format = ReturnFormat.json, string[] arms = null, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> ImportArmsAsync<T>(string token, Content content, Override overrideBhavior, RedcapAction action, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat);
        Task<string> ImportArmsAsync<T>(string token, Override overrideBhavior, RedcapAction action, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat);
        Task<string> DeleteArmsAsync(string token, Content content, RedcapAction action, string[] arms);
        Task<string> DeleteArmsAsync(string token, string[] arms);
        #endregion

        #region Data Access Groups
        Task<string> ExportDagsAsync(string token, Content content, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> ImportDagsAsync<T>(string token, Content content, RedcapAction action, ReturnFormat format, List<T> data, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> DeleteDagsAsync(string token, Content content, RedcapAction action, string[] dags);
        Task<string> SwitchDagAsync(string token, RedcapDag dag, Content content = Content.Dag, RedcapAction action = RedcapAction.Switch);
        Task<string> ExportUserDagAssignmentAsync(string token, Content content, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> ImportUserDagAssignmentAsync<T>(string token, Content content, RedcapAction action, ReturnFormat format, List<T> data, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        #endregion

        #region Events
        Task<string> ExportEventsAsync(string token, Content content = Content.Event, ReturnFormat inputFormat = ReturnFormat.json, string[] arms = null, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportEventsAsync(string token, ReturnFormat inputFormat = ReturnFormat.json, string[] arms = null, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ImportEventsAsync<T>(string token, Content content, RedcapAction action, Override overrideBhavior, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportEventsAsync<T>(string token, Override overrideBhavior, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> DeleteEventsAsync(string token, Content content, RedcapAction action, string[] events);
        Task<string> DeleteEventsAsync(string token, string[] events);
        #endregion

        #region Field Names
        Task<string> ExportFieldNamesAsync(string token, Content content = Content.ExportFieldNames, ReturnFormat inputFormat = ReturnFormat.json, string field = null, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportFieldNamesAsync(string token, ReturnFormat inputFormat = ReturnFormat.json, string field = null, OnErrorFormat returnFormat = OnErrorFormat.json);
        #endregion

        #region Files
        Task<string> ExportFileAsync(string token, Content content, RedcapAction action, string record, string field, string eventName, string repeatInstance = "1", OnErrorFormat returnFormat = OnErrorFormat.json, string filePath = null);
        Task<string> ExportFileAsync(string token, string record, string field, string eventName, string repeatInstance = "1", OnErrorFormat returnFormat = OnErrorFormat.json, string filePath = null);

        Task<string> ImportFileAsync(string token, Content content, RedcapAction action, string record, string field, string eventName, string repeatInstance, string fileName, string filePath, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportFileAsync(string token, string record, string field, string eventName, string repeatInstance, string fileName, string filePath, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> DeleteFileAsync(string token, Content content, RedcapAction action, string record, string field, string eventName, string repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> DeleteFileAsync(string token, string record, string field, string eventName, string repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json);
        #endregion

        #region File Repository
        Task<string> CreateFolderFileRepositoryAsync(string token, Content content = Content.FileRepository, RedcapAction action = RedcapAction.CreateFolder, string name = null, RedcapFormat format = RedcapFormat.json, string folderId = null, string dagId = null, string roleId = null, RedcapReturnFormat returnFormat = RedcapReturnFormat.json);
        Task<string> ExportFilesFoldersFileRepositoryAsync(string token, Content content = Content.FileRepository, RedcapAction action = RedcapAction.List, RedcapFormat format = RedcapFormat.json, string folderId = null, RedcapReturnFormat returnFormat = RedcapReturnFormat.json);
        Task<string> ExportFileFileRepositoryAsync(string token, Content content = Content.FileRepository, RedcapAction action = RedcapAction.Export, string docId = null, RedcapReturnFormat returnFormat = RedcapReturnFormat.json);
        Task<string> ImportFileRepositoryAsync(string token, Content content = Content.FileRepository, RedcapAction action = RedcapAction.Import, string file = null, string folderId = null, RedcapReturnFormat returnFormat = RedcapReturnFormat.json);
        Task<string> DeleteFileRepositoryAsync(string token, Content content = Content.FileRepository, RedcapAction action = RedcapAction.Delete, string docId = null, RedcapReturnFormat returnFormat = RedcapReturnFormat.json);
        #endregion File Repository
        #region Instruments
        Task<string> ExportInstrumentsAsync(string token, Content content = Content.Instrument, ReturnFormat inputFormat = ReturnFormat.json);
        Task<string> ExportInstrumentsAsync(string token, ReturnFormat inputFormat = ReturnFormat.json);

        Task<string> ExportPDFInstrumentsAsync(string token, Content content = Content.Pdf, string recordId = null, string eventName = null, string instrument = null, bool allRecord = false, bool compactDisplay = false, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportPDFInstrumentsAsync(string token, string recordId = null, string eventName = null, string instrument = null, bool allRecord = false, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ExportInstrumentMappingAsync(string token, Content content = Content.FormEventMapping, ReturnFormat inputFormat = ReturnFormat.json, string[] arms = null, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportInstrumentMappingAsync(string token,  ReturnFormat inputFormat = ReturnFormat.json, string[] arms = null, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ImportInstrumentMappingAsync<T>(string token, Content content, ReturnFormat inputFormat, List<T> data,  OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportInstrumentMappingAsync<T>(string token, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json);
        #endregion

        #region Logging
        Task<string> ExportLoggingAsync(string token, Content content, ReturnFormat format = ReturnFormat.json, LogType logType = LogType.All, string user = null, string record = null, string dag = null, string beginTime = null, string endTime = null, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        #endregion

        #region Metadata
        Task<string> ExportMetaDataAsync(string token, Content content, ReturnFormat inputFormat, string[] fields = null, string[] forms = null, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportMetaDataAsync(string token, ReturnFormat inputFormat, string[] fields = null, string[] forms = null, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ImportMetaDataAsync<T>(string token, Content content, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportMetaDataAsync<T>(string token, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json);
        #endregion

        #region Projects
        Task<string> CreateProjectAsync<T>(string token, Content content, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json, string odm = null);
        Task<string> CreateProjectAsync<T>(string token, ReturnFormat inputFormat, List<T> data, OnErrorFormat returnFormat = OnErrorFormat.json, string odm = null);

        Task<string> ImportProjectInfoAsync(string token, Content content, ReturnFormat inputFormat, RedcapProjectInfo projectInfo);
        Task<string> ImportProjectInfoAsync(string token, ReturnFormat inputFormat, RedcapProjectInfo projectInfo);

        Task<string> ExportProjectInfoAsync(string token, Content content = Content.Project, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportProjectInfoAsync(string token, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ExportProjectXmlAsync(string token, Content content = Content.MetaData, bool returnMetadataOnly = false, string[] records = null, string[] fields = null, string[] events = null, OnErrorFormat returnFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false);
        Task<string> ExportProjectXmlAsync(string token, bool returnMetadataOnly = false, string[] records = null, string[] fields = null, string[] events = null, OnErrorFormat returnFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false);
        #endregion

        #region Records
        /// <summary>
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
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <param name="dateRangeBegin">To return only records that have been created or modified *after* a given date/time, provide a timestamp in the format YYYY-MM-DD HH:MM:SS (e.g., '2017-01-01 00:00:00' for January 1, 2017 at midnight server time). If not specified, it will assume no begin time. </param>
        /// <param name="dateRangeEnd">To return only records that have been created or modified *before* a given date/time, provide a timestamp in the format YYYY-MM-DD HH:MM:SS (e.g., '2017-01-01 00:00:00' for January 1, 2017 at midnight server time). If not specified, it will use the current server time. </param>
        /// <param name="csvDelimiter">Set the delimiter used to separate values in the CSV data file (for CSV format only). Options include: comma ',' (default), 'tab', semi-colon ';', pipe '|', or caret '^'. Simply provide the value in quotes for this parameter.</param>
        /// <param name="decimalCharacter">If specified, force all numbers into same decimal format. You may choose to force all data values containing a decimal to have the same decimal character, which will be applied to all calc fields and number-validated text fields. Options include comma ',' or dot/full stop '.', but if left blank/null, then it will export numbers using the fields' native decimal format. Simply provide the value of either ',' or '.' for this parameter.</param>
        /// <param name="exportBlankForGrayFormStatus">true, false [default] - specifies whether or not to export blank values for instrument complete status fields that have a gray status icon. All instrument complete status fields having a gray icon can be exported either as a blank value or as "0" (Incomplete). Blank values are recommended in a data export if the data will be re-imported into a REDCap project.</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> ExportRecordsAsync(string token, Content content, RedcapFormat format, RedcapDataType redcapDataType, string[] records = null, string[] fields = null,  string[] forms = null,  string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, OnErrorFormat returnFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, DateTime? dateRangeBegin = null, DateTime? dateRangeEnd = null, string csvDelimiter = null, string decimalCharacter = null, bool exportBlankForGrayFormStatus = false);
        /// <summary>
        /// Export Records
        /// This method allows you to export a set of records for a project.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API data export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
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
        /// <param name="dateRangeBegin">To return only records that have been created or modified *after* a given date/time, provide a timestamp in the format YYYY-MM-DD HH:MM:SS (e.g., '2017-01-01 00:00:00' for January 1, 2017 at midnight server time). If not specified, it will assume no begin time. </param>
        /// <param name="dateRangeEnd">To return only records that have been created or modified *before* a given date/time, provide a timestamp in the format YYYY-MM-DD HH:MM:SS (e.g., '2017-01-01 00:00:00' for January 1, 2017 at midnight server time). If not specified, it will use the current server time. </param>
        /// <param name="csvDelimiter">Set the delimiter used to separate values in the CSV data file (for CSV format only). Options include: comma ',' (default), 'tab', semi-colon ';', pipe '|', or caret '^'. Simply provide the value in quotes for this parameter.</param>
        /// <param name="decimalCharacter">If specified, force all numbers into same decimal format. You may choose to force all data values containing a decimal to have the same decimal character, which will be applied to all calc fields and number-validated text fields. Options include comma ',' or dot/full stop '.', but if left blank/null, then it will export numbers using the fields' native decimal format. Simply provide the value of either ',' or '.' for this parameter.</param>
        /// <param name="exportBlankForGrayFormStatus">true, false [default] - specifies whether or not to export blank values for instrument complete status fields that have a gray status icon. All instrument complete status fields having a gray icon can be exported either as a blank value or as "0" (Incomplete). Blank values are recommended in a data export if the data will be re-imported into a REDCap project.</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> ExportRecordsAsync(string token, RedcapFormat inputFormat, RedcapDataType redcapDataType, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, OnErrorFormat returnFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, DateTime? dateRangeBegin = null, DateTime? dateRangeEnd = null, string csvDelimiter = null, string decimalCharacter = null, bool exportBlankForGrayFormStatus = false);

        Task<string> ImportRecordsAsync<T>(string token, Content content, ReturnFormat inputFormat, RedcapDataType redcapDataType, OverwriteBehavior overwriteBehavior, bool forceAutoNumber, List<T> data, string dateFormat = "", CsvDelimiter csvDelimiter = CsvDelimiter.tab, ReturnContent returnContent = ReturnContent.count, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportRecordsAsync<T>(string token, ReturnFormat inputFormat, RedcapDataType redcapDataType, OverwriteBehavior overwriteBehavior, bool forceAutoNumber, List<T> data, string dateFormat = "", CsvDelimiter csvDelimiter = CsvDelimiter.tab, ReturnContent returnContent = ReturnContent.count, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> DeleteRecordsAsync(string token, Content content, RedcapAction action, string[] records, int? arm);
        Task<string> DeleteRecordsAsync(string token, string[] records, int? arm);
        Task<string> RenameRecordAsync(string token, string record, string newRecordName, Content content, RedcapAction action, int? arm);
        Task<string> GenerateNextRecordNameAsync(string token, Content content = Content.GenerateNextRecordName);
        Task<string> GenerateNextRecordNameAsync(string token);
        #endregion

        #region Repeating Instruments and Events
        Task<string> ExportRepeatingInstrumentsAndEvents(string token, Content content = Content.RepeatingFormsEvents, ReturnFormat format = ReturnFormat.json);
        Task<string> ExportRepeatingInstrumentsAndEvents(string token, ReturnFormat format = ReturnFormat.json);
        Task<string> ImportRepeatingInstrumentsAndEvents<T>(string token, List<T> data, Content content = Content.RepeatingFormsEvents, ReturnFormat returnFormat = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        #endregion

        #region Export Reports
        Task<string> ExportReportsAsync(string token, Content content, int reportId, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false);
        Task<string> ExportReportsAsync(string token, int reportId, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false);
        #endregion

        #region REDCap
        Task<string> ExportRedcapVersionAsync(string token, Content content = Content.Version, ReturnFormat inputFormat = ReturnFormat.json);
        Task<string> ExportRedcapVersionAsync(string token, ReturnFormat inputFormat = ReturnFormat.json);
        #endregion

        #region Surveys
        Task<string> ExportSurveyLinkAsync(string token, Content content, string record, string instrument, string eventName, int repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportSurveyLinkAsync(string token, string record, string instrument, string eventName, int repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ExportSurveyParticipantsAsync(string token, Content content, string instrument, string eventName, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportSurveyParticipantsAsync(string token, string instrument, string eventName, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ExportSurveyQueueLinkAsync(string token, Content content, string record, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportSurveyQueueLinkAsync(string token, string record, OnErrorFormat returnFormat = OnErrorFormat.json);

        Task<string> ExportSurveyReturnCodeAsync(string token, Content content, string record, string instrument, string eventName, string repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportSurveyReturnCodeAsync(string token, string record, string instrument, string eventName, string repeatInstance, OnErrorFormat returnFormat = OnErrorFormat.json);
        #endregion

        #region Users & User Privileges
        Task<string> ExportUsersAsync(string token, Content content = Content.User, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ExportUsersAsync(string token, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportUsersAsync<T>(string token, Content content, List<T> data, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);
        Task<string> ImportUsersAsync<T>(string token, List<T> data, ReturnFormat inputFormat = ReturnFormat.json, OnErrorFormat returnFormat = OnErrorFormat.json);
        #endregion

        #region User Roles
        Task<string> ExportUserRolesAsync(string token, Content content = Content.UserRole, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> ImportUserRolesAsync<T>(string token, List<T> data, Content content = Content.UserRole, ReturnFormat format = ReturnFormat.json, OnErrorFormat onErrorFormat = OnErrorFormat.json);
        Task<string> DeleteUserRolesAsync(string token, List<string> roles, Content content = Content.UserRole, RedcapAction action = RedcapAction.Delete);
        #endregion
    }
}
