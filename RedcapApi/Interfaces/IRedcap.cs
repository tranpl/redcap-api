using System.Collections.Generic;
using System.Threading.Tasks;

using Redcap.Models;

namespace Redcap.Interfaces
{
    /// <summary>
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
        Task<string> ExportRecordsAsync(string token, Content content, ReturnFormat inputFormat, RedcapDataType redcapDataType, string[] records = null, string[] fields = null,  string[] forms = null,  string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, OnErrorFormat returnFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportBlankForGrayFormStatus = false);
        Task<string> ExportRecordsAsync(string token, ReturnFormat inputFormat, RedcapDataType redcapDataType, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, OnErrorFormat returnFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportBlankForGrayFormStatus = false);

        Task<string> ExportRecordAsync(string token, Content content, string record, ReturnFormat format = ReturnFormat.json, RedcapDataType redcapDataType = RedcapDataType.flat, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, OnErrorFormat onErrorFormat = OnErrorFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportBlankForGrayFormStatus = false);

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
