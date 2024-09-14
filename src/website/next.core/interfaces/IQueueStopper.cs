namespace next.core.interfaces
{
    public interface IQueueStopper
    {
        string ServiceName { get; }
        void Stop();
    }
}
