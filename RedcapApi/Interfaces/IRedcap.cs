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
        /// This method allows you to delete Events from a project. 
        /// Notice: Because of this method's destructive nature, it is only available for use for projects in Development status. 
        /// Additionally, please be aware that deleting an event will automatically delete any records/data that have been collected under that event (this is non-reversible data loss).
        /// </summary>
        /// <remarks>
        /// NOTE: This only works for longitudinal projects. 
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// 
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="overRide"></param>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <param name="apiToken"></param>
        /// <returns>Number of Events deleted</returns>
        Task<string> DeleteEventsAsync<T>(List<T> data, Override overRide, InputFormat inputFormat, ReturnFormat returnFormat, string apiToken = null);
        /// <summary>
        /// Export List of Export Field Names (i.e. variables used during exports and imports)
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
        /// Export List of Export Field Names
        /// </remarks>
        /// <param name="returnFormat">csv, json, xml [default] - The returnFormat is only used with regard to the format of any error messages that might be returned.</param>
        /// <param name="field">A field's variable name. By default, all fields are returned, but if field is provided, then it will only the export field name(s) for that field. If the field name provided is invalid, it will return an error.</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>Returns a list of the export/import-specific version of field names for all fields (or for one field, if desired) in a project in the format specified and ordered by their field order.</returns>
        Task<string> ExportFields(ReturnFormat returnFormat, string field = null, string apiToken = null);
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
        /// This method allows you to export a list of the data collection instruments for a project. 
        /// This includes their unique instrument name as seen in the second column of the Data Dictionary, as well as each instrument's corresponding instrument label, which is seen on a project's left-hand menu when entering data. The instruments will be ordered according to their order in the project.
        /// </summary>
        /// <remarks>
        /// Export Instruments (Data Entry Forms)
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="returnFormat">csv, json, xml [default] - The returnFormat is only used with regard to the format of any error messages that might be returned.</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>Instruments for the project in the format specified and will be ordered according to their order in the project.</returns>
        Task<string> ExportInstruments(ReturnFormat returnFormat, string apiToken = null);
        /// <summary>
        /// This method allows you to export a PDF file for any of the following: 1) a single data collection instrument (blank), 2) all instruments (blank), 3) a single instrument (with data from a single record), 4) all instruments (with data from a single record), or 5) all instruments (with data from ALL records). 
        /// This is the exact same PDF file that is downloadable from a project's data entry form in the web interface, and additionally, the user's privileges with regard to data exports will be applied here just like they are when downloading the PDF in the web interface (e.g., if they have de-identified data export rights, then it will remove data from certain fields in the PDF). 
        /// If the user has 'No Access' data export rights, they will not be able to use this method, and an error will be returned.
        /// </summary>
        /// <remarks>
        /// Export PDF file of Data Collection Instruments (either as blank or with data)
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="recordId">the record ID. The value is blank by default. If record is blank, it will return the PDF as blank (i.e. with no data). If record is provided, it will return a single instrument or all instruments containing data from that record only.</param>
        /// <param name="eventName">the unique event name - only for longitudinal projects. For a longitudinal project, if record is not blank and event is blank, it will return data for all events from that record. If record is not blank and event is not blank, it will return data only for the specified event from that record.</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. The value is blank by default, which returns all instruments. If record is not blank and instrument is blank, it will return all instruments for that record.</param>
        /// <param name="allRecord">[The value of this parameter does not matter and is ignored.] If this parameter is passed with any value, it will export all instruments (and all events, if longitudinal) with data from all records. Note: If this parameter is passed, the parameters record, event, and instrument will be ignored.</param>
        /// <param name="returnFormat">csv, json, xml [default] - The returnFormat is only used with regard to the format of any error messages that might be returned.</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>A PDF file containing one or all data collection instruments from the project, in which the instruments will be blank (no data), contain data from a single record, or contain data from all records in the project, depending on the parameters passed in the API request.</returns>
        Task<string> ExportPDFInstruments(string recordId = null, string eventName = null, string instrument = null, bool allRecord = false, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null);

        /// <summary>
        /// This method allows you to export the instrument-event mappings for a project (i.e., how the data collection instruments are designated for certain events in a longitudinal project).
        /// </summary>
        /// <remarks>
        /// Export Instrument-Event Mappings
        /// NOTE: This only works for longitudinal projects.
        /// </remarks>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="arms">an array of arm numbers that you wish to pull events for (by default, all events are pulled)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>Instrument-event mappings for the project in the format specified</returns>
        Task<string> ExportInstrumentMapping(InputFormat inputFormat = InputFormat.json, string[] arms = null, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null);
        /// <summary>
        /// This method allows you to import Instrument-Event Mappings into a project (this corresponds to the 'Designate Instruments for My Events' page in the project). 
        /// </summary>
        /// <remarks>
        /// Import Instrument-Event Mappings
        /// NOTE: This only works for longitudinal projects.
        /// To use this method, you must have API Import/Update privileges *and* Project Design/Setup privileges in the project.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Contains the attributes 'arm_num' (referring to the arm number), 'unique_event_name' (referring to the auto-generated unique event name of the given event), and 'form' (referring to the unique form name of the given data collection instrument), in which they are provided in the specified format. </param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>Number of Instrument-Event Mappings imported</returns>
        Task<string> ImportInstrumentMapping<T>(List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null);
        /// <summary>
        /// This method allows you to create a new REDCap project. A 64-character Super API Token is required for this method (as opposed to project-level API methods that require a regular 32-character token associated with the project-user). In the API request, you must minimally provide the project attributes 'project_title' and 'purpose' (with numerical value 0=Practice/Just for fun, 1=Other, 2=Research, 3=Quality Improvement, 4=Operational Support) when creating a project. 
        /// When a project is created with this method, the project will automatically be given all the project-level defaults just as if you created a new empty project via the web user interface, such as a automatically creating a single data collection instrument seeded with a single Record ID field and Form Status field, as well as (for longitudinal projects) one arm with one event. And if you intend to create your own arms or events immediately after creating the project, it is recommended that you utilize the override=1 parameter in the 'Import Arms' or 'Import Events' method, respectively, so that the default arm and event are removed when you add your own.Also, the user creating the project will automatically be added to the project as a user with full user privileges and a project-level API token, which could then be used for subsequent project-level API requests.
        /// NOTE: Only users with Super API Tokens can utilize this method.Users can only be granted a super token by a REDCap administrator(using the API Tokens page in the REDCap Control Center). Please be advised that users with a Super API Token can create new REDCap projects via the API without any approval needed by a REDCap administrator.If you are interested in obtaining a super token, please contact your local REDCap administrator.
        /// </summary>
        /// <remarks>
        /// To use this method, you must have a Super API Token.
        /// All available attributes:
        /// project_title, purpose, purpose_other, project_notes, is_longitudinal, surveys_enabled, record_autonumbering_enabled
        /// </remarks>
        /// <example>
        /// JSON Example:[{"project_title":"My New REDCap Project","purpose":"0"}]
        /// CSV Example:project_title,purpose,is_longitudinal "My New REDCap Project",0,1
        /// XML Example:<?xml version="1.0" encoding="UTF-8" ?>
        /// <item>
        /// <project_title>My New REDCap Project</project_title>
        /// <purpose>0</purpose>
        /// surveys_enabled>1</surveys_enabled>
        /// <record_autonumbering_enabled>0</record_autonumbering_enabled>
        /// </item>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="odm">default: NULL - The 'odm' parameter must be an XML string in CDISC ODM XML format that contains project metadata (fields, forms, events, arms) and might optionally contain data to be imported as well. The XML contained in this parameter can come from a REDCap Project XML export file from REDCap itself, or may come from another system that is capable of exporting projects and data in CDISC ODM format. If the 'odm' parameter is included in the API request, it will use the XML to import its contents into the newly created project. This will allow you not only to create the project with the API request, but also to import all fields, forms, and project attributes (and events and arms, if longitudinal) as well as record data all at the same time.</param>
        /// <param name="apiToken">The Super API Token specific to a user</param>
        /// <returns>When a project is created, a 32-character project-level API Token is returned (associated with both the project and user creating the project). This token could then ostensibly be used to make subsequent API calls to this project, such as for adding new events, fields, records, etc.</returns>
        Task<string> CreateProject<T>(List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, string odm = null, string apiToken = null);
        /// <summary>
        /// This method allows you to update some of the basic attributes of a given REDCap project, such as the project's title, if it is longitudinal, if surveys are enabled, etc. Its data format corresponds to the format in the API method Export Project Information. 
        /// </summary>
        /// <remarks>
        /// Import Project Information
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Contains some or all of the attributes from Export Project Information in the same data format as in the export. These attributes will change the project information.
        /// Attributes for the project in the format specified. For any values that are boolean, they should be represented as either a '0' (no/false) or '1' (yes/true). The following project attributes can be udpated:
        /// project_title, project_language, purpose, purpose_other, project_notes, custom_record_label, secondary_unique_field, is_longitudinal, surveys_enabled, scheduling_enabled, record_autonumbering_enabled, randomization_enabled, project_irb_number, project_grant_number, project_pi_firstname, project_pi_lastname, display_today_now_button
        /// </param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>Returns the number of values accepted to be updated in the project settings (including values which remained the same before and after the import).</returns>
        Task<string> ImportProjectInfo<T>(List<T> data, InputFormat inputFormat = InputFormat.json, string apiToken = null);

        /// <summary>
        /// This method allows you to export some of the basic attributes of a given REDCap project, such as the project's title, if it is longitudinal, if surveys are enabled, the time the project was created and moved to production, etc.
        /// </summary>
        /// <remarks>
        /// Export Project Information
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>Attributes for the project in the format specified. For any values that are boolean, they will be represented as either a '0' (no/false) or '1' (yes/true). Also, all date/time values will be returned in Y-M-D H:M:S format. The following attributes will be returned:
        /// project_id, project_title, creation_time, production_time, in_production, project_language, purpose, purpose_other, project_notes, custom_record_label, secondary_unique_field, is_longitudinal, surveys_enabled, scheduling_enabled, record_autonumbering_enabled, randomization_enabled, ddp_enabled, project_irb_number, project_grant_number, project_pi_firstname, project_pi_lastname, display_today_now_button
        /// </returns>
        Task<string> ExportProjectInfo(InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, string apiToken = null);

        /// <summary>
        /// The entire project(all records, events, arms, instruments, fields, and project attributes) can be downloaded as a single XML file, which is in CDISC ODM format(ODM version 1.3.1). This XML file can be used to create a clone of the project(including its data, optionally) on this REDCap server or on another REDCap server (it can be uploaded on the Create New Project page). Because it is in CDISC ODM format, it can also be used to import the project into another ODM-compatible system. NOTE: All the option paramters listed below ONLY apply to data returned if the 'returnMetadataOnly' parameter is set to FALSE (default). For this API method, ALL metadata (all fields, forms, events, and arms) will always be exported.Only the data returned can be filtered using the optional parameters.
        /// Note about export rights: If the 'returnMetadataOnly' parameter is set to FALSE, then please be aware that Data Export user rights will be applied to any data returned from this API request. For example, if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project. 
        /// </summary>
        /// <remarks>
        /// Export Entire Project as REDCap XML File (containing metadata & data)
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="exportSurveyFields">true, false [default] - specifies whether or not to export the survey identifier field (e.g., 'redcap_survey_identifier') or survey timestamp fields (e.g., instrument+'_timestamp') when surveys are utilized in the project. If you do not pass in this flag, it will default to 'false'. If set to 'true', it will return the redcap_survey_identifier field and also the survey timestamp field for a particular survey when at least one field from that survey is being exported. NOTE: If the survey identifier field or survey timestamp fields are imported via API data import, they will simply be ignored since they are not real fields in the project but rather are pseudo-fields.</param>
        /// <param name="exportDataAccessGroups">true, false [default] - specifies whether or not to export the 'redcap_data_access_group' field when data access groups are utilized in the project. If you do not pass in this flag, it will default to 'false'. NOTE: This flag is only viable if the user whose token is being used to make the API request is *not* in a data access group. If the user is in a group, then this flag will revert to its default value.</param>
        /// <param name="filterLogic">String of logic text (e.g., [age] > 30) for filtering the data to be returned by this API method, in which the API will only return the records (or record-events, if a longitudinal project) where the logic evaluates as TRUE. This parameter is blank/null by default unless a value is supplied. Please note that if the filter logic contains any incorrect syntax, the API will respond with an error message. </param>
        /// <param name="exportFiles">true, false [default] - TRUE will cause the XML returned to include all files uploaded for File Upload and Signature fields for all records in the project, whereas FALSE will cause all such fields not to be included. NOTE: Setting this option to TRUE can make the export very large and may prevent it from completing if the project contains many files or very large files. </param>
        /// <param name="returnMetaDataOnly">true, false [default] - TRUE returns only metadata (all fields, forms, events, and arms), whereas FALSE returns all metadata and also data (and optionally filters the data according to any of the optional parameters provided in the request) </param>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>The entire REDCap project's metadata (and data, if specified) will be returned in CDISC ODM format as a single XML string.</returns>
        Task<string> ExportProjectXml(ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null, bool exportFiles = false,  bool returnMetaDataOnly = false, string[] records = null, string[] fields = null, string[] events = null, string apiToken = null);

        /// <summary>
        /// To be used by projects with record auto-numbering enabled, this method exports the next potential record ID for a project. It generates the next record name by determining the current maximum numerical record ID and then incrementing it by one.
        /// Note: This method does not create a new record, but merely determines what the next record name would be.
        /// If using Data Access Groups (DAGs) in the project, this method accounts for the special formatting of the record name for users in DAGs (e.g., DAG-ID); in this case, it only assigns the next value for ID for all numbers inside a DAG. For example, if a DAG has a corresponding DAG number of 223 wherein records 223-1 and 223-2 already exist, then the next record will be 223-3 if the API user belongs to the DAG that has DAG number 223. (The DAG number is auto-assigned by REDCap for each DAG when the DAG is first created.) When generating a new record name in a DAG, the method considers all records in the entire project when determining the maximum record ID, including those that might have been originally created in that DAG but then later reassigned to another DAG.
        /// Note: This method functions the same even for projects that do not have record auto-numbering enabled.
        /// </summary>
        /// 
        /// <remarks>
        /// Generate Next Record Name
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="apiToken">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <returns>The maximum integer record ID + 1.</returns>
        Task<string> GenerateNextRecordName(string apiToken = null);
       
        /// <summary>
        /// This method allows you to export a set of records for a project
        /// </summary>
        /// <param name="records">an array of record names specifying specific records you wish to pull (by default, all records are pulled)</param>
        /// <param name="inputFormat">csv, json [default], xml, odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="redcapDataType">flat - output as one record per row [default], eav - output as one data point per row. Non-longitudinal: Will have the fields - record*, field_name, value. Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'xml'.</param>
        /// <param name="delimiters">char[] e.g [';',',']</param>
        /// <param name="forms">an array of form names you wish to pull records for. If the form name has a space in it, replace the space with an underscore (by default, all records are pulled)</param>
        /// <param name="events">an array of unique event names that you wish to pull records for - only for longitudinal projects</param>
        /// <param name="fields">an array of field names specifying specific fields you wish to pull (by default, all fields are pulled)</param>
        /// <returns></returns>
        Task<string> ExportRecordsAsync(string[] records, InputFormat inputFormat = InputFormat.json, RedcapDataType redcapDataType = RedcapDataType.flat, ReturnFormat returnFormat = ReturnFormat.json, char[] delimiters = null, string[] forms = null, string[] events = null, string[] fields = null);
        
        /// <summary>
        /// This method allows you to export a set of records for a project.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request. 
        /// For example, if you have 'No Access' data export rights in the project, then the API data export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.        
        /// </summary>
        /// <remarks>
        /// Export Records
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="format">csv, json [default], xml, odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
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
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> ExportRecordsAsync(string token, string content, InputFormat format = InputFormat.json, RedcapDataType redcapDatatype = RedcapDataType.flat, string[] records = null, string[] fields = null, string[] forms = null, string[] events = null, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false, ReturnFormat returnFormat = ReturnFormat.json, bool exportSurveyFields = false, bool exportDataAccessGroups = false, string filterLogic = null);
        
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
        /// This method allows you to delete one or more records from a project in a single API request.
        /// </summary>
        /// <remarks>
        /// Delete Records
        /// To use this method, you must have 'Delete Record' user privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">record</param>
        /// <param name="action">delete</param>
        /// <param name="records">an array of record names specifying specific records you wish to delete</param>
        /// <param name="arm">the arm number of the arm in which the record(s) should be deleted. 
        /// (This can only be used if the project is longitudinal with more than one arm.) NOTE: If the arm parameter is not provided, the specified records will be deleted from all arms in which they exist. Whereas, if arm is provided, they will only be deleted from the specified arm. </param>
        /// <returns>the number of records deleted.</returns>
        Task<string> DeleteRecords(string token, string content, string action, string[] records, int arm);
        
        /// <summary>
        /// This method allows you to export the data set of a report created on a project's 'Data Exports, Reports, and Stats' page.
        /// Note about export rights: Please be aware that Data Export user rights will be applied to this API request.For example, if you have 'No Access' data export rights in the project, then the API report export will fail and return an error. And if you have 'De-Identified' or 'Remove all tagged Identifier fields' data export rights, then some data fields *might* be removed and filtered out of the data set returned from the API. To make sure that no data is unnecessarily filtered out of your API request, you should have 'Full Data Set' export rights in the project.
        /// Also, please note the the 'Export Reports' method does *not* make use of the 'type' (flat/eav) parameter, which can be used in the 'Export Records' method.All data for the 'Export Reports' method is thus exported in flat format.If the 'type' parameter is supplied in the API request, it will be ignored.
        /// </summary>
        /// <remarks>
        /// Export Reports
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">report</param>
        /// <param name="reportId">the report ID number provided next to the report name on the report list page</param>
        /// <param name="inputFormat">csv, json, xml [default], odm ('odm' refers to CDISC ODM XML format, specifically ODM version 1.3.1)</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <param name="rawOrLabel">raw [default], label - export the raw coded values or labels for the options of multiple choice fields</param>
        /// <param name="rawOrLabelHeaders">raw [default], label - (for 'csv' format 'flat' type only) for the CSV headers, export the variable/field names (raw) or the field labels (label)</param>
        /// <param name="exportCheckboxLabel">true, false [default] - specifies the format of checkbox field values specifically when exporting the data as labels (i.e., when rawOrLabel=label). When exporting labels, by default (without providing the exportCheckboxLabel flag or if exportCheckboxLabel=false), all checkboxes will either have a value 'Checked' if they are checked or 'Unchecked' if not checked. But if exportCheckboxLabel is set to true, it will instead export the checkbox value as the checkbox option's label (e.g., 'Choice 1') if checked or it will be blank/empty (no value) if not checked. If rawOrLabel=false, then the exportCheckboxLabel flag is ignored.</param>
        /// <returns>Data from the project in the format and type specified ordered by the record (primary key of project) and then by event id</returns>
        Task<string> ExportReports(string token, string content, int reportId, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json, RawOrLabel rawOrLabel = RawOrLabel.raw, RawOrLabelHeaders rawOrLabelHeaders = RawOrLabelHeaders.raw, bool exportCheckboxLabel = false);

        /// <summary>
        /// This method returns the current REDCap version number as plain text (e.g., 4.13.18, 5.12.2, 6.0.0).
        /// </summary>
        /// <remarks>
        /// Export REDCap Version
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">version</param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <returns>The current REDCap version number (three numbers delimited with two periods) as plain text - e.g., 4.13.18, 5.12.2, 6.0.0</returns>
        Task<string> ExportRedcapVersionAsync(string token, string content, InputFormat inputFormat);

        /// <summary>
        /// This method returns a unique survey link (i.e., a URL) in plain text format for a specified record and data collection instrument (and event, if longitudinal) in a project. If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the specified data collection instrument has not been enabled as a survey in the project, an error will be returned.
        /// </summary>
        /// <remarks>
        /// Export a Survey Link for a Participant
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">surveyLink</param>
        /// <param name="record">the record ID. The name of the record in the project.</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. This instrument must be enabled as a survey in the project.</param>
        /// <param name="eventName">the unique event name (for longitudinal projects only).</param>
        /// <param name="repeatInstance">(only for projects with repeating instruments/events) The repeat instance number of the repeating event (if longitudinal) or the repeating instrument (if classic or longitudinal). Default value is '1'.</param>
        /// <param name="returnFormat">csv, json [default], xml - The returnFormat is only used with regard to the format of any error messages that might be returned.</param>
        /// <returns>Returns a unique survey link (i.e., a URL) in plain text format for the specified record and instrument (and event, if longitudinal).</returns>
        Task<string> ExportSurveyLink(string token, string content, string record, string instrument, string eventName, int repeatInstance, ReturnFormat returnFormat = ReturnFormat.json);
        
        /// <summary>
        /// This method returns the list of all participants for a specific survey instrument (and for a specific event, if a longitudinal project). If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the specified data collection instrument has not been enabled as a survey in the project, an error will be returned.
        /// </summary>
        /// <remarks>
        /// Export a Survey Participant List
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">participantList</param>
        /// <param name="instrument">the unique instrument name as seen in the second column of the Data Dictionary. This instrument must be enabled as a survey in the project.</param>
        /// <param name="eventName">the unique event name (for longitudinal projects only).</param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Returns the list of all participants for the specified survey instrument [and event] in the desired format. The following fields are returned: email, email_occurrence, identifier, invitation_sent_status, invitation_send_time, response_status, survey_access_code, survey_link. The attribute 'email_occurrence' represents the current count that the email address has appeared in the list (because emails can be used more than once), thus email + email_occurrence represent a unique value pair. 'invitation_sent_status' is '0' if an invitation has not yet been sent to the participant, and is '1' if it has. 'invitation_send_time' is the date/time in which the next invitation will be sent, and is blank if there is no invitation that is scheduled to be sent. 'response_status' represents whether the participant has responded to the survey, in which its value is 0, 1, or 2 for 'No response', 'Partial', or 'Completed', respectively. Note: If an incorrect event_id or instrument name is used or if the instrument has not been enabled as a survey, then an error will be returned.</returns>
        Task<string> ExportSurveyParticipants(string token, string content, string instrument, string eventName, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json);

        /// <summary>
        /// Export a Survey Queue Link for a Participant
        /// This method returns a unique Survey Queue link (i.e., a URL) in plain text format for the specified record in a project that is utilizing the Survey Queue feature. If the user does not have 'Manage Survey Participants' privileges, they will not be able to use this method, and an error will be returned. If the Survey Queue feature has not been enabled in the project, an error will be
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">surveyQueueLink</param>
        /// <param name="record">the record ID. The name of the record in the project.</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Returns a unique Survey Queue link (i.e., a URL) in plain text format for the specified record in the project.</returns>
        Task<string> ExportSurveyQueueLink(string token, string content, string record, ReturnFormat returnFormat = ReturnFormat.json);
        
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
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Returns a unique Return Code in plain text format for the specified record and instrument (and event, if longitudinal).</returns>
        Task<string> ExportSurveyReturnCode(string token, string content, string record, string instrument, string eventName, string repeatInstance, ReturnFormat returnFormat = ReturnFormat.json);
        
        /// <summary>
        /// Method exports redcap users for a specific project.
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <param name="returnFormat"></param>
        /// <returns></returns>
        Task<string> ExportUsersAsync(InputFormat inputFormat, ReturnFormat returnFormat = ReturnFormat.json);
        /// <summary>
        /// Export Users
        /// This method allows you to export the list of users for a project, including their user privileges and also email address, first name, and last name. Note: If the user has been assigned to a user role, it will return the user with the role's defined privileges. 
        /// </summary>
        /// <remarks>
        /// To use this method, you must have API Export privileges in the project.
        /// Data Export: 0=No Access, 2=De-Identified, 1=Full Data Set
        /// Form Rights: 0=No Access, 2=Read Only, 1=View records/responses and edit records(survey responses are read-only), 3=Edit survey responses
        /// Other attribute values: 0=No Access, 1=Access.
        /// </remarks>
        /// <param name="token">The API token specific to your REDCap project and username (each token is unique to each user for each project). See the section on the left-hand menu for obtaining a token for a given project.</param>
        /// <param name="content">user</param>
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>The method will return all the attributes below with regard to user privileges in the format specified. Please note that the 'forms' attribute is the only attribute that contains sub-elements (one for each data collection instrument), in which each form will have its own Form Rights value (see the key below to learn what each numerical value represents). Most user privilege attributes are boolean (0=No Access, 1=Access). Attributes returned:
        /// username, email, firstname, lastname, expiration, data_access_group, design, user_rights, data_access_groups, data_export, reports, stats_and_charts, manage_survey_participants, calendar, data_import_tool, data_comparison_tool, logging, file_repository, data_quality_create, data_quality_execute, api_export, api_import, mobile_app, mobile_app_download_data, record_create, record_rename, record_delete, lock_records_customization, lock_records, lock_records_all_forms, forms
        /// </returns>
        Task<string> ExportUsersAsync(string token, string content, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json);

        /// <summary>
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
        /// <param name="inputFormat">csv, json [default], xml</param>
        /// <param name="returnFormat">csv, json, xml - specifies the format of error messages. If you do not pass in this flag, it will select the default format for you passed based on the 'format' flag you passed in or if no format flag was passed in, it will default to 'json'.</param>
        /// <returns>Number of users added or updated</returns>
        Task<string> ImportUsers<T>(string token, string content,  List<T> data, InputFormat inputFormat = InputFormat.json, ReturnFormat returnFormat = ReturnFormat.json);

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
