using Bogus;
using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;
using next.processor.api.models;

namespace next.processor.api.tests.backing
{
    public class QueueProcessParameterTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var settings = new MySettings();
                var service = settings.Sut;
                Assert.Equal(0, service.Index);
                Assert.NotEmpty(service.Name);
                Assert.False(service.IsSuccess);
                Assert.False(service.AllowIterateNext);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ServiceCanExecuteAsync(int dataId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var exception = new Faker().System.Exception();
                var settings = new MySettings();
                var service = settings.Sut;
                var mock = settings.Wrapper;
                var data = settings.Payload;
                if (dataId != 0) data.IterateNext();
                mock.Setup(m => m.StartAsync(It.IsAny<QueuedRecord>())).Verifiable();
                if (dataId != 2)
                {
                    mock.Setup(m => m.PostStatusAsync(
                        It.IsAny<QueuedRecord>(),
                        It.IsAny<int>(),
                        It.IsAny<int>())).Verifiable();
                }
                else
                {
                    mock.SetupSequence(m => m.PostStatusAsync(
                        It.IsAny<QueuedRecord>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()))
                    .Returns(Task.CompletedTask)
                    .Throws(exception);
                }
                _ = await service.ExecuteAsync(data);
            });
            Assert.Null(error);
        }
        private sealed class MySettings
        {
            public MySettings()
            {
                Sut = new(Wrapper.Object);
                Payload = MockObjProvider.GetQueueResponse(10);
            }
            public Mock<IApiWrapper> Wrapper { get; } = new();
            public QueueProcessParameter Sut { get; }
            public QueueProcessResponses Payload { get; }
        }
    }
}