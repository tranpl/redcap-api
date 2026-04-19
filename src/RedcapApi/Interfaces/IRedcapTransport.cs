using System.Net.Http;

namespace Redcap.Interfaces
{
    /// <summary>
    /// Abstraction over REDCap HTTP transport so API methods can be tested without real network calls.
    /// </summary>
    public interface IRedcapTransport
    {
        /// <summary>
        /// Sends a form-urlencoded request and returns the response stream.
        /// </summary>
        Task<Stream> GetStreamContentAsync(RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100);

        /// <summary>
        /// Sends a multipart request and returns the response body.
        /// </summary>
        Task<string> SendPostRequestAsync(RedcapApi redcapApi, MultipartFormDataContent payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100);

        /// <summary>
        /// Sends a form-urlencoded request and returns the response body.
        /// </summary>
        Task<string> SendPostRequestAsync(RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100);
    }
}