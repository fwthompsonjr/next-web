using next.web.Models;

namespace next.web.tests.dep.models
{
    public class FetchIntentResponseTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<FetchIntentResponse>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].ClientSecret, sut[1].ClientSecret);
            });
            Assert.Null(error);
        }
    }
}