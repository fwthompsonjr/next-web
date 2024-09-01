using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace next.processor.api.utility
{
    public static class EnvironmentHelper
    {
        public static string? GetHomeFolder(IConfiguration? configuration = null)
        {
            var home = GetHomeOrDefault();
            var local = GetAppOrDefault();
            var data = GetDataOrDefault();
            var dataDir = GetDataDirectoryOrDefault(configuration);
            if (!string.IsNullOrEmpty(dataDir)) return dataDir;
            if (!string.IsNullOrEmpty(home)) return home;
            if (!string.IsNullOrEmpty(local)) return local;
            if (!string.IsNullOrEmpty(data)) return data;
            return null;
        }

        internal static void AppendToPath(string? keyValue)
        {
            const char colon = ':';
            const char semicolon = ';';
            const string name = "PATH";
            if (string.IsNullOrEmpty(keyValue)) return;
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var separator = isWindows ? semicolon : colon;
            var scope = EnvironmentVariableTarget.User;
            var oldValue = Environment.GetEnvironmentVariable(name, scope) ?? string.Empty;
            var items = oldValue.Split(separator).ToList();
            if (items.Contains(keyValue)) return;
            items.Add(keyValue);
            var newValue = string.Join(separator, items);
            Environment.SetEnvironmentVariable(name, newValue, scope);
        }

        [ExcludeFromCodeCoverage]
        internal static string? GetHomeOrDefault()
        {
            try
            {
                var home = Environment.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.User);
                return home;
            }
            catch
            {
                return string.Empty;
            }
        }

        [ExcludeFromCodeCoverage]
        internal static string? GetAppOrDefault()
        {
            try
            {
                var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
                return local;
            }
            catch
            {
                return string.Empty;
            }
        }

        [ExcludeFromCodeCoverage]
        internal static string? GetDataOrDefault()
        {
            try
            {
                var local = Environment.GetEnvironmentVariable("LocalAppData");
                return local;
            }
            catch
            {
                return string.Empty;
            }
        }


        [ExcludeFromCodeCoverage]
        internal static string? GetDataDirectoryOrDefault(IConfiguration? configuration = null)
        {
            try
            {
                if (configuration == null) return null;
                var local = configuration[Constants.DataDirectory];
                if (string.IsNullOrEmpty(local)) return string.Empty;
                if (!Directory.Exists(local)) return string.Empty;
                return local;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
