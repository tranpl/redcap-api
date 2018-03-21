namespace Redcap.Models
{
    /// <summary>
    /// Minimum redcap project information when creating a project
    /// 1.  Project Title
    /// 2.  Purpose
    /// 
    /// </summary>
    public class RedcapProject
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
        public bool is_longitudinal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool surveys_enabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool record_autonumbering_enabled { get; set; }

    }

}
