using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class ConfigurationProviderTests
    {
        [Fact]
        public void ConfigCanBeCaptured()
        {
            var error = Record.Exception(() =>
            {
                _ = SettingsProvider.Configuration;
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("api.permissions:destination")]
        [InlineData("api.permissions:remote")]
        [InlineData("api.permissions:local")]
        [InlineData("post_address:initialize")]
        [InlineData("post_address:update")]
        [InlineData("post_address:fetch")]
        [InlineData("post_address:start")]
        [InlineData("post_address:status")]
        [InlineData("post_address:finalize")]
        public void ConfigContainsKey(string keyname)
        {
            var error = Record.Exception(() =>
            {
                var config = SettingsProvider.Configuration;
                var keyvalue = config[keyname];
                Assert.False(string.IsNullOrWhiteSpace(keyvalue));
            });
            Assert.Null(error);
        }
    }
}
