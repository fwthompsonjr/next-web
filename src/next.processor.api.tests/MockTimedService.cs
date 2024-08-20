using next.processor.api.backing;
using next.processor.api.models;

namespace next.processor.api.tests
{
    internal class MockTimedService(
        bool isEnabled = true,
        int delay = 30,
        int interval = 2,
        bool isNull = false) : 
        BaseTimedSvc<MockTimedService>(GetSetting(isEnabled, delay, interval, isNull))
    {

        public async Task Execute()
        {
            using CancellationTokenSource cts = new();
            CancellationToken token = cts.Token;
            await StartAsync(token);
        }

        public async Task Stop()
        {
            using CancellationTokenSource cts = new();
            CancellationToken token = cts.Token;
            await StopAsync(token);
        }

        public void RunTimer()
        {
            OnTimer(null);
        }
        protected override void DoWork(object? state)
        {
            Console.WriteLine("Do work method has been called");
        }

        protected override string GetHealth()
        {
            return "Mock service is healthy";
        }

        protected override string GetStatus()
        {
            return "Mock service is ready";
        }

        private static ServiceSettings? GetSetting(
            bool isEnabled = true,
            int delay = 30,
            int interval = 2,
            bool isNull = false)
        {
            if (isNull) return default;
            return new ServiceSettings
            {
                Enabled = isEnabled,
                Delay = delay,
                Interval = interval
            };
        }

    }
}
