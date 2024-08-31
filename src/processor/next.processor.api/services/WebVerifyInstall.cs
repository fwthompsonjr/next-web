using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Runtime.InteropServices;

namespace next.processor.api.services
{
    public class WebVerifyInstall(IConfiguration configuration) : IWebContainerInstall
    {
        private readonly IConfiguration config = configuration;
        public bool IsInstalled { get; protected set; }
        public string LastErrorMessage { get; protected set; } = string.Empty;
        public virtual async Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            var isverified = await Task.Run(() =>
            {
                FirefoxDriver? driver = null;
                try
                {
                    var driverDir = GetDriverDirectoryName(config);
                    var environmentDir = EnvironmentHelper.GetHomeFolder(config);
                    if (string.IsNullOrEmpty(environmentDir))
                        throw new Exception("Environment directory not found");
                    if (string.IsNullOrEmpty(environmentDir) ||
                        string.IsNullOrEmpty(driverDir))
                    {
                        IsInstalled = false;
                        return false;
                    }

                    var downloadDir = Path.Combine(environmentDir, "download");
                    driver = GetDriver(1, downloadDir);
                    driver.Navigate().GoToUrl("https://www.google.com");
                    IsInstalled = true;
                    return true;
                }
                catch (Exception ex)
                {
                    ex.Log();
                    IsInstalled = false;
                    return false;
                }
                finally
                {
                    driver?.Dispose();
                }
            });
            return isverified;
        }

        private static FirefoxOptions GetOptions(int mode, string downloadDir, IConfiguration cfg)
        {

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var profile = new FirefoxOptions();
            var binaryFile = GetBinaryFileName(cfg);
            if (mode == 0 || File.Exists(binaryFile) && !isWindows)
            {
                profile.BrowserExecutableLocation = binaryFile;
            }
            if (!isWindows)
            {
                profile.AddAdditionalCapability("platform", "LINUX", true);
            }
            profile.AddArguments("--headless");
            profile.AddAdditionalCapability("video", "True", true);
            profile.SetPreference("download.default_directory", downloadDir);
            profile.SetPreference("browser.safebrowsing.enabled", true);
            profile.SetPreference("browser.safebrowsing.malware.enabled", true);
            profile.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
            return profile;
        }
        protected virtual FirefoxDriver GetDriver(int mode, string downloadDir)
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var options = GetOptions(mode, downloadDir, config);
            var driverDir = GetDriverDirectoryName(config);
            var driver = mode switch
            {
                0 => new FirefoxDriver(driverDir, options),
                1 => isWindows ? new FirefoxDriver(options) : new FirefoxDriver(driverDir, options),
                _ => new FirefoxDriver()
            };
            return driver;
        }

        private static string GetDriverDirectoryName(IConfiguration configuration1)
        {
            var environmentDir = EnvironmentHelper.GetHomeFolder(configuration1);
            if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
            var destinationDir = Path.Combine(environmentDir, "util");
            var geckoDir = Path.Combine(destinationDir, "gecko");
            return geckoDir;
        }

        private static string? GetBinaryFileName(IConfiguration? cfg = null)
        {
            const string ffox = "firefox";
            var environmentDir = EnvironmentHelper.GetHomeFolder(cfg);
            if (string.IsNullOrEmpty(environmentDir)) { return null; }
            var firefoxDir = Path.Combine(environmentDir, ffox);
            var subfolders = 0;
            var firefoxFile = Path.Combine(firefoxDir, ffox);
            while (!File.Exists(firefoxFile))
            {
                if (subfolders > 5) return string.Empty;
                firefoxFile = Path.Combine(firefoxFile, ffox);
                subfolders++;
            }
            return firefoxFile;
        }
    }
}