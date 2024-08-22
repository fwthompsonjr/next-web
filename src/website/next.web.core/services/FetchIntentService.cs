using next.web.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace next.web.core.services
{
    [ExcludeFromCodeCoverage(Justification = "Wrapper class that makes remote http call")]
    public class FetchIntentService : IFetchIntentService
    {
        public async Task<string?> GetIntent(string url, string request)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(url, request);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
