namespace next.processor.api.interfaces
{
    public interface IQueueExecutor
    {
        bool IsRunning { get; }

        Task ExecuteAsync();
        Dictionary<string, object> GetDetails();
        IQueueProcess? GetInstance(string queueName);
        int InstallerCount();
        bool? IsReady();
        int IsReadyCount();
    }
}
