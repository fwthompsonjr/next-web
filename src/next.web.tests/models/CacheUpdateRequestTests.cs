using next.web.Models;

namespace next.web.tests.dep.models
{
    public class CacheUpdateRequestTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<CacheUpdateRequest>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].Name, sut[1].Name);
            });
            Assert.Null(error);
        }
    }
}