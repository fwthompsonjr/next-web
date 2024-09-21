namespace next.processor.api.tests.backing
{
    public class NonPersonQueueServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            using var service = new MockNonPersonQueueService();
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanDoWork()
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockNonPersonQueueService();
                service.Work();
            });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanReportHealth()
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockNonPersonQueueService();
                var actual = service.EchoMyHealth();
                Assert.True(string.IsNullOrEmpty(actual));
            });
            Assert.Null(error);
        }


        [Fact]
        public void ServiceCanReportStatus()
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockNonPersonQueueService();
                var actual = service.EchoMyStatus();
                Assert.True(string.IsNullOrEmpty(actual));
            });
            Assert.Null(error);
        }
    }
}