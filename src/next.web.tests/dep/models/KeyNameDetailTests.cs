using legallead.desktop.entities;
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
        [InlineData("Not Mapped")]
        public void ModelCanBeCreated(string keyName)
        {
            var error = Record.Exception(() =>
            {
                var mock = GetSession();
                var session = mock.MqSession.Object;
                var sut = new KeyNameDetail(keyName, session);
                Assert.NotNull(sut);
                if (keyName.Equals("Not Mapped")) return;
                Assert.True(sut.KeyIndex > 0);
                Assert.True(sut.ItemCount > 0);
                Assert.True(sut.ExpirationDt > DateTime.UtcNow);
                Assert.True(Convert.ToDecimal(sut.ExpirationMinutes) > 0);
                Assert.False(string.IsNullOrEmpty(sut.ExpirationDate));
            });
            Assert.Null(error);
        }

        private static MockUserSession GetSession()
        {
            var session = MockUserSession.GetInstance();
            return session;
        }
    }
}
