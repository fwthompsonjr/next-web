using next.processor.api.interfaces;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace next.processor.api.services
{
    public class WebVerifyInstall : IWebContainerInstall
    {
        public async Task<bool> InstallAsync()
        {
            var isverified = await Task.Run(() =>
            {
                FirefoxDriver? driver = null;
                try
                {
                    var environmentDir = Environment.GetEnvironmentVariable("HOME");
                    if (string.IsNullOrEmpty(environmentDir) ||
                        string.IsNullOrEmpty(DriverDirectory) ||
                        string.IsNullOrEmpty(BinaryFile)) { return false; }

                    var downloadDir = Path.Combine(environmentDir, "download");
                    var profile = new FirefoxOptions
                    {
                        BrowserExecutableLocation = BinaryFile
                    };
                    profile.AddAdditionalCapability("platform", "LINUX", true);
                    profile.AddAdditionalCapability("video", "True", true);
                    profile.SetPreference("download.default_directory", downloadDir);
                    profile.SetPreference("browser.safebrowsing.enabled", true);
                    profile.SetPreference("browser.safebrowsing.malware.enabled", true);
                    profile.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
                    driver = new FirefoxDriver(DriverDirectory, profile);
                    return true;
                }
                catch(Exception ex) 
                {
                    Debug.WriteLine(ex);
                    return false;
                }
                finally
                {
                    driver?.Dispose();
                }
            });
            return isverified;
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