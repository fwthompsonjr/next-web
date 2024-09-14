namespace next.core.interfaces
{
    public interface IQueueFilter
    {
        void Append(string userId);
        void Clear();
    }
}
