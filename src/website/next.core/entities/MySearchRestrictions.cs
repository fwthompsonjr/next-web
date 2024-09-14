namespace next.core.entities
{
    public class MySearchRestrictions
    {
        public bool? IsLocked { get; set; }

        public string? Reason { get; set; }

        public int? MaxPerMonth { get; set; }

        public int? MaxPerYear { get; set; }

        public int? ThisMonth { get; set; }

        public int? ThisYear { get; set; }
    }
}
