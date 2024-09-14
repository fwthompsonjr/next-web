using Newtonsoft.Json;

namespace next.core.entities
{
    internal class CaseSearchModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        public int? CountyId { get; set; }
    }
}