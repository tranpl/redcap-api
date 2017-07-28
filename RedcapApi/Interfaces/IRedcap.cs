using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redcap.Interfaces
{
    /// <summary>
    /// REDCAP API VERSION 7.2.2
    /// This interface defines available methods for REDCap API.
    /// The REDCap API is an interface that allows external applications 
    /// to connect to REDCap remotely, and is used for programmatically 
    /// retrieving or modifying data or settings within REDCap, such as performing 
    /// automated data imports/exports for a specified REDCap project. 
    /// Programmers can use the REDCap API to make applications, websites, apps, widgets, 
    /// and other projects that interact with REDCap. Programs talk to the REDCap API 
    /// over HTTP, the same protocol that your browser uses to visit and interact with web pages.
    /// 
    /// Author: Michael Tran tranpl@outlook.com
    /// </summary>
    public interface IRedcap
    {
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
        /// <param name="RedcapFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> GetRecordsAsync(RedcapFormat RedcapFormat, RedcapDataType redcapDataType, char[] delimiters);
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
        /// <param name="RedcapFormat">0 = JSON (default), 1 = CSV, 2 = XML</param>
        /// <param name="redcapDataType">0 = FLAT, 1 = EAV, 2 = NONLONGITUDINAL, 3 = LONGITUDINAL</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> GetRecordAsync(string record, RedcapFormat RedcapFormat, RedcapDataType redcapDataType, char[] delimiters);
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
        Task<string> GetMetaData(RedcapFormat? RedcapFormat, ReturnFormat? returnFormat, char[] delimiters, string fields = "", string forms = "");
        /// <summary>
        /// This method allows you to import a set of records for a project
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="returnFormat"></param>
        /// <param name="dateFormat"></param>
        /// <returns>Http Status Code</returns>
        Task<string> SaveRecordsAsync(object data, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat);
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
        Task<string> SaveRecordsAsync(object data, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, OverwriteBehavior? overwriteBehavior, string DateFormat = "MDY");
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
        Task<string> SaveRecordsAsync(List<string> data, RedcapFormat? redcapFormat, RedcapDataType? redcapDataType, ReturnFormat? returnFormat, OverwriteBehavior? overwriteBehavior, string DateFormat = "MDY");
        Task<string> ExportUsers();
    }
}
