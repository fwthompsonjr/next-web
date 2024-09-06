using next.processor.api.models;

namespace next.processor.api.interfaces
{
    public interface IApiWrapper
    {
        /// <summary>
        /// Retrieves list of records that require processing
        /// </summary>
        /// <returns></returns>
        Task<List<QueuedRecord>?> FetchAsync();
        Task PostSaveContentAsync(QueuedRecord dto, byte[] content);
        Task PostStatusAsync(QueuedRecord dto, int messageId, int statusId);
        Task PostStepCompletionAsync(QueuedRecord dto, int messageId, int statusId);
        Task PostStepFinalizedAsync(QueuedRecord dto, List<QueuePersonItem> people);
        Task ReportIssueAsync(QueuedRecord dto, Exception exception);
        Task StartAsync(QueuedRecord dto);
    }
}
