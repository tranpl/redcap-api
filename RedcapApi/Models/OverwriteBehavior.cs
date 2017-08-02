namespace Redcap.Models
{
    /// <summary>
    /// normal - blank/empty values will be ignored [default]
    /// overwrite - blank/empty values are valid and will overwrite data
    /// </summary>
    public enum OverwriteBehavior
    {
        normal = 0,
        overwrite = 1
    }
}
