using Newtonsoft.Json;
using next.core.interfaces;

namespace next.core.entities
{
    public class QueueSettings : IQueueSettings
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("folder")]
        public string? FolderName { get; set; }
    }
}
