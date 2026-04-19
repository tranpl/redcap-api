using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{

    /// <summary>
    /// If specified, force all numbers into same decimal format. 
    /// You may choose to force all data values containing a decimal to have the same decimal character, which will be applied to all calc fields and number-validated text fields. 
    /// Options include comma ',' or dot/full stop '.', but if left blank/null, then it will export numbers using the fields' native decimal format. Simply provide the value of either ',' or '.' for this parameter.
    /// </summary>
    public enum DecimalCharacter
    {
        /// <summary>
        /// Comma [,]
        /// </summary>
        /// 
        [Display(Name = ",")]
        comma,
        /// <summary>
        /// Dot [.]
        /// </summary>
        /// 
        [Display(Name = ".")]
        dot,
        /// <summary>
        /// Non (blank/null)
        /// </summary>
        [Display(Name = "none")]
        none
    }
}
