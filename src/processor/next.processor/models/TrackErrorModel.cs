namespace next.processor.api.models
{
    public class TrackErrorModel : BaseTrackingModel
    {
        public QueueReportIssueRequest Data { get; set; } = new();
    }
}
