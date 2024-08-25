using next.processor.api.interfaces;
using next.processor.api.models;
using System.Diagnostics.CodeAnalysis;

namespace next.processor.api.backing
{
    public class QueueExecutor(
        IServiceProvider services,
        IApiWrapper api) : IQueueExecutor
    {
        private readonly IServiceProvider _provider = services;
        private readonly IApiWrapper _api = api;

        public bool IsRunning { get; private set; }

        public bool? IsReady()
        {
            var installers = _installNames.Select(GetInstaller).ToList();
            foreach (var installer in installers)
            {
                if (installer == null) return null;
                if (!installer.IsInstalled) return false;
            }
            return true;
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
            var name = _installNames.Find(x => x.Equals(queueName, oic));
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
                var isready = await CanExecute();
                if (!isready) return;
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
                }
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

        private async Task<bool> CanExecute()
        {
            var installers = _installNames.Select(x => GetInstaller(x)).ToList();
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
            return !responses.Exists(a => !a);
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
        private static readonly List<string> _installNames = ["firefox", "geckodriver", "verification"];
        private static readonly object locker = new();
    }
}
