using next.processor.api.services;

namespace next.processor.api.tests.services
{
    public class HtmlProviderTests
    {
        [Fact]
        public void ProviderCanGetHomePage()
        {
            var content = HtmlProvider.HomePage;
            Assert.False(string.IsNullOrWhiteSpace(content));
        }
    }
}
