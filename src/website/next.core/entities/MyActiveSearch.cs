using Newtonsoft.Json;

namespace next.core.entities
{
    public class MyActiveSearch
    {
        [JsonProperty("details")]
        public List<MyActiveSearchDetail> Details { get; set; } = new();

        [JsonProperty("history")]
        public MyActiveSearchHistory History { get; set; } = new();
    }

    public class MyActiveSearchDetail
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

    public class MyActiveSearchHistory
    {
        [JsonProperty("searches")]
        public List<MyActiveSearchItem> Searches { get; set; } = new();

        [JsonProperty("statuses")]
        public List<MyActiveSearchStatus> Statuses { get; set; } = new();

        [JsonProperty("staged")]
        public List<MyActiveSearchStaged> Staged { get; set; } = new();
    }

    public class MyActiveSearchItem
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
        public int? ExpectedRows { get; set; }
    }

    public class MyActiveSearchStaged
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("lineNbr")]
        public int LineNbr { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; } = string.Empty;

        [JsonProperty("createDate")]
        public string CreateDate { get; set; } = string.Empty;

        [JsonProperty("stagingType")]
        public string StagingType { get; set; } = string.Empty;
    }

    public class MyActiveSearchStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("line")]
        public string Line { get; set; } = string.Empty;

        [JsonProperty("lineNbr")]
        public int LineNbr { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; } = string.Empty;

        [JsonProperty("createDate")]
        public string CreateDate { get; set; } = string.Empty;
    }


}
