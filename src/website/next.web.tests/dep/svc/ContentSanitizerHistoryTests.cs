using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerHistoryTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var content = MockObjectProvider.GetContent("blank");
            var sut = new ContentSanitizerHistory();
            var tmp = sut.Sanitize(content);
            Assert.False(string.IsNullOrEmpty(tmp));
        }
    }
}