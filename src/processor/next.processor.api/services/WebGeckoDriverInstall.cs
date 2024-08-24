using next.processor.api.interfaces;

namespace next.processor.api.services
{
    public class WebGeckoDriverInstall(IWebInstallOperation webInstallOperation) : BaseWebInstall(webInstallOperation)
    {
        public async override Task<bool> InstallAsync()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(environmentDir)) { return false; }
            var downloadDir = Path.Combine(environmentDir, "download");
            var destinationDir = Path.Combine(environmentDir, "util");
            var geckoDir = Path.Combine(destinationDir, "gecko");
            var geckoFile = Path.Combine(geckoDir, "geckodriver");
            var paths = new[] { downloadDir, destinationDir, geckoDir }.ToList();
            paths.ForEach(path => _fileSvc.CreateDirectory(path));
            var uri = _driverPath;
            var downloadPath = Path.Combine(downloadDir, "geckodriver.tar.gz");
            var isdownloaded = await _fileSvc.DownloadDriverAsync(uri, downloadPath, default);
            if (!isdownloaded) return false;
            var extracted = await _fileSvc.ExtractTarToDirectoryAsync(downloadPath, geckoDir, default);
            if (!extracted) return false;
            if (!_fileSvc.FileExists(geckoFile)) return false;
            var added = _fileSvc.AppendToPath(geckoFile);
            return added;
        }
    }
}