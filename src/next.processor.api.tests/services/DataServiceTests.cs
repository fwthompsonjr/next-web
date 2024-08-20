using next.processor.api.services;

namespace next.processor.api.tests.services
{
    public class DataServiceTests
    {
        [Fact]
        public void ServiceCanReportHealth()
        {
            var error = Record.Exception(() =>
            {
                DataService.ReportHealth("health state");
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanReportState()
        {
            var error = Record.Exception(() =>
            {
                DataService.ReportState("status state");
            });
            Assert.Null(error);
        }
    }
}
