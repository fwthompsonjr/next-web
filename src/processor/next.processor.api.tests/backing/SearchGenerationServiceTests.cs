namespace next.processor.api.tests.backing
{
    public class SearchGenerationServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            using var service = new MockSearchGenerationService();
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanDoWork()
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockSearchGenerationService();
                service.Work();
            });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanReportHealth()
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockSearchGenerationService();
                var actual = service.EchoMyHealth();
                Assert.False(string.IsNullOrEmpty(actual));
            });
            Assert.Null(error);
        }


        [Fact]
        public void ServiceCanReportStatus()
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockSearchGenerationService();
                var actual = service.EchoMyStatus();
                Assert.False(string.IsNullOrEmpty(actual));
            });
            Assert.Null(error);
        }
    }
}
