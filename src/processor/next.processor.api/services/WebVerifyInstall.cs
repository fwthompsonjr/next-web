using next.processor.api.interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;

namespace next.processor.api.services
{
    public class WebVerifyInstall : IWebContainerInstall
    {
        public bool IsInstalled { get; protected set; }
        public virtual async Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            var binaryFile = GetBinaryFileName();
            if (string.IsNullOrEmpty(binaryFile)) return false;
            var isverified = await Task.Run(() =>
            {
                FirefoxDriver? driver = null;
                try
                {
                    var environmentDir = Environment.GetEnvironmentVariable("HOME");
                    if (string.IsNullOrEmpty(environmentDir) ||
                        string.IsNullOrEmpty(DriverDirectory))
                    {
                        IsInstalled = false;
                        return false;
                    }

                    var downloadDir = Path.Combine(environmentDir, "download");
                    driver = GetDriver(1, downloadDir);
                    IsInstalled = true;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
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
        protected static FirefoxDriver GetDriver(int mode, string downloadDir)
        {
            var options = GetOptions(mode, downloadDir);
            var driver = mode switch
            {
                0 => new FirefoxDriver(DriverDirectory, options),
                1 => new FirefoxDriver(options),
                _ => new FirefoxDriver()
            };
            return driver;
        }

        private static string DriverDirectory => driverDirectory ??= GetDriverDirectoryName();
        private static string? driverDirectory;
        private static string GetDriverDirectoryName()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
            var destinationDir = Path.Combine(environmentDir, "util");
            var geckoDir = Path.Combine(destinationDir, "gecko");
            return geckoDir;
        }

        private static string GetBinaryFileName()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
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