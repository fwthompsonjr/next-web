using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.utility;
using System.Diagnostics;
using System.Reflection;

namespace next.processor.api.services
{
    public class WebFireFoxWindowsInstall(IWebInstallOperation webInstallOperation) : BaseWebInstall(webInstallOperation)
    {

        public async override Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            try
            {
                LastErrorMessage = string.Empty;
                var environmentDir = EnvironmentHelper.GetHomeFolder();
                if (string.IsNullOrEmpty(environmentDir)) { return false; }
                var destinationDir = Path.Combine(environmentDir, "mozilla-win");
                var mozillaDir = Path.Combine(destinationDir, "install");
                var firefoxFile = Path.Combine(destinationDir, "firefox-installation.txt");
                if (_fileSvc.FileExists(firefoxFile))
                {
                    IsInstalled = true;
                    return true;
                }
                var paths = new[] { destinationDir, mozillaDir }.ToList();
                paths.ForEach(path => { _fileSvc.CreateDirectory(path); });
                var installation = await ExtractFileAsync(mozillaDir);
                if (!installation) return false;
                IsInstalled = _fileSvc.FileExists(firefoxFile);
                return IsInstalled;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.ToString();
                ex.Log();
                IsInstalled = false;
                return false;
            }
        }

        private async Task<bool> ExtractFileAsync(
            string installDirectory,
            CancellationToken cancellationToken = default)
        {
            const string uri = "https://download.mozilla.org/?product=firefox-stub";
            var fullName = Path.Combine(installDirectory, "Firefox_Installer.exe");
            var confirmName = Path.Combine(installDirectory, "firefox-installation.txt");
            if (!_fileSvc.FileExists(fullName))
            {
                var isdownloaded = await _fileSvc.DownloadFromUriAsync(uri, fullName, cancellationToken);
                if(!isdownloaded) return isdownloaded;
            }
            var isinstalled = InstallExe(fullName, confirmName);
            if (!isinstalled) return false;
            return _fileSvc.FileExists(confirmName);
        }

        private static bool InstallExe(string fullName, string confirmationFile)
        {
            try
            {
                var psi = new ProcessStartInfo(fullName)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = "/S",
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                // wrap IDisposable into using (in order to release hProcess) 
                using Process process = new();
                process.StartInfo = psi;
                process.Start();

                // Add this: wait until process does its work
                process.WaitForExit();

                // and only then read the result
                string errors = process.StandardError.ReadToEnd();
                string result = process.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
                if (process.ExitCode != 0 || errors.Length > 0) return false;
                var obj = new
                {
                    status = "ok",
                    installationDate = DateTime.UtcNow,
                }.ToJsonString();
                File.WriteAllText(confirmationFile, obj);
                return true;
            }
            catch (Exception ex)
            {
                ex.Log();
                return false;
            }
        }
    }
}