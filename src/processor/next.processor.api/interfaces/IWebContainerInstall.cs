namespace next.processor.api.interfaces
{
    public interface IWebContainerInstall
    {
        bool IsInstalled { get; }
        Task<bool> InstallAsync();
    }
}
