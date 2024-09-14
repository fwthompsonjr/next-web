using Newtonsoft.Json;

namespace next.core.entities
{
    internal class CboDropDownModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("isDisplayed")]
        public bool? IsDisplayed { get; set; } = true;

        [JsonProperty("members")]
        public List<DropDownModel> Members { get; set; } = new();

        public int? CountyId { get; set; }
    }
}