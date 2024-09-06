namespace next.processor.api.interfaces
{
    public interface IStatusChanger
    {
        void ChangeStatus(string status);
        void ChangeStatus(string status, string health);
    }
}
