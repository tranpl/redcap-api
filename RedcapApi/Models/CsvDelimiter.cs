using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Csv Delimiter for the input data (redcap data type)
    /// </summary>
    public enum CsvDelimiter
    {
        /// <summary>
        /// , [default]
        /// </summary>
        [Display(Name = ",")]
        comma,
        /// <summary>
        /// tab
        /// </summary>
        /// 
        [Display(Name = "tab")]
        tab,
        /// <summary>
        /// ;
        /// </summary>
        /// 
        [Display(Name = ";")]
        semiColon,
        /// <summary>
        /// |
        /// </summary>
        /// 
        [Display(Name = "|")]
        pipe,
        /// <summary>
        /// ^
        /// </summary>
        /// 
        [Display(Name = "^")]
        caret
    }
}
