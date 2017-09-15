namespace Redcap.Models
{
    /// <summary>
    /// Event for redcap longitudinal projects.
    /// </summary>
    public class RedcapEvent
    {
        /// <summary>
        /// Name of the event
        /// </summary>
        public string event_name { get; set; }
        /// <summary>
        /// Arm the event belongs to
        /// </summary>
        public string arm_num { get; set; }
        /// <summary>
        /// Days of offset
        /// </summary>
        public string day_offset { get; set; }
        /// <summary>
        /// Minimum(floor) of offset
        /// </summary>
        public string offset_min { get; set; }
        /// <summary>
        /// Max(ceiling) of offset
        /// </summary>
        public string offset_max { get; set; }
        /// <summary>
        /// Unique event name used to identify this event
        /// </summary>
        public string unique_event_name { get; set; }
        /// <summary>
        /// Label displayed for this event
        /// </summary>
        public object custom_event_label { get; set; }
    }

}
