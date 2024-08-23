using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using next.processor.api.interfaces;

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