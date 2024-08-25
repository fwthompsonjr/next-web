using Moq;
using next.processor.api.interfaces;
using next.processor.api.services;

namespace next.processor.api.tests.services
{
    public class CheckContainerServicesTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanGetHealthAsync(bool? response)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var executor = new Mock<IQueueExecutor>();
                executor.Setup(m => m.IsReady()).Returns(response);
                var service = new CheckContainerServices(executor.Object);
                _ = await service.CheckHealthAsync(new());
            });
            Assert.Null(error);
        }

    }
}
