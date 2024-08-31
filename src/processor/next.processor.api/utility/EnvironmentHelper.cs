namespace next.processor.api.utility
{
    public static class EnvironmentHelper
    {
        public static string? GetHomeFolder()
        {
            var home = Environment.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.User);
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
            if (!string.IsNullOrEmpty(home)) return home;
            if (!string.IsNullOrEmpty(local)) return local;
            return null;
        }
    }
}
