namespace Redcap.Models
{
    /// <summary>
    /// The format that the response object should be when returned from the http request.
    /// Format, 0 = JSON
    /// Format, 1 = CSV
    /// Format, 2 = XML
    /// </summary>
    public enum ReturnFormat
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
        xml = 2,
        /// <summary>
        /// CDISC ODM XML format, specifically ODM version 1.3.1
        /// Only usable on Project Create 
        /// </summary>
        odm = 3
    }
}
