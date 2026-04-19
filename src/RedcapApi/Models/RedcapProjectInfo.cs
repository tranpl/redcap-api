using Newtonsoft.Json;

namespace Redcap.Models
{
    /// <summary>
    /// Minimum redcap project information when creating a project
    /// 1.  Project Title
    /// 2.  Purpose
    /// 
    /// </summary>
    public class RedcapProjectInfo
    {
        /// <summary>
        /// Project Identifier
        /// </summary>
        /// 
        [JsonProperty("project_id")]
        public string ProjectId { get; set; }
        /// <summary>
        /// Title of project
        /// </summary>
        /// 
        [JsonProperty("project_title")]
        public string ProjectTitle { get; set; }
        /// <summary>
        /// Purpose, i.e. 0, 1, 2, 3
        /// 0 = Pratice For Fun
        /// 1 = Other
        /// 2 = Research
        /// 3 = Quality Improvement
        /// 4 = Other
        /// </summary>
        /// 
        [JsonProperty("purpose")]
        public ProjectPurpose Purpose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("purpose_other")]
        public string PurposeOther { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("project_notes")]
        public string ProjectNotes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("project_language")]
        public string ProjectLanguage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("custom_record_label")]
        public string CustomRecordLabel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("secondary_unique_field")]
        public string SecondaryUniqueField { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        /// 
        [JsonProperty("is_longitudinal")]
        public int IsLongitudinal { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        /// 
        [JsonProperty("surveys_enabled")]
        public int SurveysEnabled { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        /// 
        [JsonProperty("scheduling_enabled")]
        public int SchedulingEnabled { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        /// 
        [JsonProperty("record_autonumbering_enabled")]
        public int RecordAutonumberingEnabled { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        /// 
        [JsonProperty("randomization_enabled")]
        public int RandomizationEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("project_irb_number")]
        public string ProjectIrbNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("project_grant_number")]
        public string ProjectGrantNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("project_pi_firstname")]
        public string ProjectPiFirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("project_pi_lastname")]
        public string ProjectPiLastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [JsonProperty("display_today_now_button")]
        public bool DisplayTodayNowButton { get; set; }
    }

}
