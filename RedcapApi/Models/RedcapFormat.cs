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
        /// <summary>
        /// Default Javascript Notation
        /// </summary>
        json = 0,
        /// <summary>
        /// Comma Seperated Values
        /// </summary>
        csv = 1,
        /// <summary>
        /// Extensible Markup Language
        /// </summary>
        xml = 2
    }
}
