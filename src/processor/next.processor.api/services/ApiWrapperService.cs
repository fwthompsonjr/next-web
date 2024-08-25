using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;
using System.Globalization;
using System.Text;

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
            var payload = AppPayload.ToInstance<QueueFetchRequest>();
            var obj = await GetApiResponseAsync(payload, uri);
            if (obj == null || string.IsNullOrWhiteSpace(obj.Message)) return null;
            var data = obj.Message.ToInstance<List<QueuedRecord>>();
            return data;
        }

        public async Task StartAsync(QueuedRecord dto)
        {
            var uri = PostUris.Find(x => x.Name == "start")?.Address;
            if (uri == null) return;
            var copy = dto.ToJsonString().ToInstance<QueuedRecord>();
            if (copy == null) return;
            copy.AppendSource();
            _ = await GetApiResponseAsync(copy, uri);
        }

        public async Task PostStatusAsync(QueuedRecord dto, int messageId, int statusId)
        {
            var uri = PostUris.Find(x => x.Name == "status")?.Address;
            if (uri == null) return;
            var payload = GetStatusPayload(dto, messageId, statusId);
            _ = await GetApiResponseAsync(payload, uri);
        }

        public async Task PostStepCompletionAsync(QueuedRecord dto, int messageId, int statusId)
        {
            var uri = PostUris.Find(x => x.Name == "complete")?.Address;
            if (uri == null) return;
            var payload = GetStatusPayload(dto, messageId, statusId);
            _ = await GetApiResponseAsync(payload, uri);
        }
        public async Task PostSaveContentAsync(QueuedRecord dto, byte[] content)
        {
            var uri = PostUris.Find(x => x.Name == "save")?.Address;
            if (uri == null) return;
            var payload = new QueuePersistenceRequest
            {
                Id = dto.Id ?? string.Empty,
                Content = content
            };
            payload.AppendSource();
            _ = await GetApiResponseAsync(payload, uri);
        }
        public async Task PostStepFinalizedAsync(QueuedRecord dto, List<QueuePersonItem> people)
        {
            var uri = PostUris.Find(x => x.Name == "finalize")?.Address;
            if (uri == null) return;
            var uniqueId = dto.Id ?? string.Empty;
            var payload = GetFinalizedPayload(uniqueId, dto, people);
            _ = await GetApiResponseAsync(payload, uri);
        }

        public async Task ReportIssueAsync(QueuedRecord dto, Exception exception)
        {
            var uri = PostUris.Find(x => x.Name == "log-issue")?.Address ?? string.Empty;
            var id = dto.Id ?? string.Empty;
            var message = exception.Message;
            var details = Encoding.UTF8.GetBytes(exception.ToString());
            var issue = new QueueReportIssueRequest
            {
                Id = id,
                Message = message,
                Data = details
            };
            issue.AppendSource();
            _ = await GetApiResponseAsync(issue, uri);
        }


        private static QueueCompletionRequest GetFinalizedPayload(string uniqueId, QueuedRecord dto, List<QueuePersonItem> people)
        {
            var request = (dto.Payload ?? string.Empty).ToInstance<QueueSearchItem>() ?? new();
            var data = people.ToJsonString();
            var obj = new QueueCompletionRequest { Data = data, QueryParameter = request.ToJsonString(), UniqueId = uniqueId };
            obj.AppendSource();
            return obj;
        }

        private static QueueRecordStatusRequest? GetStatusPayload(QueuedRecord dto, int messageId, int statusId)
        {
            var uniqueId = dto.Id ?? string.Empty;
            var message = GetMessage(dto.Payload, messageId, statusId);
            if (string.IsNullOrEmpty(uniqueId) || string.IsNullOrWhiteSpace(message)) return null;
            var obj = new QueueRecordStatusRequest
            {
                MessageId = messageId,
                StatusId = statusId,
                UniqueId = uniqueId
            };
            obj.AppendSource();
            return obj;
        }

        private static string GetMessage(string? payload, int messageId, int statusId)
        {
            if (messageId < 0 && !string.IsNullOrEmpty(payload))
            {
                return GetSearchBeginMessage(payload);
            }
            var message_item = Messages.Find(x => x.Id == messageId);
            var status_item = Statuses.Find(x => x.Id == statusId);
            if (message_item == null || status_item == null) return string.Empty;
            var fmt = message_item.Descriptor;
            return string.Format(fmt, component_key, status_item.Descriptor);
        }

        private static string GetSearchBeginMessage(string payload)
        {
            var fmt = new DateTimeFormatInfo();
            var style = DateTimeStyles.AllowWhiteSpaces;
            var request = payload.ToInstance<QueueSearchItem>();
            if (request == null) return string.Empty;
            if (!DateTime.TryParse(request.StartDate, fmt, style, out var starting)) return string.Empty;
            if (!DateTime.TryParse(request.EndDate, fmt, style, out var ending)) return string.Empty;
            var message = $"{component_key}: search begin " +
                $"State: {request.State}, " +
                $"County: {request.County}, " +
                $"Start: {starting:d}, Ending: {ending:d}";
            return message;
        }

        private async Task<ApiResponse?> GetApiResponseAsync(object? payload, string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)) return null;

            using var client = new HttpClient();
            using var wrp = GetClientWrapper(client);
            wrp.AppendHeader(application_key, AppPayload);
            var response = await wrp.PostAsJsonAsync(client, uri, payload);
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return content.ToInstance<ApiResponse>();
        }
        private const string component_key = "oxford.processor.api";
        private const string application_key = "APP_IDENTITY";
        private static string? application_payload;
        private static string AppPayload => application_payload ??= GetApplicationPayload();
        private static readonly List<ApiAddress> PostUris = PostAddressProvider.PostAddresses() ?? [];
        private static readonly List<ItemDescriptor> Messages = MessageNameProvider.MessageSequence() ?? [];
        private static readonly List<ItemDescriptor> Statuses = StatusNameProvider.StatusSequence() ?? [];
        private static string GetApplicationPayload()
        {
            var source = SettingsProvider.Configuration["api.source"] ?? "oxford.leads.data.services";
            var obj = new { Id = Guid.NewGuid(), Name = source };
            return obj.ToJsonString();
        }
    }
}
