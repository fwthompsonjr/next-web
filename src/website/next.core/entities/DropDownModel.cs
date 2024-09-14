using Newtonsoft.Json;

namespace next.core.entities
{
    internal class DropDownModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}