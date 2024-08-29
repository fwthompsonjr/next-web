using next.processor.api.interfaces;
using next.processor.api.utility;
using System.Diagnostics.CodeAnalysis;

namespace next.processor.api.services
{
    public class StatusChangeService(IConfiguration configuration) : IStatusChanger
    {
        private readonly IConfiguration config = configuration;
        public bool AllowModelChanges { get; set; } = true;
        public void ChangeStatus(string status)
        {
            var clearName = changeTypes.Find(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
            if (clearName == null) return;
            switch (clearName)
            {
                case "errors":
                    ClearErrorCollection();
                    return;
                case "start":
                    config[Constants.KeyServiceInstallation] = "true";
                    config[Constants.KeyQueueProcessEnabled] = "true";
                    return;
                case "stop":
                    config[Constants.KeyServiceInstallation] = "false";
                    config[Constants.KeyQueueProcessEnabled] = "false";
                    return;
                case "toggle-installation":
                    var installation = Constants.KeyServiceInstallation;
                    ToggleBooleanConfiguration(installation);
                    return;
                case "toggle-queue":
                    var queue = Constants.KeyQueueProcessEnabled;
                    ToggleBooleanConfiguration(queue);
                    return;
            }
        }

        private void ToggleBooleanConfiguration(string keyName)
        {
            var exists = config[keyName] != null;
            if (!exists) return;
            var change = !config.GetValue<bool>(keyName);
            var text = change ? "true" : "false";
            config[keyName] = text;
        }

        [ExcludeFromCodeCoverage(Justification = "Behavior executes tested code from shared static class.")]
        private void ClearErrorCollection()
        {
            if (!AllowModelChanges) return;
            var selection = TrackEventService.Models.Find(x => x.Name == Constants.ErrorLogName);
            if (selection != null) TrackEventService.Models.Remove(selection);
        }

        private static readonly List<string> changeTypes = [
            "errors",
            "start",
            "stop",
            "toggle-installation",
            "toggle-queue"
        ];
    }
}
