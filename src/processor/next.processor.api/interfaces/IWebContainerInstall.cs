namespace next.processor.api.interfaces
{
    public interface IWebContainerInstall
    {
        bool IsInstalled { get; }
        string LastErrorMessage { get; }

        Task<bool> InstallAsync();
    }
}
