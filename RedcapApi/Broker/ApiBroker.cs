using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;

using Serilog;

namespace Redcap.Broker
{
    public partial class ApiBroker: IApiBroker
    {
        protected readonly HttpClient httpClient;
        protected readonly IRestClient restClient;
        public ApiBroker(HttpClient httpClient, IRestClient restClient)
        {
            this.httpClient = httpClient;
            this.restClient = restClient;
        }
        public void LogException(Exception ex,
            [CallerMemberName] string method = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            var errorMessage = $"Message: {ex.Message}. Method: {method} File: {filePath} LineNumber: {lineNumber}";
            Log.Error($"Message: {ex.Message}. Method: {method} File: {filePath} LineNumber: {lineNumber}");
            throw new Exception(errorMessage);
        }
        public async Task<T> PostAsync<T>(IRestRequest request, CancellationToken cancellationToken = default)
        {
            var response = await restClient.PostAsync<T>(request, cancellationToken);
            
            return response;
        }
        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            var response = await restClient.ExecuteAsync<T>(request);
            if(response.ErrorException != null)
            {
                LogException(response.ErrorException);
            }
            return response.Data;
        }
    }
}
