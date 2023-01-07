using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Specifies the format of data
    /// 
    /// Format, 0 = json
    /// Format, 1 = csv [default]
    /// Format, 2 = xml 
    public enum RedcapFormat
    {

        /// </summary>
        /// <summary>
        /// Default Javascript Notation
        /// </summary>
        /// 
        [Display(Name = "json")]
        json = 0,
        /// <summary>
        /// Comma Seperated Values
        /// </summary>
        /// 
        [Display(Name = "csv")]

        csv = 1,
        /// <summary>
        /// Extensible Markup Language
        /// </summary>
        /// 
        [Display(Name = "xml")]
        xml = 2
    }
}
