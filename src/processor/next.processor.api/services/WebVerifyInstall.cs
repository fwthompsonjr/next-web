using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Runtime.InteropServices;

namespace next.processor.api.services
{
    public class WebVerifyInstall : IWebContainerInstall
    {
        public bool IsInstalled { get; protected set; }
        public string LastErrorMessage { get; protected set; } = string.Empty;
        public virtual async Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            LastErrorMessage = string.Empty;
            var isverified = await Task.Run(() =>
            {
                FirefoxDriver? driver = null;
                try
                {
                    var driverDir = GetDriverDirectoryName();
                    var environmentDir = EnvironmentHelper.GetHomeFolder();
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
                    LastErrorMessage = ex.ToString();
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

        private static FirefoxOptions GetOptions(int mode, string downloadDir)
        {

            var profile = new FirefoxOptions();
            if (mode == 0)
            {

                var binaryFile = GetBinaryFileName();
                profile.BrowserExecutableLocation = binaryFile;
            }

            profile.AddArguments("-headless");
            profile.AddArguments("--headless");
            profile.AddAdditionalCapability("platform", "LINUX", true);
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
            var options = GetOptions(mode, downloadDir);
            var driverDir = GetDriverDirectoryName();
            var driver = mode switch
            {
                0 => new FirefoxDriver(driverDir, options),
                1 => isWindows ? new FirefoxDriver(options) : new FirefoxDriver(driverDir, options),
                _ => new FirefoxDriver()
            };
            return driver;
        }

        private static string GetDriverDirectoryName()
        {
            var environmentDir = EnvironmentHelper.GetHomeFolder();
            if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
            var destinationDir = Path.Combine(environmentDir, "util");
            var geckoDir = Path.Combine(destinationDir, "gecko");
            return geckoDir;
        }

        private static string? GetBinaryFileName()
        {
            var environmentDir = EnvironmentHelper.GetHomeFolder();
            if (string.IsNullOrEmpty(environmentDir)) { return null; }
            var firefoxDir = Path.Combine(environmentDir, "firefox");
            var subfolders = 0;
            var firefoxFile = Path.Combine(firefoxDir, "firefox");
            while (!File.Exists(firefoxFile))
            {
                if (subfolders > 5) return string.Empty;
                firefoxFile = Path.Combine(firefoxFile, "firefox");
                subfolders++;
            }
            return firefoxFile;
        }
    }
}