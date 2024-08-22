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
            var actual = SettingsProvider.GetSettingOrDefault(name);
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("default")]
        [InlineData("record.processor", true, 30, 1)]
        public void SettingsProviderSettingHasExpectedValue(
            string name,
            bool enabled = true,
            int delay = 30,
            int interval = 5)
        {
            var actual = SettingsProvider.GetSettingOrDefault(name).Setting;
            Assert.Equal(enabled, actual.Enabled);
            Assert.Equal(delay, actual.Delay);
            Assert.Equal(interval, actual.Interval);
        }
    }
}