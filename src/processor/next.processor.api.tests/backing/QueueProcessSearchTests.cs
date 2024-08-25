using Bogus;
using legallead.records.search.Classes;
using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.services;

namespace next.processor.api.tests.backing
{
    public class QueueProcessSearchTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var settings = new MySettings();
                var service = settings.Sut;
                Assert.Equal(3, service.Index);
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
                var fetched = MockObjProvider.GetWebFetchResult(10);
                if (dataId != 0)
                {
                    data.IterateNext();
                    data.WebReader = settings.WebReader;
                    settings.WebWrapper.Setup(m => m.Fetch(It.IsAny<WebInteractive>())).Returns(fetched);
                }
                mock.Setup(m => m.StartAsync(It.IsAny<QueuedRecord>())).Verifiable();
                if (dataId == 2)
                {
                    mock.SetupSequence(m => m.PostStatusAsync(
                        It.IsAny<QueuedRecord>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()))
                    .Returns(Task.CompletedTask)
                    .Throws(exception);
                }
                else
                {
                    mock.Setup(m => m.PostStatusAsync(
                        It.IsAny<QueuedRecord>(),
                        It.IsAny<int>(),
                        It.IsAny<int>())).Verifiable();
                }
                _ = await service.ExecuteAsync(data);
            });
            Assert.Null(error);
        }

        private sealed class MySettings
        {
            public MySettings()
            {
                var reader = new ExcelGenerator();
                Sut = new(Wrapper.Object, reader, WebWrapper.Object);
                Payload = MockObjProvider.GetQueueResponse(10);
            }
            public Mock<IApiWrapper> Wrapper { get; } = new();
            public Mock<IWebInteractiveWrapper> WebWrapper { get; } = new();
            public QueueProcessSearch Sut { get; }
            public QueueProcessResponses Payload { get; }
            public WebInteractive WebReader { get; } = new();
        }
    }
}