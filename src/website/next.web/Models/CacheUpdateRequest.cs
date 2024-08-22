using Newtonsoft.Json;

namespace next.web.Models
{
    public class CacheUpdateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
