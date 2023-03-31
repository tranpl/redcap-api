using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Enum for different redcap record status type
    /// </summary>
    public enum RedcapProjectStatus
    {
        /// <summary>
        /// Project is in Development status
        /// </summary>
        /// 
        [Display(Name = "Development")]
        [Description("Development")]
        Development = 0,
        /// <summary>
        /// Project is in Production status                     
        /// </summary>                    
        /// 
        [Display(Name = "Production")]
        [Description("Production")]
        Production = 1,

        /// <summary>
        /// Project is in Analysis/Cleanup status
        /// </summary>
        /// 
        [Display(Name = "Analysis/Cleanup")]
        [Description("Analysis/Cleanup")]
        Inactive = 2,
        /// <summary>
        ///  This has been deprecated
        /// </summary>
        /// 
        [Display(Name = "Archived")]
        [Description("Archived")]
        Archived = 3,
        /// <summary>
        ///  Project is completed, will not be shown on the Projects List
        /// </summary>
        /// 
        [Display(Name = "Completed")]
        [Description("Completed")]
        Completed = 4
    }

}
