using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Formats.Tar;
using System.IO.Compression;
using System.Reflection;
namespace next.processor.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IWebHostEnvironment hostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = hostEnvironment;


        [HttpGet("content-path")]
        public ActionResult ContentPath()
        {
            var contentRootPath = _env.ContentRootPath;
            var webRootPath = _env.WebRootPath;
            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyPath = entryAssembly == null ? string.Empty : Path.GetDirectoryName(entryAssembly.Location);
            if (Directory.Exists(assemblyPath))
            {
                var files = Directory.GetFiles(assemblyPath, "*.bz2", SearchOption.AllDirectories);
                var zipfile = files.Length > 0 ? files[0] : string.Empty;
                return Ok(new { contentRootPath, webRootPath, assemblyPath, zipfile });
            }
            return Ok(new { contentRootPath, webRootPath, assemblyPath });
        }

        [HttpGet("install-driver")]
        public async Task<ActionResult> InstallAsync()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(environmentDir)) { return BadRequest("Environment variable HOME not found"); }
            var downloadDir = Path.Combine(environmentDir, "download");
            var destinationDir = Path.Combine(environmentDir, "util");
            var geckoDir = Path.Combine(destinationDir, "gecko");
            var geckoFile = Path.Combine(geckoDir, "geckodriver");
            var currentPaths = Environment.GetEnvironmentVariable("PATH")?.Split(':').ToList() ?? [];
            var paths = new [] { downloadDir, destinationDir, geckoDir }.ToList();
            paths.ForEach(path => {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            });
            var downloadPath = Path.Combine(downloadDir, "geckodriver.tar.gz");
            await DownloadGeckoAsync(downloadPath);
            await SaveGeckoDriverAsync(downloadPath, geckoDir);
            if (System.IO.File.Exists(geckoFile) && !currentPaths.Contains(geckoDir))
            {
                currentPaths.Add(geckoDir);
                Environment.SetEnvironmentVariable("PATH", string.Join(":", currentPaths));
            }
            var response = new { environmentDir, downloadDir };
            return Ok(response);
        }

        [HttpGet("verify-driver")]
        public ActionResult Verify()
        {
            FirefoxDriver? driver = null;
            try
            {
                var environmentDir = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(environmentDir)) { return BadRequest("Environment variable HOME not found"); }
                var downloadDir = Path.Combine(environmentDir, "download");
                var profile = new FirefoxOptions();
                profile.AddAdditionalCapability("platform", "LINUX", true);
                profile.AddAdditionalCapability("video", "True", true);
                profile.SetPreference("download.default_directory", downloadDir);
                profile.SetPreference("browser.safebrowsing.enabled", true);
                profile.SetPreference("browser.safebrowsing.malware.enabled", true);
                profile.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
                driver = new FirefoxDriver(profile);
                return Ok("Driver is working");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
            finally
            {
                driver?.Dispose();
            }
        }

        private static async Task DownloadGeckoAsync(string downloadPath)
        {
            if (System.IO.File.Exists(downloadPath)) return;
            var httpClient = new HttpClient();
            var responseStream = await httpClient.GetStreamAsync(_driverPath);
            using var fileStream = new FileStream(downloadPath, FileMode.Create);
            await responseStream.CopyToAsync(fileStream);
        }
        private static async Task SaveGeckoDriverAsync(string downloadPath, string outputDirectory, CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(outputDirectory) && Directory.GetFiles(outputDirectory, "*.*").Length > 0) return;
            await using var tarStream = new FileStream(downloadPath, new FileStreamOptions { Mode = FileMode.Open, Access = FileAccess.Read, Options = FileOptions.Asynchronous });
            await using MemoryStream memoryStream = new();
            await using (GZipStream gzipStream =
                new(tarStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(memoryStream, cancellationToken);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            await TarFile.ExtractToDirectoryAsync(
                memoryStream,
                outputDirectory,
                overwriteFiles: true,
                cancellationToken: cancellationToken
            );
        }
        private const string _driverPath = "https://github.com/mozilla/geckodriver/releases/download/v0.35.0/geckodriver-v0.35.0-linux64.tar.gz";
    }
}
