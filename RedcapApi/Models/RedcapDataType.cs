namespace Redcap.Interfaces
{
    /// <summary>
    /// The format which is provided when requesting a import through Redcap API
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
