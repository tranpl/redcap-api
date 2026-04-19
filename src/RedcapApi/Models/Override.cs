using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Used to erase existing arms in the project or only add new ones
    /// </summary>
    public enum Override
    {
        /// <summary>
        ///   true — You may use override=1 as a 'delete all + import' action in order to erase all existing Arms in the project while importing new Arms.
        /// </summary>
        /// 
        [Display(Name = "true")]
        True = 1,
        /// <summary>
        ///  false [default] If override=0, then you can only add new Arms or rename existing ones. 
        /// </summary>
        /// 
        [Display(Name = "false")]
        False = 0
    }
}
