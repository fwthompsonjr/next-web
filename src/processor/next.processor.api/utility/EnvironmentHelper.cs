namespace next.processor.api.utility
{
    public static class EnvironmentHelper
    {
        public static string? GetHomeFolder()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(home)) return home;
            if (!string.IsNullOrEmpty(local)) return local;
            return null;
        }
    }
}
