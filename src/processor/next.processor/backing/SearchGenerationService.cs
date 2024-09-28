using Microsoft.VisualStudio.Threading;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;

namespace next.processor.api.backing
{
    public class SearchGenerationService(IQueueExecutor executor) : BaseTimedSvc<SearchGenerationService>(GetSettings())
    {
        private readonly IQueueExecutor _executor = executor;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "VSSDK005:Avoid instantiating JoinableTaskContext", Justification = "<Pending>")]
        private readonly JoinableTaskFactory jtf = new(new JoinableTaskContext());
        protected override void DoWork(object? state)
        {
            if (_executor.IsRunning) return;
            jtf.Run(_executor.ExecuteAsync);
        }

        protected override string GetHealth()
        {
            return "Search Generation Service is healthy";
        }

        protected override string GetStatus()
        {
            if (_executor.IsRunning)
            {
                return "Search Generation Service is running";
            }
            return "Search Generation Service is ready";
        }

        private static ServiceSettings GetSettings()
        {
            return TheSettingsProvider.GetSettingOrDefault("record.processor").Setting;
        }

    }
}
