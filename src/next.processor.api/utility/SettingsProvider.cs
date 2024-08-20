using Newtonsoft.Json;
using next.processor.api.models;

namespace next.processor.api.utility
{
    internal static class SettingsProvider
    {
        public static NamedServiceSetting GetSettingOrDefault(string name)
        {
            var fallback = new NamedServiceSetting { Name = name };
            var find = Settings.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return find ?? fallback;
        }

        private static string? backgroundSettings;
        private static List<NamedServiceSetting>? settings;
        private static string BackgroundSetting => backgroundSettings ??= GetBackgroundSettings();
        private static List<NamedServiceSetting> Settings => settings ??= GetSettings();
        private static List<NamedServiceSetting> GetSettings()
        {
            var list = JsonConvert.DeserializeObject<List<NamedServiceSetting>>(BackgroundSetting) ?? [];
            if (list.Count == 0) list.Add(new());
            return list;
        }
        private static string GetBackgroundSettings()
        {
            return Properties.Resources.background_settings_list;
        }
    }
}
