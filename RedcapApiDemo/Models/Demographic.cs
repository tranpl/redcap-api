using Newtonsoft.Json;

namespace RedcapApiDemo.Models
{
    /// <summary>
    /// A class that represents the demopgrahics form for the demographics template.
    /// Add additional properties that you've added to the redcap instrument as needed.
    /// </summary>
    public class Demographic
    {
        [JsonRequired]
        [JsonProperty("record_id")]
        public string RecordId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("bio")]
        public string Bio { get; set; }

        /// <summary>
        /// Test file uploads
        /// </summary>
        [JsonProperty("upload_file")]
        public string UploadFile { get; set; }
    }


}
