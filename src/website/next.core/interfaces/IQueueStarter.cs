namespace next.core.interfaces
{
    public interface IQueueStarter
    {
        int PriorityId { get; }
        string Name { get; }
        bool IsReady { get; }
        string ServiceName { get; }
        void Start();
        List<string> UserList();
    }
}