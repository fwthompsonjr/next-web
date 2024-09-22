namespace next.core.tests
{
    using CoreResources = next.core.Properties.Resources;
    public class CoreResourceTests
    {

        [Fact]
        public void MangerCanGetAndSetCulture()
        {
            var error = Record.Exception(() =>
            {
                _ = CoreResources.Culture;
            });
            Assert.Null(error);
        }

        [Fact]
        public void MangerContainsExpectedKeys()
        {
            var error = Record.Exception(() =>
            {
                var items = Keys.Values.Where(w => string.IsNullOrEmpty(w));
                Assert.Empty(items);
            });
            Assert.Null(error);
        }

        private static readonly Dictionary<string, string> Keys = new()
        {
            { "appsettings", CoreResources.appsettings },
            { "appsettings-debug", CoreResources.appsettings_debug },
            { "base-css", CoreResources.base_css },
            { "blank_html", CoreResources.blank_html },
            { "bootstrapmin_css", CoreResources.bootstrapmin_css },
            { "common_status", CoreResources.common_status },
            { "error_html", CoreResources.error_html },
            { "errorstatus_json", CoreResources.errorstatus_json },
            { "home_html", CoreResources.home_html },
            { "introduction_html", CoreResources.introduction_html },
            { "invoice_html", CoreResources.invoice_html },
            { "myaccount_base_html", CoreResources.myaccount_base_html },
            { "myaccount_permissions_html", CoreResources.myaccount_permissions_html },
            { "myaccount_profile_html", CoreResources.myaccount_profile_html },
            { "mysearchactive_html", CoreResources.mysearchactive_html },
            { "searchhistory_table_html", CoreResources.searchhistory_table_html },
        };
    }
}
