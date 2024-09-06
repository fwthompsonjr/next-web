using System.Text.Json;

namespace next.processor.api.interfaces
{
    public interface IHttpClientWrapper : IDisposable
    {
        HttpClient Client { get; }

        void AppendHeader(string key, string value);

        Task<string> GetStringAsync(HttpClient client, string? requestUri);

        Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
            HttpClient client,
            string? requestUri,
            TValue value,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default);
    }
}
