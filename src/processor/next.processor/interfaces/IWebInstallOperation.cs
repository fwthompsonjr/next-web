namespace next.processor.api.interfaces
{
    public interface IWebInstallOperation
    {
        bool AppendToPath(string path);
        bool CreateDirectory(string path);
        bool DirectoryExists(string path);
        int DirectoryFileCount(string path);
        Task<bool> DownloadFromUriAsync(string uri, string destinationPath, CancellationToken cancellationToken);
        Task<bool> DownloadDriverAsync(string uri, string destinationPath, CancellationToken cancellationToken);
        Task<bool> ExtractGzipToDirectoryAsync(string sourceFileName, string destinationDir, CancellationToken cancellationToken);
        Task<bool> ExtractTarToDirectoryAsync(string sourceFileName, string destinationDir, CancellationToken cancellationToken);
        bool FileExists(string path);
    }
}
