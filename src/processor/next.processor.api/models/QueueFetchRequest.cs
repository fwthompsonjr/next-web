namespace next.processor.api.models
{
    public class QueueFetchRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
