using next.web.Models;

namespace next.web.tests.dep.models
{
    public class DownloadJsResponseTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<DownloadJsResponse>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].ExternalId, sut[1].ExternalId);
                Assert.NotEqual(sut[0].Description, sut[1].Description);
                Assert.NotEqual(sut[0].Content, sut[1].Content);
                Assert.NotEqual(sut[0].Error, sut[1].Error);
                Assert.NotEqual(sut[0].CreateDate, sut[1].CreateDate);
                _ = sut[0].FileName();
            });
            Assert.Null(error);
        }
        [Fact]
        public void ModelCanParseFileName()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetSingle<DownloadJsResponse>();
                sut.Description = "Record Search : TARRANT TX - 2024-06-24 to 2024-06-25 on 2024-06-26 15:32:00";
                _ = sut.FileName();
            });
            Assert.Null(error);
        }
    }
}