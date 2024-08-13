using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class MailItemBodyTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<MailItemBody>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].Id, sut[1].Id);
                Assert.NotEqual(sut[0].Body, sut[1].Body);
            });
            Assert.Null(error);
        }
    }
}