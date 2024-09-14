using Newtonsoft.Json;

namespace next.core.entities
{
    public class MySearchDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("countyName")]
        public string CountyName { get; set; } = string.Empty;

        [JsonProperty("stateName")]
        public string StateName { get; set; } = string.Empty;

        [JsonProperty("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonProperty("endDate")]
        public string EndDate { get; set; } = string.Empty;

        [JsonProperty("searchProgress")]
        public string SearchProgress { get; set; } = string.Empty;
    }
}
