using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;

namespace next.processor.api.tests.backing
{
    public class QueueProcessBeginTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var settings = new MySettings();
                var service = settings.Sut;
                Assert.Equal(-1, service.Index);
                Assert.NotEmpty(service.Name);
                Assert.False(service.IsSuccess);
                Assert.False(service.AllowIterateNext);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        public async Task ServiceCanExecuteAsync(int dataId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var data = dataId switch
                {
                    -1 => null,
                    _ => MockObjProvider.RecordFaker.Generate(dataId)
                };
                var settings = new MySettings();
                var service = settings.Sut;
                var mock = settings.MockApiWrapper;
                mock.Setup(m => m.FetchAsync()).ReturnsAsync(data);
                _ = await service.ExecuteAsync(null);
                mock.Verify(m => m.FetchAsync());
            });
            Assert.Null(error);
        }
        private sealed class MySettings
        {
            public MySettings()
            {
                Sut = new(MockApiWrapper.Object);
            }
            public Mock<IApiWrapper> MockApiWrapper { get; } = new();
            public QueueProcessBegin Sut { get; }
        }
    }
}
