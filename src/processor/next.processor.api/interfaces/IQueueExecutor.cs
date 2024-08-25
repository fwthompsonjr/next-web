
namespace next.processor.api.interfaces
{
    public interface IQueueExecutor
    {
        bool IsRunning { get; }

        Task ExecuteAsync();
        IQueueProcess? GetInstance(string queueName);
        bool? IsReady();
    }
}
