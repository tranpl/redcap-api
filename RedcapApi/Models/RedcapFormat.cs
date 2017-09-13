namespace Redcap.Models
{
    /// <summary>
    /// The format which is provided when requesting through Redcap API
    /// RedcapFormat, 0 = JSON
    /// RedcapFormat, 1 = CSV
    /// RedcapFormat, 2 = XML
    /// </summary>
    public enum InputFormat
    {
        json = 0,
        csv = 1,
        xml = 2
    }
}
