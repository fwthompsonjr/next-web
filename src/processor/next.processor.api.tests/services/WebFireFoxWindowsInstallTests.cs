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
        public async Task ServiceCanBeInstalledAsync()
        {
            if (!Debugger.IsAttached) return;
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetServiceProvider();
                var service = provider.GetKeyedService<IWebContainerInstall>("windows-firefox");
                Assert.NotNull(service);
                _ = await service.InstallAsync();
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
