using next.web.core.models;
using next.web.core.util;

namespace next.web.tests.dep.models
{
    public class KeyNameDetailTests
    {
        [Theory]
        [InlineData(SessionKeyNames.UserMailbox)]
        [InlineData(SessionKeyNames.UserSearchActive)]
        [InlineData(SessionKeyNames.UserSearchPurchases)]
        [InlineData(SessionKeyNames.UserSearchHistory)]
        [InlineData(SessionKeyNames.UserIdentity)]
        [InlineData(SessionKeyNames.UserMailbox, false)]
        [InlineData(SessionKeyNames.UserSearchActive, false)]
        [InlineData(SessionKeyNames.UserSearchPurchases, false)]
        [InlineData(SessionKeyNames.UserSearchHistory, false)]
        [InlineData(SessionKeyNames.UserIdentity, false)]
        [InlineData("Not Mapped")]
        public void ModelCanBeCreated(string keyName, bool authorized = true)
        {
            var error = Record.Exception(() =>
            {
                var mock = GetSession(authorized);
                var session = mock.MqSession.Object;
                var sut = new KeyNameDetail(keyName, session);
                Assert.NotNull(sut);
                if (keyName.Equals("Not Mapped") || !authorized) return;
                Assert.True(sut.KeyIndex > 0);
                Assert.True(sut.ItemCount > 0);
                Assert.True(sut.ExpirationDt > DateTime.UtcNow);
                Assert.True(Convert.ToDecimal(sut.ExpirationMinutes) > 0);
                Assert.False(string.IsNullOrEmpty(sut.ExpirationDate));
            });
            Assert.Null(error);
        }

        private static MockUserSession GetSession(bool authorized = true)
        {
            var session = MockUserSession.GetInstance(authorized);
            return session;
        }
    }
}
