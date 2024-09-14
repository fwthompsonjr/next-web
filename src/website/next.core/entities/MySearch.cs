using Newtonsoft.Json;

namespace next.core.entities
{
    public class MySearch
    {
        [JsonProperty("details")]
        public List<MySearchDetail> Details { get; set; } = new();

        [JsonProperty("history")]
        public MySearchHistory History { get; set; } = new();
    }
}
