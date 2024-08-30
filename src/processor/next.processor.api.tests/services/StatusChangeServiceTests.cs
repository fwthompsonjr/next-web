
using Microsoft.Extensions.Configuration;
using Moq;
using next.processor.api.services;
using next.processor.api.utility;

namespace next.processor.api.tests.services
{
    public class StatusChangeServiceTests
    {

        [Fact]
        public void ServiceCanBeConstruced()
        {
            var config = GetConfiguration();
            var service = new StatusChangeService(config);
            Assert.True(service.AllowModelChanges);
        }


        [Theory]
        [InlineData("")]
        [InlineData("missing")]
        [InlineData("errors")]
        [InlineData("start")]
        [InlineData("stop")]
        [InlineData("toggle-installation")]
        [InlineData("toggle-queue")]
        public void ServiceCanChange(string key)
        {
            var error = Record.Exception(() =>
            {
                var config = GetConfiguration();
                var service = new StatusChangeService(config) { AllowModelChanges = false };
                service.ChangeStatus(key);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("")]
        [InlineData("missing")]
        [InlineData("errors")]
        [InlineData("start")]
        [InlineData("stop")]
        [InlineData("toggle-installation")]
        [InlineData("toggle-queue")]
        [InlineData("toggle-queue", "not-healthy")]
        [InlineData("toggle-queue", "degraded")]
        [InlineData("toggle-queue", "unhealthy")]
        public void ServiceCanChangeWithHealth(string key, string health = "healthy")
        {
            var error = Record.Exception(() =>
            {
                var config = GetConfiguration();
                var service = new StatusChangeService(config) { AllowModelChanges = false };
                service.ChangeStatus(key, health);
            });
            Assert.Null(error);
        }

        private static IConfiguration GetConfiguration()
        {
            return SettingsProvider.GetConfiguration();
        }
    }
}
