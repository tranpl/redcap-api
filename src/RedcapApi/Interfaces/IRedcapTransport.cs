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
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">The form fields or multipart content to submit.</param>
        /// <param name="uri">The URL of the REDCap endpoint.</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">The HTTP request timeout in seconds.</param>
        /// <returns>A stream of response bytes for the caller to dispose.</returns>
        Task<Stream> GetStreamContentAsync(RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100);

        /// <summary>
        /// Sends a multipart request and returns the response body.
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">The form fields or multipart content to submit.</param>
        /// <param name="uri">The URL of the REDCap endpoint.</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">The HTTP request timeout in seconds.</param>
        /// <returns>The REDCap response body as text.</returns>
        Task<string> SendPostRequestAsync(RedcapApi redcapApi, MultipartFormDataContent payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100);

        /// <summary>
        /// Sends a form-urlencoded request and returns the response body.
        /// </summary>
        /// <param name="redcapApi">The client whose transport settings apply to this operation.</param>
        /// <param name="payload">The form fields or multipart content to submit.</param>
        /// <param name="uri">The URL of the REDCap endpoint.</param>
        /// <param name="cancellationToken">A token that can cancel the request.</param>
        /// <param name="timeOutSeconds">The HTTP request timeout in seconds.</param>
        /// <returns>The REDCap response body as text.</returns>
        Task<string> SendPostRequestAsync(RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100);
    }
}