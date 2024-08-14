using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerCacheTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var sut = new ContentSanitizerCache();
            var tmp = sut.Sanitize("");
            Assert.False(string.IsNullOrEmpty(tmp));
        }
    }
}
