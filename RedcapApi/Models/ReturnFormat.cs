namespace RedcapApi.Interfaces
{
    /// <summary>
    /// The format that the response object should be when returned from Redcap API.
    /// RedcapFormat, 0 = JSON
    /// RedcapFormat, 1 = CSV
    /// RedcapFormat, 2 = XML
    /// </summary>
    public enum ReturnFormat
    {
        json = 0,
        csv = 1,
        xml = 2
    }
}
