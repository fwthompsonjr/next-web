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
    }
}
