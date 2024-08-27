namespace next.processor.api.models
{
    public class TrackEventModel
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
