namespace next.processor.api.models
{
    public class LocalCountyItem
    {
        public int Index { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int? WebId { get; set; }
    }
}
