using Newtonsoft.Json;

namespace next.processor.api.models
{
    public class QueueInitializeRequestItem
    {
        [JsonProperty("Id")]
        public string Id { get; set; } = string.Empty;
    }
}
