using Newtonsoft.Json;

namespace next.processor.api.models
{
    public class QueueUpdateRequest : BaseQueueRequest
    {
        [JsonProperty("Id")]
        public string? Id { get; set; }
        [JsonProperty("SearchId")]
        public string? SearchId { get; set; }
        [JsonProperty("Message")]
        public string? Message { get; set; }
        [JsonProperty("StatusId")]
        public int? StatusId { get; set; }
    }
}
