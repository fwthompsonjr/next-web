using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;
using System.Diagnostics.CodeAnalysis;

namespace next.processor.api.services
{
    public class ApiWrapperService : IApiWrapper
    {
        
        public virtual IHttpClientWrapper GetClientWrapper(HttpClient client)
        {
            return new HttpClientWrapper(client);
        }

        public async Task<List<QueuedRecord>?> FetchAsync()
        {
            var uri = PostUris.Find(x => x.Name == "fetch")?.Address;
            if (uri == null) return null;
            using var client = new HttpClient();
            using var wrp = GetClientWrapper(client);
            wrp.AppendHeader(application_key, AppPayload);
            var response = await wrp.PostAsJsonAsync<object?>(client, uri, null);
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            var obj = content.ToInstance<ApiResponse>();
            if (obj == null || string.IsNullOrWhiteSpace(obj.Message)) return null;
            var data = obj.Message.ToInstance<List<QueuedRecord>>();
            return data;
        }

        private const string application_key = "APP_IDENTITY";
        private static string? application_payload;
        private static string AppPayload => application_payload ??= GetApplicationPayload();
        private static readonly List<ApiAddress> PostUris = PostAddressProvider.PostAddresses() ?? [];
        private static string GetApplicationPayload()
        {
            var source = SettingsProvider.Configuration["api.source"] ?? "oxford.leads.data.services";
            var obj = new { Id = Guid.NewGuid(), Name = source };
            return obj.ToJsonString();
        }
    }
}
