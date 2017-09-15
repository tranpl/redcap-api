namespace Redcap.Models
{
    /// <summary>
    /// The data format which the Redcap API endpoint should receive.
    /// RedcapDataType, 0 = flat
    /// RedcapDataType, 1 = eav
    /// RedcapDataType, 2 = nonlogitudinal
    /// RedcapDataType, 3 = longitudinal
    /// </summary>
    public enum RedcapDataType
    {
        /// <summary>
        /// output as one record per row [default]
        /// </summary>
        flat = 0,
        /// <summary>
        /// input as one data point per row
        /// </summary>
        eav = 1,
        /// <summary>
        /// EAV: Non-longitudinal: Will have the fields - record*, field_name, value
        /// </summary>
        nonlongitudinal = 2,
        /// <summary>
        /// EAV: Longitudinal: Will have the fields - record*, field_name, value, redcap_event_name
        /// </summary>
        longitudinal = 3
    }
}
