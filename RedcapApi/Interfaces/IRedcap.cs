using System.Collections.Generic;
using System.Threading.Tasks;
using Redcap.Models;

namespace Redcap.Interfaces
{
    /// <summary>
    /// REDCAP API VERSION 7.4.9
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
        /// <param name="content"></param>
        /// <param name="redcapFormat"></param>
        /// <param name="returnFormat"></param>
        /// <param name="arms"></param>
        /// <returns>Arms for the project in the format specified</returns>
        Task<string> ExportArmsAsync<T>(RedcapFormat redcapFormat, ReturnFormat returnFormat, List<T> arms);

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
        Task<string> DeleteArmsAsync<T>(List<T> data, Override overRide, RedcapFormat redcapFormat, ReturnFormat returnFormat);
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
        Task<string> DeleteArmsAsync<T>(T data);
        Task<string> ExportEvents(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ImportEvents(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> DeleteEvents(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportFields(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportFile(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ImportFile(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> DeleteFile(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportInstruments(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ImportPdfInstrument(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> CreateProject(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ImportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportProjectInfo(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportProjectXml(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> GenerateNextRecordName(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportRecords(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ImportRecords(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> DeleteRecords(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportRedcapVersion(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportSurveyLink(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportSurveyParticipants(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportSurveyQueueLink(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportSurveyReturnCode(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);
        Task<string> ExportUsers(RedcapFormat RedcapFormat, ReturnFormat returnFormat = ReturnFormat.json);
        Task<string> ImportUsers(int[] arms, OverwriteBehavior overwriteBehavior, RedcapFormat RedcapFormat, ReturnFormat returnFormat);

        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="type">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        Task<string> GetRedcapVersionAsync(RedcapFormat RedcapFormat, RedcapDataType redcapDataType);
        /// <summary>
        /// This method allows you to export a set of records for a project.
        /// Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have "No Access" data export rights in the project, then the 
        /// API data export will fail and return an error. And if you have "De-Identified" 
        /// or "Remove all tagged Identifier fields" data export rights, then some data 
        /// fields *might* be removed and filtered out of the data set returned from the API. 
        /// To make sure that no data is unnecessarily filtered out of your API request, 
        /// you should have "Full Data Set" export rights in the project.
        /// </summary>
        /// <param name="redcapFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> GetRecordsAsync(RedcapFormat redcapFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters);
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
        /// <param name="record"></param>
        /// <param name="redcapFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> GetRecordAsync(string record, RedcapFormat redcapFormat, ReturnFormat returnFormat, RedcapDataType redcapDataType, char[] delimiters);
        /// <summary>
        /// This method allows you to export the metadata for a project. 
        /// </summary>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> ExportMetaData(RedcapFormat? RedcapFormat, ReturnFormat? returnFormat);
        /// <summary>
        /// This method allows you to export the metadata for a project. Overload method. 
        /// </summary>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat"></param>
        /// <param name="fields">example: "firstName, lastName, age"</param>
        /// <param name="forms">example: "demographics, labs, administration"</param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> GetMetaDataAsync(RedcapFormat? RedcapFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "");
        /// <summary>
        /// This method allows you to export the metadata for a project. Overload method. 
        /// </summary>
        /// <param name="format">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="returnFormat"></param>
        /// <returns>Metadata from the project (i.e. Data Dictionary values) in the format specified ordered by the field order</returns>
        Task<string> GetMetaDataAsync(RedcapFormat? RedcapFormat, ReturnFormat? returnFormat);

        /// <summary>
        /// This method allows you to import a set of records for a project
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="returnFormat"></param>
        /// <param name="dateFormat"></param>
        /// <returns>Http Status Code</returns>
        Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat);
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
        /// <returns></returns>
        Task<string> SaveRecordsAsync(object data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string DateFormat = "MDY");
        /// <summary>
        /// Bulk Import
        /// </summary>
        /// <param name="data"></param>
        /// <param name="apiKey"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="returnFormat"></param>
        /// <param name="overwriteBehavior"></param>
        /// <param name="DateFormat"></param>
        /// <returns></returns>
        Task<string> SaveRecordsAsync(List<string> data, ReturnContent returnContent, OverwriteBehavior overwriteBehavior, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, string DateFormat = "MDY");
    }
}
