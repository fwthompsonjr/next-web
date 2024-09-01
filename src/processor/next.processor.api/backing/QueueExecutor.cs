using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace next.processor.api.backing
{
    public class QueueExecutor(
        IServiceProvider services,
        IApiWrapper api,
        IConfiguration configuration) : IQueueExecutor
    {
        private readonly IServiceProvider _provider = services;
        private readonly IApiWrapper _api = api;
        private readonly IConfiguration _configuration = configuration;

        public bool IsRunning { get; private set; }

        public bool? IsReady()
        {
            var installers = GetNames().Select(GetInstaller).ToList();
            foreach (var installer in installers)
            {
                if (installer == null) return null;
                if (!installer.IsInstalled) return false;
            }
            return true;
        }
        public int IsReadyCount()
        {
            var installers = GetNames().Select(GetInstaller).ToList();
            var count = installers.Count(x => x != null && x.IsInstalled);
            return count;
        }
        public int InstallerCount() => GetNames().Count;

        public Dictionary<string, object> GetDetails()
        {
            var details = new Dictionary<string, object>();
            var installers = GetNames().Select(x =>
            {
                var obj = GetInstaller(x);
                var name = x.Split('-')[^1];
                var status = obj?.IsInstalled switch
                {
                    true => " is installed.",
                    false => "is not installed",
                    _ => "is not initialized."
                };
                return new { name, status };
            }
            ).ToList();
            installers.ForEach(i =>
            {
                details.Add(i.name, $"{i.name} : {i.status}");
            });
            return details;
        }

        public IQueueProcess? GetInstance(string queueName)
        {
            var oic = StringComparison.OrdinalIgnoreCase;
            var name = _queueNames.Find(x => x.Equals(queueName, oic));
            if (name == null) return null;
            return _provider.GetKeyedService<IQueueProcess>(name);
        }

        public IWebContainerInstall? GetInstaller(string queueName)
        {
            var oic = StringComparison.OrdinalIgnoreCase;
            var name = GetNames().Find(x => x.Equals(queueName, oic));
            if (name == null) return null;
            return _provider.GetKeyedService<IWebContainerInstall>(name);
        }


        private QueuedRecord? Current;
        public async Task ExecuteAsync()
        {
            lock (locker)
            {
                if (IsRunning) return;
                IsRunning = true;
            }
            try
            {
                var isready = await CanExecuteAsync();
                if (!isready) return;
                await ExecuteBatchAsync();
            }
            catch (Exception ex)
            {
                await ReportIssueAsync(ex);
            }
            finally
            {
                Current = null;
                IsRunning = false;
            }
        }

        private bool IsQueueServiceAvailable()
        {
            return _configuration.GetValue<bool>(Constants.KeyQueueProcessEnabled);
        }

        [ExcludeFromCodeCoverage(Justification = "Deferred unit testing this method until other behaviors are complete.")]
        private async Task ExecuteBatchAsync()
        {
            var parent = GetInstance(_queueNames[0]);
            var children = _queueNames.Where(x => !x.Equals(_queueNames[0])).ToList();
            if (parent == null) return;
            var response = await parent.ExecuteAsync(null);
            if (response == null || response.CurrentBatch.Count == 0) return;
            var workers = children.Select(GetInstance).ToList();
            while (response.IterateNext())
            {
                Current = response.QueuedRecord;
                foreach (var worker in workers)
                {
                    if (worker == null) continue;
                    _ = await worker.ExecuteAsync(response);
                    if (!worker.IsSuccess) break;
                }
                if (!IsQueueServiceAvailable()) break;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member called and test from public accessor")]
        private async Task<bool> CanExecuteAsync()
        {
            var isInstallationEnabled = _configuration.GetValue<bool>(Constants.KeyServiceInstallation);
            if (!isInstallationEnabled) return false;
            var installers = GetNames().Select(GetInstaller).ToList();
            if (installers.Exists(x => x == null)) return false;
            var responses = new List<bool>();
            foreach (var installer in installers)
            {
                if (installer == null)
                {
                    responses.Add(false);
                    continue;
                }
                var rsp = await installer.InstallAsync();
                responses.Add(rsp);
            }
            var installationCompleted = !responses.Exists(a => !a);
            if (installationCompleted && enableQueueOnInstallation)
            {
                EnableQueueService();
                enableQueueOnInstallation = false;
            }
            var isQueueEnabled = IsQueueServiceAvailable();
            if (!isQueueEnabled) return false;
            return installationCompleted;
        }

        private void EnableQueueService()
        {
            _configuration[Constants.KeyQueueProcessEnabled] = "true";
        }

        [ExcludeFromCodeCoverage(Justification = "Private member called and test from public accessor")]
        private async Task ReportIssueAsync(Exception exception)
        {
            try
            {
                if (Current == null) return;
                await _api.ReportIssueAsync(Current, exception);
            }
            catch (Exception)
            {
                // do not report expections
            }
        }

        private static readonly List<string> _queueNames = ["begin", "parameter", "search"];
        private static readonly List<string> _installNames = [
            "linux-firefox",
            "linux-geckodriver",
            "read-collin",
            "read-denton",
            "read-harris",
            "read-tarrant"
        ];
        private static readonly object locker = new();
        private static List<string> GetNames()
        {
            const string linux = "linux";
            const string windows = "windows";
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var names = new List<string>(_installNames);
            if (isWindows) { names.RemoveAll(n => n.StartsWith(linux)); }
            else { names.RemoveAll(n => n.StartsWith(windows)); }
            return names;
        }
        private static bool enableQueueOnInstallation = true;
    }
}
