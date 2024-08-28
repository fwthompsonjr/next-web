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
                executor.Setup(m => m.GetDetails()).Returns(CommonKeys);
                var service = new CheckContainerServices(executor.Object);
                _ = await service.CheckHealthAsync(new());
            });
            Assert.Null(error);
        }

        private static readonly Dictionary<string, object> CommonKeys = new()
        {
            { "1", "Number one" },
            { "2", "Number two" },
            { "3", "Number three" }
        };

    }
}
