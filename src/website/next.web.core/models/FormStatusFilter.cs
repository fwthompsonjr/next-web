using Newtonsoft.Json;

namespace next.web.core.models
{
    internal class FormStatusFilter
    {
        [JsonProperty("statusId")]
        public int StatusId { get; set; }
        [JsonProperty("countyName")]
        public string CountyName { get; set; } = string.Empty;
        [JsonProperty("heading")]
        public string Heading { get; set; } = "history";
    }
}