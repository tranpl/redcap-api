using Newtonsoft.Json;

namespace Redcap.Models
{
    /// <summary>
    /// Event for redcap longitudinal projects and repeating instruments/forms.
    /// </summary>
    public class RedcapEvent
    {
        /// Name of the Event
        /// This is required.
        /// The unique name of the redcap event, usually appended with arm_1 after event name.
        /// e.g "event_1_arm_1"
        [JsonProperty("event_name")]
        [JsonRequired]
        public string EventName { get; set; }
        /// <summary>
        /// Arm the event belongs to, if any
        /// </summary>
        /// 
        [JsonProperty("arm_num")]
        public string ArmNumber { get; set; }
        /// <summary>
        /// Days of offset, if any
        /// </summary>
        /// 
        [JsonProperty("day_offset")]
        public string DayOffset { get; set; }
        /// <summary>
        /// Minimum(floor) of offset, if any
        /// </summary>
        /// 
        [JsonProperty("offset_min")]
        public string MinimumOffset { get; set; }
        /// <summary>
        /// Max(ceiling) of offset, if any
        /// </summary>
        /// 
        [JsonProperty("offset_max")]
        public string MaximumOffset { get; set; }
        /// <summary>
        /// Unique event name used to identify this event, if any
        /// </summary>
        /// 
        [JsonProperty("unique_event_name")]
        public string UniqueEventName { get; set; }
        /// <summary>
        /// Label displayed for this event, if any
        /// </summary>
        /// 
        [JsonProperty("custom_event_label")]
        public object CustomEventLabel { get; set; }
    }

}
