using next.processor.api.interfaces;
using next.processor.api.services;
using next.processor.api.utility;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace next.processor.api.backing
{
    internal abstract class BaseTimedSvc<T> : IHostedService, IDisposable where T : class
    {
        private bool disposedValue;

        protected Timer? _timer = null;
        protected virtual int DelayedStartInSeconds { get; set; }
        protected virtual int IntervalInMinutes { get; set; }
        protected virtual bool IsServiceEnabled { get; set; }

        protected BaseTimedSvc(IBackgroundServiceSettings? settings)
        {
            settings ??= SettingsProvider.GetSettingOrDefault("").Setting;
            IsServiceEnabled = settings.Enabled;
            DelayedStartInSeconds = settings.Delay;
            IntervalInMinutes = settings.Interval;
        }

        protected abstract void DoWork(object? state);
        protected abstract string GetStatus();
        protected abstract string GetHealth();

        protected void OnTimer(object? state)
        {
            DataService.ReportState(GetStatus());
            DataService.ReportHealth(GetHealth());
            DoWork(state);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!IsServiceEnabled) return Task.CompletedTask;
            cancellationToken.ThrowIfCancellationRequested();
            var message = $"{typeof(T).Name} : {DateTime.Now:s} : Timed Process is starting";
            Console.WriteLine(message);
            _timer = new Timer(OnTimer, null, TimeSpan.FromSeconds(DelayedStartInSeconds), TimeSpan.FromMinutes(IntervalInMinutes));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var message = $"{typeof(T).Name} : {DateTime.Now:s} : Timed Process is stopping";
            Console.WriteLine(message);

            _timer?.Change(Timeout.Infinite, 0);
            StopDrivers();
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                    StopDrivers();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private static void StopDrivers()
        {
            var processes = new List<string> {
                "geckodriver",
                "chromedriver",
                "IEDriverServer" };
            processes.ForEach(Kill);
        }
        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private static void Kill(string processName)
        {
            var enumerable = Process.GetProcessesByName(processName);
            if (enumerable == null || enumerable.Length == 0) return;
            enumerable.ToList().ForEach(p => p.Kill());
        }
    }
}
