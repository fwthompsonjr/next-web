using Microsoft.VisualStudio.Threading;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;

namespace next.processor.api.backing
{
    public class NonPersonQueueService(IApiWrapper api) : BaseTimedSvc<NonPersonQueueService>(GetSettings())
    {
        protected bool IsRunnning { get; set; }
        private readonly IApiWrapper _api = api;
        private readonly JoinableTaskFactory jtf = new(new JoinableTaskContext());
        protected override void DoWork(object? state)
        {
            if (IsRunnning) return;
            IsRunnning = true;
            _ = jtf.RunAsync(async () =>
            {
                try
                {
                    var list = await _api.FetchNonPersonAsync();
                    if (list == null || list.Count == 0) { return; }
                    foreach (var item in list)
                    {
                        await _api.PostSaveNonPersonAsync(item);
                    }
                }
                finally
                {
                    IsRunnning = false;
                }
            });
        }

        protected override string GetHealth()
        {
            return string.Empty;
        }

        protected override string GetStatus()
        {
            return string.Empty;
        }

        private static ServiceSettings GetSettings()
        {
            return TheSettingsProvider.GetSettingOrDefault("non.person.service").Setting;
        }

    }
}