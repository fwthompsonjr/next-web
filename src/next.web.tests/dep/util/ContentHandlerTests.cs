using next.web.core.util;

namespace next.web.tests.dep.util
{
    public class ContentHandlerTests
    {
        [Theory]
        [InlineData("home")]
        [InlineData("blank")]
        [InlineData("introduction")]
        [InlineData("myaccount")]
        [InlineData("mysearch")]
        [InlineData("mailbox")]
        [InlineData("viewhistory")]
        [InlineData("not-mapped", true)]
        public void HandlerCanGetContent(string name, bool expected = false)
        {
            var error = Record.Exception(() =>
            {
                var page = ContentHandler.GetLocalContent(name);
                var content = page?.Content ?? string.Empty;
                Assert.Equal(expected, string.IsNullOrEmpty(content));
            });
            Assert.Null(error);
        }
    }
}
