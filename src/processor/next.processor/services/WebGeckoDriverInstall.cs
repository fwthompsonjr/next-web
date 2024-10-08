﻿using Microsoft.Extensions.Configuration;
using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.utility;
using System.Diagnostics.CodeAnalysis;

namespace next.processor.api.services
{
    [ExcludeFromCodeCoverage(Justification = "Intergration testing only. Interacts with remote services")]
    public class WebGeckoDriverInstall(
        IWebInstallOperation webInstallOperation,
        IConfiguration configuration) : BaseWebInstall(webInstallOperation)
    {
        private readonly IConfiguration config = configuration;
        public async override Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            try
            {
                var environmentDir = EnvironmentHelper.GetHomeFolder(config);
                if (string.IsNullOrEmpty(environmentDir))
                    throw new Exception("Environment directory not found");

                var downloadDir = Path.Combine(environmentDir, "download");
                var destinationDir = Path.Combine(environmentDir, "util");
                var geckoDir = Path.Combine(destinationDir, "gecko");
                var geckoFile = Path.Combine(geckoDir, "geckodriver");
                IsInstalled = _fileSvc.FileExists(geckoFile);
                if (IsInstalled)
                {
                    EnvironmentHelper.AppendToPath(geckoDir);
                    return true;
                }
                var paths = new[] { downloadDir, destinationDir, geckoDir }.ToList();
                paths.ForEach(path => _fileSvc.CreateDirectory(path));
                var uri = _driverPath;
                var downloadPath = Path.Combine(downloadDir, "geckodriver.tar.gz");
                var isdownloaded = await _fileSvc.DownloadDriverAsync(uri, downloadPath, default);
                if (!isdownloaded) return false;
                var extracted = await _fileSvc.ExtractTarToDirectoryAsync(downloadPath, geckoDir, default);
                if (!extracted) return false;
                IsInstalled = _fileSvc.FileExists(geckoFile);
                if (!IsInstalled) return false;
                EnvironmentHelper.AppendToPath(geckoDir);
                return IsInstalled;
            }
            catch (Exception ex)
            {
                ex.Log(_source);
                IsInstalled = false;
                return false;
            }
        }
        private const string _source = "linux-gecko-installer";
    }
}