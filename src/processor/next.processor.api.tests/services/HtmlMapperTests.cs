using next.processor.api.services;

namespace next.processor.api.tests.services
{
    public class HtmlMapperTests
    {
        [Theory]
        [InlineData("healthy")]
        [InlineData("degraded")]
        [InlineData("unhealthy")]
        [InlineData("not responding")]
        public void MapperCanTransformContent(string status)
        {
            var home = HtmlProvider.HomePage;
            var error = Record.Exception(() =>
            {
                var transform = HtmlMapper.Home(home, status);
                Assert.False(string.IsNullOrEmpty(transform));
            });
            Assert.Null(error);
        }
    }
}
