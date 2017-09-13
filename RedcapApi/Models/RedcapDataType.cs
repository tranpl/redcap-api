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
        flat = 0,
        eav = 1,
        nonlongitudinal = 2,
        longitudinal = 3
    }
}
