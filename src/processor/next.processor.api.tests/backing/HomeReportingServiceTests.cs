using Bogus;
using legallead.jdbc.entities;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;

namespace next.processor.api.tests.backing
{
    public class HomeReportingServiceTests
    {
        [Fact]
        public void ServiceCanBeConstructed()
        {
            var error = Record.Exception(() =>
            {
                var queue = new Mock<IQueueExecutor>();
                var api = new Mock<IApiWrapper>();
                var service = new HomeReportingServiceAccessor(queue.Object, api.Object);
                Assert.NotNull(service);
            });
            Assert.Null(error);
        }


        [Fact]
        public void ServiceCanDoWork()
        {
            var error = Record.Exception(() =>
            {
                var queue = new Mock<IQueueExecutor>();
                var api = new Mock<IApiWrapper>();
                var service = new HomeReportingServiceAccessor(queue.Object, api.Object);
                service.Work();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanGetStatus()
        {
            var error = Record.Exception(() =>
            {
                var queue = new Mock<IQueueExecutor>();
                var api = new Mock<IApiWrapper>();
                var service = new HomeReportingServiceAccessor(queue.Object, api.Object);
                var status = service.GetStatusAccessor();
                Assert.True(string.IsNullOrEmpty(status));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 6)]
        [InlineData(2, 0)]
        [InlineData(2, 2)]
        [InlineData(2, 1)]
        [InlineData(4, 6)]
        [InlineData(8, 6)]
        [InlineData(8, 8)]
        public void ServiceCanGetHealth(int readyCount, int installCount)
        {
            var error = Record.Exception(() =>
            {
                var queue = new Mock<IQueueExecutor>();
                var api = new Mock<IApiWrapper>();
                var list = statusbofaker.Generate(5);
                var details = new Dictionary<string, object> {
                    { "00", "item 00" },
                    { "01", "item 01" },
                    { "02", "item 02" }
                };
                queue.Setup(x => x.IsReadyCount()).Returns(readyCount);
                queue.Setup(x => x.InstallerCount()).Returns(installCount);
                queue.Setup(x => x.GetDetails()).Returns(details);
                api.Setup(x => x.FetchSummaryAsync()).ReturnsAsync(list);
                var service = new HomeReportingServiceAccessor(queue.Object, api.Object);
                var health = service.GetHealthAccessor();
                Assert.False(string.IsNullOrEmpty(health));
            });
            Assert.Null(error);
        }

        private sealed class HomeReportingServiceAccessor(IQueueExecutor queue, IApiWrapper api) : HomeReportingService(queue, api)
        {
            public void Work()
            {
                DoWork(null);
            }

            public string GetHealthAccessor()
            {
                return GetHealth();
            }
            public string GetStatusAccessor()
            {
                return GetStatus();
            }
        }

        private static readonly Faker<StatusSummaryBo> statusbofaker =
            new Faker<StatusSummaryBo>()
            .RuleFor(x => x.SearchProgress, y => y.Music.Genre())
            .RuleFor(x => x.Total, y => y.Random.Int(0, 50000));
    }
}