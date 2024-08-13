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

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        private static MockUserSession GetSession()
        {

            var session = new MockUserSession()
                .With((UserContextBo)null)
                .With((UserIdentityBo)null)
                .With((MyPurchaseBo)null)
                .With((MySearchRestrictions)null)
                .With((UserSearchQueryBo)null)
                .With((MailItem)null);
            return session;
        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }
}
