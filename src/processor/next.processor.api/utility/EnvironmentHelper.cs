using System.Diagnostics.CodeAnalysis;

namespace next.processor.api.utility
{
    public static class EnvironmentHelper
    {
        public static string? GetHomeFolder()
        {
            var home = GetHomeOrDefault();
            var local = GetAppOrDefault();
            if (!string.IsNullOrEmpty(home)) return home;
            if (!string.IsNullOrEmpty(local)) return local;
            return null;
        }
        [ExcludeFromCodeCoverage]
        private static string? GetHomeOrDefault()
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
        private static string? GetAppOrDefault()
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
    }
}
