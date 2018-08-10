using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Redcap.Models
{
    /// <summary>
    /// Action 
    /// </summary>
    public enum RedcapAction
    {
        /// <summary>
        /// Export Action
        /// </summary>
        /// 
        [Description("export")]
        Export,

        /// <summary>
        /// Import Action
        /// </summary>
        /// 
        [Description("Import")]
        Import,
        /// <summary>
        /// Delete Action
        /// </summary>
        /// 
        [Description("Delete")]
        Delete,
    }
}
