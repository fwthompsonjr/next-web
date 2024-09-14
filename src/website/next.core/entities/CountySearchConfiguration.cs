namespace next.core.entities
{
    internal class CountySearchConfiguration
    {
        public string? Name { get; set; }
        public int Index { get; set; }
        public string? StateCode { get; set; }
        public string? ShortName { get; set; }
        public bool IsActive { get; set; }
        public CountySearchDetail? Data { get; set; }
    }
}