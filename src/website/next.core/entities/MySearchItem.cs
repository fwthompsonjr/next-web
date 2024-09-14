using Newtonsoft.Json;

namespace next.core.entities
{
    public class MySearchItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("endDate")]
        public string EndDate { get; set; } = string.Empty;

        [JsonProperty("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonProperty("createDate")]
        public string CreateDate { get; set; } = string.Empty;

        [JsonProperty("expectedRows")]
        public int ExpectedRows { get; set; }
    }
}
