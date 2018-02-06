using System.Collections.Generic;

namespace Redcap.Models
{
    /// <summary>
    /// A record should contain a set of key/value pair.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// A record value contains a key and its associated value.
        /// </summary>
        public KeyValuePair<string, string> Value { get; set; }
    }
}
