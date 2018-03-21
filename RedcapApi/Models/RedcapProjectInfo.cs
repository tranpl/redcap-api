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
        /// Title of project
        /// </summary>
        public string project_title { get; set; }
        /// <summary>
        /// Purpose, i.e. 0, 1, 2, 3
        /// 0 = Pratice For Fun
        /// 1 = Other
        /// 2 = Research
        /// 3 = Quality Improvement
        /// 4 = Other
        /// </summary>
        public ProjectPurpose purpose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string purpose_other { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project_notes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project_language { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string custom_record_label { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondary_unique_field { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        public int is_longitudinal { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        public int surveys_enabled { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        public int scheduling_enabled { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        public int record_autonumbering_enabled { get; set; }
        /// <summary>
        /// 1: True, 0: False
        /// </summary>
        public int randomization_enabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project_irb_number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project_grant_number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project_pi_firstname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project_pi_lastname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool display_today_now_button { get; set; }
    }

}
