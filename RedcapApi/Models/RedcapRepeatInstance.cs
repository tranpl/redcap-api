using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redcap.Models
{
    /// <summary>
    /// A single repeat instance in redcap
    /// </summary>
    public class RedcapRepeatInstance
    {
        /// <summary>
        /// The instance number that the instrument is repeated, e.g 3, repeated on the 3rd instance
        /// </summary>
        [JsonProperty("redcap_repeat_instance")]
        public int RepeatInstance { get; set; }
    }
}
