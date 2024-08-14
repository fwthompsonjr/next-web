using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerDownloadTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var content = MockObjectProvider.GetContent("blank");
            var sut = new ContentSanitizerDownload();
            var tmp = sut.Sanitize(content);
            Assert.False(string.IsNullOrEmpty(tmp));
        }
        [Fact]
        public void ServiceCanAppendContent()
        {
            var keys = new List<KeyValuePair<string, string>>
            {
                new("//*[@id='spn-download-external-id']", " - "),
                new("//*[@id='spn-download-description']", " - "),
                new("//*[@id='spn-download-date']", " - "),
            };
            var content = MockObjectProvider.GetContent("blank");
            var sut = new ContentSanitizerDownload();
            var tmp = sut.Sanitize(content);
            tmp = ContentSanitizerDownload.AppendContext(tmp, keys);
            Assert.False(string.IsNullOrEmpty(tmp));
        }
    }
}