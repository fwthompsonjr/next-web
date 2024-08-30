using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using next.processor.api.backing;
using next.processor.api.interfaces;
using next.processor.api.services;
using next.processor.api.utility;

namespace next.processor.api.tests
{
    public class ProgramExtensionsTest
    {
        [Theory]
        [InlineData(typeof(IApiWrapper))]
        [InlineData(typeof(IExcelGenerator))]
        [InlineData(typeof(IWebInteractiveWrapper))]
        [InlineData(typeof(IServiceProvider))]
        [InlineData(typeof(IQueueExecutor))]
        [InlineData(typeof(IConfiguration))]
        [InlineData(typeof(SearchGenerationService))]
        [InlineData(typeof(InitializationService))]
        [InlineData(typeof(CheckContainerServices))]
        [InlineData(typeof(CheckPostApiRequest))]
        public void CollectionCanGetInstance(Type type)
        {
            var error = Record.Exception(() =>
            {
                var provider = GetServiceProvider();
                var actual = provider.GetService(type);
                if (actual is IQueueExecutor executor) { ExecutorCanExecute(executor); }
                Assert.NotNull(actual);

            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceInstallationShouldBeTrue()
        {
            // expected value for service installation = false
            // installation should start after service hosting has been confirmed
            const bool expected = false;
            var provider = GetServiceProvider();
            var service = provider.GetService<IConfiguration>();
            Assert.NotNull(service);
            var isInstallationEnabled = service.GetValue<bool>(Constants.KeyServiceInstallation);
            Assert.Equal(expected, isInstallationEnabled);
        }

        [Fact]
        public void QueueProcessingShouldBeSet()
        {
            // expected value for queue processing = false
            // the queue process should only be established 
            // once the installation behaviors are confirmed
            const bool expected = false;
            var provider = GetServiceProvider();
            var service = provider.GetService<IConfiguration>();
            Assert.NotNull(service);
            var isQueueEnabled = service.GetValue<bool>(Constants.KeyQueueProcessEnabled);
            Assert.Equal(expected, isQueueEnabled);
        }
        [Fact]
        public void QueueProcessingShouldInitializeAsNotReady()
        {
            // expected value for queue readiness = false
            const bool expected = false;
            var provider = GetServiceProvider();
            var service = provider.GetService<IQueueExecutor>();
            Assert.NotNull(service);
            var isQueueEnabled = service.IsReady();
            Assert.Equal(expected, isQueueEnabled);
        }

        [Fact]
        public void QueueProcessingShouldInitializeWithNoneReady()
        {
            // expected value for queue readiness = 0
            const int expected = 0;
            var provider = GetServiceProvider();
            var service = provider.GetService<IQueueExecutor>();
            Assert.NotNull(service);
            var actual = service.IsReadyCount();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QueueProcessingShouldInitializeExpectedInstallers()
        {
            // expected value for queue initializers = 7
            const int expected = 7;
            var provider = GetServiceProvider();
            var service = provider.GetService<IQueueExecutor>();
            Assert.NotNull(service);
            var actual = service.InstallerCount();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("begin")]
        [InlineData("parameter")]
        [InlineData("search")]
        public void CollectionCanGetKeyedInstance(string name)
        {
            var error = Record.Exception(() =>
            {
                var provider = GetServiceProvider();
                var actual = provider.GetKeyedService<IQueueProcess>(name);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData("firefox")]
        [InlineData("geckodriver")]
        [InlineData("verification")]
        [InlineData("read-collin")]
        [InlineData("read-denton")]
        [InlineData("read-harris")]
        [InlineData("read-tarrant")]
        public void CollectionCanGetKeyedInstaller(string name)
        {
            var error = Record.Exception(() =>
            {
                var provider = GetServiceProvider();
                var actual = provider.GetKeyedService<IWebContainerInstall>(name);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }

        [Fact]
        public void CollectionCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var provider = new ServiceCollection();
                provider.Configure();
            });
            Assert.Null(error);
        }
        [Fact]
        public void AppCanBeBuilt()
        {
            var error = Record.Exception(() =>
            {
                var builder = WebApplication.CreateBuilder();
                var services = builder.Services;
                services.Configure();
                var app = builder.Build();
                app.ConfigureApp();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AppCanSetSwaggerOptions(bool isDevelopment)
        {
            var error = Record.Exception(() =>
            {
                var builder = WebApplication.CreateBuilder();
                var services = builder.Services;
                services.Configure();
                var app = builder.Build();
                app.SetSwaggerOptions(isDevelopment);
            });
            Assert.Null(error);
        }

        private static void ExecutorCanExecute(IQueueExecutor executor)
        {
            List<string> names = ["begin", "parameter", "search"];
            names.ForEach(n =>
            {
                var instance = executor.GetInstance(n);
                Assert.NotNull(instance);
            });
            var missing = executor.GetInstance("missing");
            Assert.Null(missing);
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