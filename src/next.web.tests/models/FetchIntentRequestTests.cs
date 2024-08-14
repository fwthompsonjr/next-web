using next.web.Models;

namespace next.web.tests.dep.models
{
    public class FetchIntentRequestTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<FetchIntentRequest>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].Id, sut[1].Id);
            });
            Assert.Null(error);
        }
    }
}