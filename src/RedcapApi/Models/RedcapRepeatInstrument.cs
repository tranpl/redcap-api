using Newtonsoft.Json;

namespace Redcap.Models
{
    /// <summary>
    /// Redcap Repeat Instrument
    /// The instrument that is repeated in a project that has repeating instruments enabled.
    /// This can support "repeat entire event" or "repeat instruments" mode.
    /// </summary>
    public class RedcapRepeatInstrument: RedcapEvent
    {

        /// <summary>
        /// The unique instrument/form name that is repeated for the specific event.
        /// e.g demographics
        /// </summary>
        [JsonProperty("form_name")]
        public string FormName { get; set; }

        /// <summary>
        /// The custom form/instrument label for this repeating instrument or event.
        /// </summary>
        [JsonProperty("custom_form_label")]
        public string CustomFormLabel { get; set; }
    }
}
