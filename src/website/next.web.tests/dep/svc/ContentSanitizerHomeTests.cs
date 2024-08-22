using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerHomeTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var content = MockObjectProvider.GetContent("blank");
            var sut = new ContentSanitizerHome();
            var tmp = sut.Sanitize(content);
            Assert.False(string.IsNullOrEmpty(tmp));
        }
    }
}