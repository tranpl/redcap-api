namespace Redcap.Models
{
    /// <summary>
    /// The format that the response object should be if there are errors generated when executing the http request.
    /// OnErrorFormat, 0 = JSON
    /// OnErrorFormat, 1 = CSV
    /// OnErrorFormat, 2 = XML
    /// </summary>
    public enum OnErrorFormat
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
