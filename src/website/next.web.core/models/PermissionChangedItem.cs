using Newtonsoft.Json;

namespace next.web.core.models
{
    public class PermissionChangedItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [JsonProperty("externalId")]
        public string ExternalId { get; set; } = string.Empty;

        [JsonProperty("invoiceUri")]
        public string InvoiceUri { get; set; } = string.Empty;

        [JsonProperty("levelName")]
        public string LevelName { get; set; } = string.Empty;

        [JsonProperty("sessionId")]
        public string SessionId { get; set; } = string.Empty;

        [JsonProperty("isPaymentSuccess")]
        public bool IsPaymentSuccess { get; set; }

        [JsonProperty("completionDate")]
        public DateTime? CompletionDate { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
    }
}
