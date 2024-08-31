﻿using next.processor.api.extensions;
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
                var zipfilename = FirefoxShortName;
                if (string.IsNullOrEmpty(environmentDir) || string.IsNullOrWhiteSpace(zipfilename)) { return false; }
                var destinationDir = Path.Combine(environmentDir, "mozilla-win");
                var mozillaDir = Path.Combine(destinationDir, "install");
                var firefoxDir = Path.Combine(environmentDir, "firefox");
                if (DoesFileExist(firefoxDir))
                {
                    IsInstalled = true;
                    return true;
                }
                var paths = new[] { destinationDir, mozillaDir, firefoxDir }.ToList();
                paths.ForEach(path => { _fileSvc.CreateDirectory(path); });
                var installation = await ExtractBzFileAsync(mozillaDir, firefoxDir, zipfilename);
                if (!installation) return false;
                IsInstalled = DoesFileExist(firefoxDir);
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

        private bool DoesFileExist(string firefoxDir)
        {
            const string ffox = "firefox";
            var subfolders = 0;
            var firefoxFile = Path.Combine(firefoxDir, ffox);
            while (!_fileSvc.FileExists(firefoxFile))
            {
                if (subfolders > 5) return false;
                firefoxFile = Path.Combine(firefoxFile, ffox);
                subfolders++;
            }
            return _fileSvc.FileExists(firefoxFile);
        }

        private async Task<bool> ExtractBzFileAsync(
            string downloadDirectory,
            string installDirectory,
            string zipFileName,
            CancellationToken cancellationToken = default)
        {
            if (_fileSvc.DirectoryFileCount(installDirectory) > 0) return true;
            var fullName = Path.Combine(downloadDirectory, zipFileName);
            Debug.WriteLine("Extracting bz2 file: {0}", Path.GetFileName(fullName));
            if (!_fileSvc.FileExists(fullName))
            {
                var browserPath = GetFireFoxDownloadUri(_firefoxVersion);
                var isdownloaded = await _fileSvc.DownloadFromUriAsync(browserPath, fullName, cancellationToken);
                if (!isdownloaded) return false;
            }
            if (!_fileSvc.CreateDirectory(installDirectory)) return false;
            var extracted = await _fileSvc.ExtractGzipToDirectoryAsync(fullName, installDirectory, cancellationToken);
            return extracted;
        }

        private static string FirefoxShortName => firefoxShortName ??= GetTargetFileName();

        private static string? firefoxShortName;
        private static string GetTargetFileName()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyPath = entryAssembly == null ? string.Empty : Path.GetDirectoryName(entryAssembly.Location);

            if (string.IsNullOrEmpty(assemblyPath) || !Directory.Exists(assemblyPath)) return string.Empty;
            var files = Directory.GetFiles(assemblyPath, "*.bz2", SearchOption.AllDirectories);
            var zipfile = files.Length > 0 ? files[0] : string.Empty;
            var zipfilename = Path.GetFileName(zipfile);
            if (string.IsNullOrEmpty(zipfile) || string.IsNullOrWhiteSpace(zipfilename)) return string.Empty;
            return zipfilename;
        }


    }
}