using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Enum for different redcap project purpose type
    /// </summary>
    public enum RedcapProjectPurpose
    {
        /// <summary>
        /// Project in Pratice Mode
        /// </summary>
        /// 
        [Display(Name = "Pratice")]
        Pratice = 0,
        /// <summary>
        /// Project in Other Mode
        /// </summary>
        /// 
        [Display(Name = "Other")]
        Other = 1,
        /// <summary>
        /// Project in Research Mode
        /// </summary>
        /// 
        [Display(Name = "Research")]
        Research = 2,
        /// <summary>
        /// Project in QI Mode
        /// </summary>
        [Display(Name = "Quality Improvement")]
        QualityImprovement = 3,
        /// <summary>
        /// Project in Operational Support Mode
        /// </summary>
        [Display(Name = "Operational Support")]
        OperationalSupport = 4,
    }

}
