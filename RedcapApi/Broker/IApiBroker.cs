using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;

namespace Redcap.Broker
{
    public interface IApiBroker
    {
        Task<T> ExecuteAsync<T>(RestRequest request) where T : new();
        void LogException(Exception ex, [CallerMemberName] string method = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0);
        Task<T> PostAsync<T>(IRestRequest request, CancellationToken cancellationToken = default);
    }
}
