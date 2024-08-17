namespace next.processor.api.services
{
    internal static class HtmlProvider
    {
        public static string HomePage => homePage ??= GetHomeContent();
        private static string? homePage;
        private static string GetHomeContent()
        {
            return Properties.Resources.home_layout;
        }
    }
}
