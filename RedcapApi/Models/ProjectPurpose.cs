using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// raw [default], label - export the raw coded values or labels for the options of multiple choice fields
    /// </summary>
    public enum ProjectPurpose
    {
        /// <summary>
        /// Pratice 
        /// </summary>
        /// 
        [Display(Name = "Pratice or For Fun")]
        [Description("Pratice or For Fun")]
        Pratice_ForFun = 0,

        /// <summary>
        /// Other
        /// </summary>
        /// 
        [Display(Name = "Other")]
        [Description("Other")]
        Other = 1,

        /// <summary>
        /// Research
        /// </summary>
        /// 
        [Display(Name = "Research")]
        [Description("Research")]
        Research = 2,

        /// <summary>
        /// Quality Improvement
        /// </summary>
        /// 
        [Display(Name = "Quality Improvement")]
        [Description("Quality Improvement")]
        QualityImprovement = 3,

        /// <summary>
        /// Other
        /// </summary>
        /// 
        [Display(Name = "Operational Support")]
        [Description("Operational Support")]
        OperationalSupport = 4
    }
}
