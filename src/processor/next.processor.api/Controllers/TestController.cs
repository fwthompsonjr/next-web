using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.AspNetCore.Mvc;
using next.processor.api.utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Reflection;
namespace next.processor.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController() : ControllerBase
    {

        [HttpGet("install-browser")]
        public async Task<ActionResult> BrowserInstallAsync()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(environmentDir)) { return BadRequest("Environment variable HOME not found"); }
            var destinationDir = Path.Combine(environmentDir, "mozilla");
            var mozillaDir = Path.Combine(destinationDir, "install");
            var firefoxDir = Path.Combine(environmentDir, "firefox");
            var paths = new[] { destinationDir, mozillaDir, firefoxDir }.ToList();
            paths.ForEach(path => { if (!Directory.Exists(path)) Directory.CreateDirectory(path); });

            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyPath = entryAssembly == null ? string.Empty : Path.GetDirectoryName(entryAssembly.Location);

            if (string.IsNullOrEmpty(assemblyPath) || !Directory.Exists(assemblyPath)) return BadRequest("Application dir is not found.");
            var files = Directory.GetFiles(assemblyPath, "*.bz2", SearchOption.AllDirectories);
            var zipfile = files.Length > 0 ? files[0] : string.Empty;
            var zipfilename = Path.GetFileName(zipfile);
            if (string.IsNullOrEmpty(zipfile) || string.IsNullOrWhiteSpace(zipfilename)) return BadRequest("Application zip file is not found.");

            await ExtractBzFileAsync(mozillaDir, firefoxDir, zipfilename);
            var currentPaths = Environment.GetEnvironmentVariable("PATH")?.Split(':').ToList() ?? [];
            var firefoxFile = Path.Combine(firefoxDir, "firefox");
            if (System.IO.File.Exists(firefoxFile) && !currentPaths.Contains(firefoxDir))
            {
                currentPaths.Add(firefoxDir);
                Environment.SetEnvironmentVariable("PATH", string.Join(":", currentPaths));
            }
            return Ok(new { zipfile });
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
            await ExtractTarFileAsync(downloadPath, geckoDir);
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
        private static async Task ExtractTarFileAsync(string downloadPath, string outputDirectory, CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(outputDirectory) && Directory.GetFiles(outputDirectory, "*.*", SearchOption.AllDirectories).Length > 0) return;
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
        private static async Task ExtractBzFileAsync(
            string downloadDirectory,
            string installDirectory,
            string zipFileName, 
            CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(installDirectory) && Directory.GetFiles(installDirectory, "*.*", SearchOption.AllDirectories).Length > 0) return;
            var fullName = Path.Combine(downloadDirectory, zipFileName);
            Debug.WriteLine("Extracting bz2 file: {0}", Path.GetFileName(fullName));
            if (!System.IO.File.Exists(fullName))
            {
                var browserPath = GetFireFoxDownloadUri(_firefoxVersion);
                using var httpClient = new HttpClient();
                using MemoryStream ms = new();
                using var responseStream = await httpClient.GetStreamAsync(browserPath, cancellationToken);
                await responseStream.CopyToAsync(ms, cancellationToken);
                var contents = ms.ToArray();
                await System.IO.File.WriteAllBytesAsync(fullName, contents, cancellationToken);
            }
            if (Directory.Exists(installDirectory)) Directory.CreateDirectory(installDirectory);
            using var stream = new BZip2InputStream(System.IO.File.OpenRead(fullName));
            await TarFile.ExtractToDirectoryAsync(stream, installDirectory, true, cancellationToken);
        }


        private static string GetFireFoxDownloadUri(string version)
        {
            var address = "https://download-installer.cdn.mozilla.net/pub/firefox/releases/{0}" +
                "/linux-x86_64/en-US/firefox-{0}.tar.bz2";
            return string.Format(address, version);
        }
        private const string _firefoxVersion = "129.0.2";
        private const string _driverPath = "https://github.com/mozilla/geckodriver/releases/download/v0.35.0/geckodriver-v0.35.0-linux64.tar.gz";
    }
}
