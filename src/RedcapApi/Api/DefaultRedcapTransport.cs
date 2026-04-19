using System.Net.Http;

using Redcap.Interfaces;
using Redcap.Utilities;

namespace Redcap.Api
{
    /// <summary>
    /// Default HTTP transport implementation used by RedcapApi.
    /// </summary>
    public sealed class DefaultRedcapTransport : IRedcapTransport
    {
        /// <inheritdoc />
        public Task<Stream> GetStreamContentAsync(RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            return Utils.GetStreamContentAsync(redcapApi, payload, uri, cancellationToken, timeOutSeconds);
        }

        /// <inheritdoc />
        public Task<string> SendPostRequestAsync(RedcapApi redcapApi, MultipartFormDataContent payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            return Utils.SendPostRequestAsync(redcapApi, payload, uri, cancellationToken, timeOutSeconds);
        }

        /// <inheritdoc />
        public Task<string> SendPostRequestAsync(RedcapApi redcapApi, Dictionary<string, string> payload, Uri uri, CancellationToken cancellationToken = default, long timeOutSeconds = 100)
        {
            return Utils.SendPostRequestAsync(redcapApi, payload, uri, cancellationToken, timeOutSeconds);
        }
    }
}