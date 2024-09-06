namespace next.processor.api.interfaces
{
    public interface ITrackable
    {
        Guid Id { get; set; }
        DateTime ExpirationDate { get; set; }
    }
}
