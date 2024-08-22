using Newtonsoft.Json;

namespace next.processor.api.models
{
    public class QueueInitializeRequest : BaseQueueRequest
    {
        [JsonProperty("MachineName")]
        public string MachineName { get; set; } = string.Empty;
        [JsonProperty("Message")]
        public string Message { get; set; } = string.Empty;
        [JsonProperty("StatusId")]
        public int? StatusId { get; set; }
        [JsonProperty("Items")]
        public List<QueueInitializeRequestItem> Items { get; set; } = [];
    }
}
