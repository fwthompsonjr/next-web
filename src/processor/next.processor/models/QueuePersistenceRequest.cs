namespace next.processor.api.models
{
    public class QueuePersistenceRequest : BaseQueueRequest
    {
        public string Id { get; set; } = string.Empty;
        public byte[]? Content { get; set; }
    }
}
