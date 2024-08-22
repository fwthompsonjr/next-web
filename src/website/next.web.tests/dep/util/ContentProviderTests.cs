using next.web.core.util;

namespace next.web.tests.dep.util
{
    public class ContentProviderTests
    {
        [Fact]
        public void ServiceCanGetProvider()
        {
            var error = Record.Exception(() =>
            {
                var provider = ContentProvider.LocalContentProvider;
                Assert.NotNull(provider);
            });
            Assert.Null(error);
        }
    }
}
