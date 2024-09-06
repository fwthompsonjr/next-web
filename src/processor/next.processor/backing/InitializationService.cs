using Microsoft.Extensions.Configuration;
using next.processor.api.models;
using next.processor.api.utility;

namespace next.processor.api.backing
{
    public class InitializationService(IConfiguration config) : BaseTimedSvc<InitializationService>(GetSettings())
    {
        private readonly IConfiguration _configuration = config;
        protected override void DoWork(object? state)
        {
            if (isInitialized) return;
            var current = _configuration.GetValue<bool>(Constants.KeyServiceInstallation);
            if (!current)
            {
                _configuration[Constants.KeyServiceInstallation] = "true";
            }
            isInitialized = true;
        }

        protected override string GetHealth()
        {
            return $"{serviceName} is healthy";
        }

        protected override string GetStatus()
        {
            return $"{serviceName} is ready";
        }

        private static ServiceSettings GetSettings()
        {
            return SettingsProvider.GetSettingOrDefault("initialization.service").Setting;
        }
        private static bool isInitialized = false;
        private const string serviceName = "Initialization Service";
    }
}