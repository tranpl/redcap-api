using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    /// <summary>
    /// Logging type
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Data Export
        /// </summary>
        [Display(Name = "export")]
        Export,

        /// <summary>
        /// Manage/Design
        /// </summary>
        [Display(Name = "manage")]
        Manage,

        /// <summary>
        /// User or role created-updated-deleted
        /// </summary>
        [Display(Name = "user")]
        User,

        /// <summary>
        /// Record created-updated-deleted
        /// </summary>
        [Display(Name = "record")]
        Record,

        /// <summary>
        /// Record created (only)
        /// </summary>
        [Display(Name = "record_add")]
        RecordAdd,

        /// <summary>
        /// Record updated (only)
        /// </summary>
        [Display(Name = "record_edit")]
        RecordEdit,

        /// <summary>
        /// Record deleted (only)
        /// </summary>
        [Display(Name = "record_delete")]
        RecordDelete,

        /// <summary>
        /// Record locking and e-signatures
        /// </summary>
        [Display(Name ="lock_record")]
        LockRecord,
        /// <summary>
        /// Page views
        /// </summary>
        /// 
        [Display(Name = "page_view")]
        PageView,
        /// <summary>
        /// All event types (excluding page views)
        /// </summary>
        [Display(Name = "")]
        All
    }
}
