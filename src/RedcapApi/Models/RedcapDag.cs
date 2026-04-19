using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Redcap.Models
{
    /// <summary>
    /// Data Access Group 
    /// Example:
    /// [{"data_access_group_name":"CA Site","unique_group_name":"ca_site"}
    /// {"data_access_group_name":"FL Site","unique_group_name":"fl_site"},
    /// { "data_access_group_name":"New Site","unique_group_name":""}]
    /// </summary>
    public class RedcapDag
    {
        /// <summary>
        /// group name
        /// </summary>
        /// 
        [JsonProperty("data_access_group_name")]
        public string GroupName { get; set; }
        /// <summary>
        /// auto-generated unique group name
        /// </summary>
        /// 
        [JsonProperty("unique_group_name")]

        public string UniqueGroupName { get; set; }
    }
}
