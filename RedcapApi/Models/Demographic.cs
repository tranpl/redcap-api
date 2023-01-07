using Newtonsoft.Json;

namespace Redcap.Models
{
    /// <summary>
    /// Simplified demographics instrument that we can test with.
    /// </summary>
    public class Demographic
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonRequired]
        [JsonProperty("record_id")]
        public string RecordId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}
