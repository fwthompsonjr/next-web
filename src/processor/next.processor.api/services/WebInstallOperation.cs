using ICSharpCode.SharpZipLib.BZip2;
using next.processor.api.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Tar;
using System.IO.Compression;

namespace next.processor.api.services
{
    [ExcludeFromCodeCoverage(Justification = "Wrapper class to extract file system operations")]
    public class WebInstallOperation : IWebInstallOperation
    {
        public bool AppendToPath(string path)
        {
            try
            {

                if (!FileExists(path)) return false;
                var currentPaths = Environment.GetEnvironmentVariable(pathVariable)?.Split(colon).ToList() ?? [];
                if (currentPaths.Contains(path)) return true;
                currentPaths.Add(path);
                Environment.SetEnvironmentVariable(pathVariable, string.Join(colon, currentPaths));
                return AppendToPath(path);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return Directory.Exists(path);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public int DirectoryFileCount(string path)
        {
            if (!Directory.Exists(path)) return 0;
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;
        }

        public async Task<bool> DownloadDriverAsync(string uri, string destinationPath, CancellationToken cancellationToken)
        {
            if (FileExists(destinationPath)) return true;
            var httpClient = new HttpClient();
            var responseStream = await httpClient.GetStreamAsync(_driverPath, cancellationToken);
            using var fileStream = new FileStream(destinationPath, FileMode.Create);
            await responseStream.CopyToAsync(fileStream, cancellationToken);
            return FileExists(destinationPath);
        }

        public async Task<bool> DownloadFromUriAsync(string uri, string destinationPath, CancellationToken cancellationToken)
        {
            if (FileExists(destinationPath)) return true;
            using var httpClient = new HttpClient();
            using MemoryStream ms = new();
            using var responseStream = await httpClient.GetStreamAsync(uri, cancellationToken);
            await responseStream.CopyToAsync(ms, cancellationToken);
            var contents = ms.ToArray();
            await File.WriteAllBytesAsync(destinationPath, contents, cancellationToken);
            return FileExists(destinationPath);
        }

        public async Task<bool> ExtractGzipToDirectoryAsync(string sourceFileName, string destinationDir, CancellationToken cancellationToken)
        {
            if (!FileExists(sourceFileName)) return false;
            var count = DirectoryFileCount(destinationDir);
            if (count > 0) return true;
            if (!CreateDirectory(destinationDir)) return false;
            using var stream = new BZip2InputStream(File.OpenRead(sourceFileName));
            await TarFile.ExtractToDirectoryAsync(stream, destinationDir, true, cancellationToken);
            return DirectoryFileCount(destinationDir) > 0;
        }

        public async Task<bool> ExtractTarToDirectoryAsync(string sourceFileName, string destinationDir, CancellationToken cancellationToken)
        {
            if (!FileExists(sourceFileName)) return false;
            var count = DirectoryFileCount(destinationDir);
            if (count > 0) return true;
            if (!CreateDirectory(destinationDir)) return false;
            await using var tarStream = new FileStream(sourceFileName, new FileStreamOptions { Mode = FileMode.Open, Access = FileAccess.Read, Options = FileOptions.Asynchronous });
            await using MemoryStream memoryStream = new();
            await using (GZipStream gzipStream =
                new(tarStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(memoryStream, cancellationToken);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            await TarFile.ExtractToDirectoryAsync(
                memoryStream,
                destinationDir,
                overwriteFiles: true,
                cancellationToken: cancellationToken
            );
            return DirectoryFileCount(destinationDir) > 0;
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }


        private const char colon = ':';
        private const string pathVariable = "PATH";
        protected const string _driverPath = "https://github.com/mozilla/geckodriver/releases/download/v0.35.0/geckodriver-v0.35.0-linux64.tar.gz";
    }
}
