namespace next.processor.api.models
{
    public class QueueReportIssueRequest : BaseQueueRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public byte[] Data { get; set; } = [];
    }
}
