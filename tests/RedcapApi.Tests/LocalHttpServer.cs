using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RedcapApi.Tests;

internal sealed class LocalHttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly Func<CapturedRequest, TestResponse> _responseFactory;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _listenerTask;

    public LocalHttpServer(Func<CapturedRequest, TestResponse> responseFactory)
    {
        _responseFactory = responseFactory;

        var port = GetAvailablePort();
        Url = new Uri($"http://localhost:{port}/");

        _listener = new HttpListener();
        _listener.Prefixes.Add(Url.ToString());
        _listener.Start();

        _listenerTask = Task.Run(ListenAsync);
    }

    public Uri Url { get; }

    public ConcurrentQueue<CapturedRequest> Requests { get; } = new();

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();

        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener.Close();

        try
        {
            _listenerTask.GetAwaiter().GetResult();
        }
        catch
        {
        }

        _cancellationTokenSource.Dispose();
    }

    private async Task ListenAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            HttpListenerContext? context = null;

            try
            {
                context = await _listener.GetContextAsync();
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }

            if (context is null)
            {
                continue;
            }

            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding ?? Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            var request = new CapturedRequest(
                context.Request.HttpMethod,
                context.Request.Url?.AbsolutePath ?? "/",
                body,
                context.Request.Headers.AllKeys.ToDictionary(key => key!, key => context.Request.Headers[key] ?? string.Empty));

            Requests.Enqueue(request);

            var response = _responseFactory(request);
            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = response.ContentType;

            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            if (!string.IsNullOrEmpty(response.Body))
            {
                var bytes = Encoding.UTF8.GetBytes(response.Body);
                await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            }

            context.Response.Close();
        }
    }

    private static int GetAvailablePort()
    {
        using var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        return ((IPEndPoint)tcpListener.LocalEndpoint).Port;
    }
}

internal sealed record CapturedRequest(string Method, string Path, string Body, Dictionary<string, string> Headers);

internal sealed class TestResponse
{
    public TestResponse(int statusCode, string body, string contentType = "text/plain", Dictionary<string, string>? headers = null)
    {
        StatusCode = statusCode;
        Body = body;
        ContentType = contentType;
        Headers = headers ?? new Dictionary<string, string>();
    }

    public int StatusCode { get; }

    public string Body { get; }

    public string ContentType { get; }

    public Dictionary<string, string> Headers { get; }
}