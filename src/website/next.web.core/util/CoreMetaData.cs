using next.web.core.models;

namespace next.web.core.util
{
    internal static class CoreMetaData
    {
        public static string GetKeyOrDefault(string key, string fallback)
        {
            if (common.Count == 0) { Populate(); }
            if (!common.TryGetValue(key, out string? value)) { return fallback; }
            if (string.IsNullOrEmpty(value)) return fallback;
            return value;
        }

        private static readonly Dictionary<string, string> common = new();
        private static readonly List<string> _commonkeys =
            [.. "app.meta:name;app.meta:prefix".Split(';')];
        private static void Populate()
        {
            var configuration = new CoreConfigurationModel().GetConfiguration;
            foreach (var item in _commonkeys)
            {
                if (common.ContainsKey(item)) continue;
                var value = configuration[item] ?? string.Empty;
                common[item] = value;
            }
        }
    }
}
