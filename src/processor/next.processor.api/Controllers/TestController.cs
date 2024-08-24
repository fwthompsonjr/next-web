using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
namespace next.processor.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IServiceProvider provider) : ControllerBase
    {
        private readonly IServiceProvider _provider = provider;
        [HttpGet("install-browser")]
        public async Task<ActionResult> BrowserInstallAsync()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(environmentDir)) { return BadRequest("Environment variable HOME not found"); }
            var service = _provider.GetKeyedService<IWebContainerInstall>("firefox");
            if (service == null) { return BadRequest("Unable to create installation instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted) { return BadRequest("Failed to install firefox component"); }
            return Ok("firefox has been installed.");
        }

        [HttpGet("install-driver")]
        public async Task<ActionResult> InstallAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("geckodriver");
            if (service == null) { return BadRequest("Unable to create installation instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted) { return BadRequest("Failed to install geckodriver component"); }
            return Ok("geckodriver has been installed.");
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

    }
}
