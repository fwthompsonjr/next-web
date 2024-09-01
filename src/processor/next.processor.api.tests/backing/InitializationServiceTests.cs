using Microsoft.Extensions.Configuration;
using next.processor.api.backing;
using next.processor.api.utility;

namespace next.processor.api.tests.services
{
    public class InitializationServiceTests
    {

        [Fact]
        public void ServiceCanBeConstruced()
        {
            var error = Record.Exception(() =>
            {
                var config = GetConfiguration();
                var service = new InitializationService(config);
                Assert.NotNull(service);
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanDoWork()
        {
            var error = Record.Exception(() =>
            {
                var config = GetConfiguration();
                var service = new MockInitializationService(config);
                for (var i = 0; i < 3; i++)
                {
                    service.Work();
                }
            });
            Assert.Null(error);
        }


        [Fact]
        public void ServiceCanGetHealth()
        {
            var error = Record.Exception(() =>
            {
                var config = GetConfiguration();
                var service = new MockInitializationService(config);
                var health = service.Health();
                Assert.NotEmpty(health);
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanGetStatus()
        {
            var error = Record.Exception(() =>
            {
                var config = GetConfiguration();
                var service = new MockInitializationService(config);
                var status = service.Status();
                Assert.NotEmpty(status);
            });
            Assert.Null(error);
        }

        private sealed class MockInitializationService(IConfiguration configuration) : InitializationService(configuration)
        {
            public void Work()
            {
                DoWork(null);
            }
            public string Health()
            {
                return GetHealth();
            }
            public string Status()
            {
                return GetStatus();
            }
        }
        private static IConfiguration GetConfiguration()
        {
            return SettingsProvider.GetConfiguration();
        }
    }
}