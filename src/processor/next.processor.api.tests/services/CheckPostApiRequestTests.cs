using Moq;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.services;

namespace next.processor.api.tests.services
{
    public class CheckPostApiRequestTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ServiceCanGetHealthAsync(int responseId)
        {
            var response = responseId switch
            {
                0 => null,
                _ => new List<QueuedRecord>()
            };
            var error = await Record.ExceptionAsync(async () =>
            {
                var mock = new Mock<IApiWrapper>();
                mock.Setup(m => m.FetchAsync()).ReturnsAsync(response);
                var service = new CheckPostApiRequest(mock.Object);
                _ = await service.CheckHealthAsync(new());
                mock.Verify(m => m.FetchAsync());
            });
            Assert.Null(error);
        }

    }
}