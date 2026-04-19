namespace Redcap.Models
{
    /// <summary>
    /// Event for redcap longitudinal projects.
    /// </summary>
    public class FormEventMapping
    {
        /// <summary>
        /// Arm the event belongs to
        /// </summary>
        public string arm_num { get; set; }
        /// <summary>
        /// Days of offset
        /// </summary>
        public string form { get; set; }
        /// <summary>
        /// Unique event name used to identify this event
        /// </summary>
        public string unique_event_name { get; set; }

    }

}
