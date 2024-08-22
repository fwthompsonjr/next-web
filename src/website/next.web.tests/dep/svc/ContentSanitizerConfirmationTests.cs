using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerConfirmationTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var content = MockObjectProvider.GetContent("blank");
            var sut = new ContentSanitizerConfirmation();
            var tmp = sut.Sanitize(content);
            Assert.False(string.IsNullOrEmpty(tmp));
        }
    }
}