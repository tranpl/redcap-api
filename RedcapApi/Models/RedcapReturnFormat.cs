using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Specifies the format of error messages
    /// 
    /// The format that the response object should be if there are errors generated when executing the http request.
    /// ReturnFormat, 0 = json
    /// ReturnFormat, 1 = csv
    /// ReturnFormat, 2 = xml
    /// </summary>
    /// 
    public enum RedcapReturnFormat
    {

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
