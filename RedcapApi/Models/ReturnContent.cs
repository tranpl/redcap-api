namespace Redcap.Models
{
    /// <summary>
    /// The content that the response object should contain from Redcap API.
    /// ReturnContent, 0 = ids
    /// ReturnContent, 1 = count
    /// </summary>
    public enum ReturnContent
    {
        ids = 0,
        count = 1
    }
}
