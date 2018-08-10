using System.ComponentModel;

namespace Redcap.Models
{
    /// <summary>
    /// Redcap Content Model
    /// </summary>
    public enum Content
    {
        /// <summary>
        /// Arm Content
        /// </summary>
        /// 
        [Description("arm")]
        Arm,
        /// <summary>
        /// Event Content
        /// </summary>
        /// 
        [Description("event")]
        Event,
        /// <summary>
        /// Export Field Names Content
        /// </summary>
        /// 
        [Description("exportFieldNames")]
        ExportFieldNames,
        /// <summary>
        /// File Content
        /// </summary>
        /// 
        [Description("file")]
        File,
        /// <summary>
        /// Meta Data Content
        /// </summary>
        /// 
        [Description("metadata")]
        MetaData,
        /// <summary>
        /// Instrument Content
        /// </summary>
        /// 
        [Description("instrument")]
        Instrument,
        /// <summary>
        /// Pdf Content
        /// </summary>
        /// 
        [Description("pdf")]
        Pdf,
        /// <summary>
        /// Form Event Mapping Content
        /// </summary>
        /// 
        [Description("pdf")]
        FormEventMapping,
        /// <summary>
        /// Project Content
        /// </summary>
        /// 
        [Description("project")]
        Project,
        /// <summary>
        /// Project Settings Content
        /// </summary>
        /// 
        [Description("project_settings")]
        ProjectSettings,
        /// <summary>
        /// Project Xml Content
        /// </summary>
        /// 
        [Description("project_xml")]
        ProjectXml,
        /// <summary>
        /// Record Content
        /// </summary>
        /// 
        [Description("record")]
        Record,
        /// <summary>
        /// Generate Next Record Name Content
        /// </summary>
        /// 
        [Description("generateNextRecordName")]
        GenerateNextRecordName,
        /// <summary>
        /// Repeating Forms Events Content
        /// </summary>
        /// 
        [Description("repeatingFormsEvents")]
        RepeatingFormsEvents,
        /// <summary>
        /// Report Content
        /// </summary>
        /// 
        [Description("report")]
        Report,
        /// <summary>
        /// Version Content
        /// </summary>
        /// 
        [Description("version")]
        Version,
        /// <summary>
        /// Survey Link Content
        /// </summary>
        /// 
        [Description("surveyLink")]
        SurveyLink,
        /// <summary>
        /// Participant List Content
        /// </summary>
        /// 
        [Description("participantList")]
        ParticipantList,
        /// <summary>
        /// Survey Queue Link Content
        /// </summary>
        /// 
        [Description("surveyQueueLink")]
        SurveyQueueLink,
        /// <summary>
        /// Survey Return Code Content
        /// </summary>
        /// 
        [Description("surveyReturnCode")]
        SurveyReturnCode,
        /// <summary>
        /// User Content
        /// </summary>
        /// 
        [Description("user")]
        User

    }
}
