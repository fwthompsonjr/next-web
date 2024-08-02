namespace next.web.tests.dep
{
    public class ResourcesTests
    {
        [Fact]
        public void ResourceCanBeConstructed()
        {
            var error = Record.Exception(() =>
            {
                _ = new core.Properties.Resources();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ResourceHasCultureInfo()
        {
            var error = Record.Exception(() =>
            {
                var info = core.Properties.Resources.Culture;
                core.Properties.Resources.Culture = info;
            });
            Assert.Null(error);
        }

        [Fact]
        public void ResourceHasExpectedStrings()
        {
            var error = Record.Exception(() =>
            {
                _ = core.Properties.Resources.base_menu;
                _ = core.Properties.Resources.core_configuration;
                _ = core.Properties.Resources.logout_page;
            });
            Assert.Null(error);
        }
    }
}
