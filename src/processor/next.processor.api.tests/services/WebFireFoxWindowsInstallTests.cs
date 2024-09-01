using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.interfaces;
using next.processor.api.services;
using System.Diagnostics;

namespace next.processor.api.tests.services
{
    public class WebFireFoxWindowsInstallTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var mock = new Mock<IWebInstallOperation>();
            var cfgmock = new Mock<IConfiguration>();
            var service = new WebFireFoxWindowsInstall(mock.Object, cfgmock.Object);
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanBeInstalled()
        {
            var error = Record.Exception(() =>
            {
                var provider = GetServiceProvider();
                var service = provider.GetKeyedService<IWebContainerInstall>("windows-firefox");
                Assert.Null(service);
            });
            Assert.Null(error);
        }

        private static ServiceProvider GetServiceProvider()
        {
            lock (locker)
            {
                var provider = new ServiceCollection();
                provider.Configure();
                return provider.BuildServiceProvider();
            }
        }
        private static readonly object locker = new();
    }
}
