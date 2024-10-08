using next.processor.api.utility;

namespace next.processor.api.tests
{
    public class SettingsProviderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("default")]
        [InlineData("record.processor")]
        public void SettingsProviderCanGetSetting(string name)
        {
            var actual = TheSettingsProvider.GetSettingOrDefault(name);
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("default")]
        [InlineData("record.processor", true, 90, 1)]
        [InlineData("initialization.service", true, 30, 5)]
        public void SettingsProviderSettingHasExpectedValue(
            string name,
            bool enabled = true,
            int delay = 30,
            int interval = 5)
        {
            var actual = TheSettingsProvider.GetSettingOrDefault(name).Setting;
            Assert.Equal(enabled, actual.Enabled);
            Assert.Equal(delay, actual.Delay);
            Assert.Equal(interval, actual.Interval);
        }
    }
}