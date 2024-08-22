using next.processor.api.models;

namespace next.processor.api.interfaces
{
    public interface IQueueProcess
    {
        int Index { get; }
        string Name { get; }
        bool IsSuccess { get; }
        bool AllowIterateNext { get; }
        Task<QueueProcessResponses?> ExecuteAsync(QueueProcessResponses? record);
    }
}
