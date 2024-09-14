using next.core.implementations;

namespace next.core.tests.implementations
{
    public class HtmlBeautifyTests
    {
        [Fact]
        public void TextCanBeLoaded()
        {
            var text = Properties.Resources.myaccount_test;
            var parser = new ContentParser();
            var result = parser.BeautfyHTML(text);
            Assert.NotNull(result);
        }
    }
}