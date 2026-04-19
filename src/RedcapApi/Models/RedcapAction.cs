using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// API Action => Export, Import, Delete 
    /// </summary>
    public enum RedcapAction
    {
        /// <summary>
        /// Create Folder
        /// </summary>
        [Display(Name ="createFolder")]
        CreateFolder,

        /// <summary>
        /// List Folders
        /// </summary>
        [Display(Name ="list")]
        List,

        /// <summary>
        /// Export Action
        /// </summary>
        /// 
        [Display(Name = "export")]
        Export,

        /// <summary>
        /// Import Action
        /// </summary>
        /// 
        [Display(Name = "import")]
        Import,
        /// <summary>
        /// Delete Action
        /// </summary>
        /// 
        [Display(Name = "delete")]
        Delete,
        /// <summary>
        /// Switch
        /// </summary>
        [Display(Name = "switch")]
        Switch,
        /// <summary>
        /// Rename
        /// </summary>
        [Display(Name = "rename")]
        Rename,

        /// <summary>
        /// Randomize
        /// </summary>
        [Display(Name = "randomize")]
        Randomize
    }
}
