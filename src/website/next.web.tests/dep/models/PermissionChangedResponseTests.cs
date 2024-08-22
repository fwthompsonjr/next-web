using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class PermissionChangedResponseTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<PermissionChangedResponse>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].Name, sut[1].Name);
                Assert.NotEqual(sut[0].Email, sut[1].Email);
                Assert.NotEqual(sut[0].Request, sut[1].Request);
                Assert.NotEqual(sut[0].Dto, sut[1].Dto);
            });
            Assert.Null(error);
        }
    }
}