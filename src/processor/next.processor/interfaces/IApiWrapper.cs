using legallead.jdbc.entities;
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
        Task<List<StatusSummaryByCountyBo>?> FetchStatusAsync(int statusId);
        Task<List<StatusSummaryBo>?> FetchSummaryAsync();
        /// <summary>
        /// Retrieves list of non person records that require processing
        /// </summary>
        /// <returns></returns>
        Task<List<QueueNonPersonBo>?> FetchNonPersonAsync();
        Task PostSaveContentAsync(QueuedRecord dto, byte[] content);
        Task PostStatusAsync(QueuedRecord dto, int messageId, int statusId);
        Task PostStepCompletionAsync(QueuedRecord dto, int messageId, int statusId);
        Task PostStepFinalizedAsync(QueuedRecord dto, List<QueuePersonItem> people);
        Task ReportIssueAsync(QueuedRecord dto, Exception exception);
        Task StartAsync(QueuedRecord dto);
        Task PostSaveNonPersonAsync(QueueNonPersonBo dto);
    }
}
