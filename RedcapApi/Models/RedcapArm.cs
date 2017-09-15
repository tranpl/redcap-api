namespace Redcap.Models
{
    /// <summary>
    /// Arms are only available for longitudinal projects.
    /// </summary>
    public class RedcapArm
    {
        /// <summary>
        /// Number associated with the event, e.g "1"
        /// </summary>
        public string arm_num { get; set; }
        /// <summary>
        /// Name of the event. e.g "event1_arm_1"
        /// </summary>
        public string name { get; set; }
    }
}
