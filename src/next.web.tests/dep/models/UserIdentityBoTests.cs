using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class UserIdentityBoTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<UserIdentityBo>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].UserName, sut[1].UserName);
                Assert.NotEqual(sut[0].Email, sut[1].Email);
                Assert.NotEqual(sut[0].Created, sut[1].Created);
                Assert.NotEqual(sut[0].Role, sut[1].Role);
                Assert.NotEqual(sut[0].RoleDescription, sut[1].RoleDescription);
                Assert.NotEqual(sut[0].FullName, sut[1].FullName);
                _ = sut[0].GetCaption();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ModelCanGetCaption(int testid)
        {
            const string dash = " - ";
            var error = Record.Exception(() =>
            {
                var sut = testid == 0 ?
                    new() :
                    MockObjectProvider.GetSingle<UserIdentityBo>();
                if (testid == 1) sut.Email = string.Empty;
                if (testid == 2) sut.UserName = string.Empty;
                var caption = sut.GetCaption();
                Assert.Equal(dash, caption);
            });
            Assert.Null(error);
        }
    }
}