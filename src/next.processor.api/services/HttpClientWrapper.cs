using next.processor.api.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace next.processor.api.services
{
    [ExcludeFromCodeCoverage(Justification = "All methods here are wrappers to only library methods that are tested.")]
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private HttpClient? _httpClient;
        private bool disposedValue;

        public HttpClientWrapper()
        { }

        public HttpClientWrapper(HttpClient httpClient)
        { _httpClient = httpClient; }

        public async Task<string> GetStringAsync(HttpClient client, string? requestUri)
        {
            _httpClient ??= client;
            var response = await _httpClient.GetStringAsync(requestUri);
            return response;
        }

        public HttpClient Client => _httpClient ?? new();

        public async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(HttpClient client, string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _httpClient ??= client;
                _httpClient.Timeout = TimeSpan.FromSeconds(90);
                var response = await _httpClient.PostAsJsonAsync(
                    requestUri,
                    value,
                    options,
                    cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public void AppendHeader(string key, string value)
        {
            if (_httpClient == null) return;
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.
            // Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}