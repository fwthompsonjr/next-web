namespace next.core.entities
{
    public class BeginSearchModel
    {
        public string State { get; set; } = string.Empty;
        public BeginSearchCounty County { get; set; } = new();
        public List<BeginSearchDetail> Details { get; set; } = new();
        public long? Start { get; set; }
        public long? End { get; set; }
    }
}
