using Newtonsoft.Json;

namespace next.web.core.models
{
    public class PermissionChangedResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("request")]
        public string Request { get; set; } = string.Empty;

        [JsonProperty("dto")]
        public PermissionChangedItem Dto { get; set; } = new();
    }
}
