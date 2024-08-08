using Newtonsoft.Json;

namespace next.web.Models
{
    public class FetchIntentResponse
    {
        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; } = string.Empty;
    }
}
