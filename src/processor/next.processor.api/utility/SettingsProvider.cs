using Newtonsoft.Json;
using next.processor.api.models;
using System.Text;

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
        public static IConfiguration Configuration => _configuration ??= GetConfiguration();

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


        private static IConfiguration? _configuration;

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            // GetConfigJson method should get the JSON string from the source.
            // I am leaving the implementation of that method up to you.
            var jsonData = GetConfigJson();

            // Load the JSON into MemoryStream
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));

            // Call AddJsonStream method of builder by passing the stream object.
            builder.AddJsonStream(stream);
            return builder.Build();
        }

        private static string GetConfigJson()
        {
            return Properties.Resources.appsettings;
        }
    }
}
