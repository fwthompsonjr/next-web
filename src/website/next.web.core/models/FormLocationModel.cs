using Newtonsoft.Json;

namespace next.web.core.models
{
    internal class FormLocationModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("location")]
        public string? Location { get; set; }
    }
}
