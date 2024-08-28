namespace next.processor.api.services
{
    internal static class HtmlProvider
    {
        public static string HomePage => homePage ??= GetHomeContent();
        public static string StatusPage => statusPage ??= GetStatusContent();

        private static string? homePage;
        private static string? statusPage;
        private static string GetHomeContent()
        {
            return Properties.Resources.home_layout;
        }
        private static string GetStatusContent()
        {
            return Properties.Resources.status_layout;
        }
    }
}
