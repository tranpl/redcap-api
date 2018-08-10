namespace Redcap.Models
{
    /// <summary>
    /// The format that the response object should be if there are errors generated when executing the http request.
    /// OnErrorFormat, 0 = json
    /// OnErrorFormat, 1 = csv
    /// OnErrorFormat, 2 = xml
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
