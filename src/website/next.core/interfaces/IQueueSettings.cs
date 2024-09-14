namespace next.core.interfaces
{
    public interface IQueueSettings
    {
        bool IsEnabled { get; set; }
        string? Name { get; set; }
        string? FolderName { get; set; }
    }
}
