using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;
using next.processor.api.models;

namespace next.processor.api.tests.backing
{
    public class QueueExecutorTests
    {
        [Theory]
        [InlineData("begin")]
        [InlineData("parameter")]
        [InlineData("search")]
        public void ProviderCanGetInstances(string name)
        {
            var provider = GetServiceProvider(5);
            var service = provider.GetRequiredService<IQueueExecutor>();
            var instance = service.GetInstance(name);
            Assert.NotNull(instance);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task ProviderCanExecuteAsync(int count)
        {
            var provider = GetServiceProvider(count);
            if (count == 10)
            {
                var exception = new Faker().System.Exception();
                var mock = provider.GetService<Mock<IQueueProcess>>();
                mock?.Setup(m => m.ExecuteAsync(It.IsAny<QueueProcessResponses>())).ThrowsAsync(exception);
            }
            var error = await Record.ExceptionAsync(async () =>
            {
                var service = provider.GetRequiredService<IQueueExecutor>();
                await service.ExecuteAsync();
            });
            Assert.Null(error);
        }
        private static ServiceProvider GetServiceProvider(int count)
        {
            lock (locker)
            {
                var wrapper = new MockApiWrapperService();
                var mockmapper = new Mock<IQueueProcess>();
                var mockfetch = new Mock<IQueueProcess>();
                var begin = new ProcessBeginProvider(count).Sut;
                var provider = new ServiceCollection();
                mockmapper.SetupGet(x => x.IsSuccess).Returns(true);
                mockfetch.SetupGet(x => x.IsSuccess).Returns(true);

                mockmapper.Setup(x => x.ExecuteAsync(It.IsAny<QueueProcessResponses>()));
                mockfetch.Setup(x => x.ExecuteAsync(It.IsAny<QueueProcessResponses>()));

                provider.AddSingleton(mockmapper);
                provider.AddSingleton<IApiWrapper>(wrapper);
                provider.AddKeyedSingleton<IQueueProcess>("begin", begin);
                provider.AddKeyedSingleton("parameter", mockmapper.Object);
                provider.AddKeyedSingleton("search", mockfetch.Object);
                provider.AddSingleton(s => s);
                provider.AddSingleton<IQueueExecutor, QueueExecutor>();

                return provider.BuildServiceProvider();
            }
        }
        private static readonly object locker = new();

        private sealed class ProcessBeginProvider
        {
            public ProcessBeginProvider(int count)
            {
                var mock = MockApiWrapper;
                Sut = new(mock.Object);
                var data = count switch
                {
                    -1 => null,
                    _ => MockObjProvider.RecordFaker.Generate(count)
                };
                mock.Setup(m => m.FetchAsync()).ReturnsAsync(data);
            }
            public Mock<IApiWrapper> MockApiWrapper { get; } = new();
            public QueueProcessBegin Sut { get; }
        }
    }
}
