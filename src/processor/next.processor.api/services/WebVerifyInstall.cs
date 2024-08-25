using next.processor.api.interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;

namespace next.processor.api.services
{
    public class WebVerifyInstall : IWebContainerInstall
    {
        public bool IsInstalled { get; private set; }
        public async Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            var isverified = await Task.Run(() =>
            {
                FirefoxDriver? driver = null;
                try
                {
                    var environmentDir = Environment.GetEnvironmentVariable("HOME");
                    if (string.IsNullOrEmpty(environmentDir) ||
                        string.IsNullOrEmpty(DriverDirectory) ||
                        string.IsNullOrEmpty(BinaryFile))
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
                profile.BrowserExecutableLocation = BinaryFile;
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
        private static FirefoxDriver GetDriver(int mode, string downloadDir)
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



        private static string BinaryFile => binaryFile ??= GetBinaryFileName();
        private static string? binaryFile;
        private static string GetBinaryFileName()
        {
            var environmentDir = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
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