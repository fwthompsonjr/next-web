using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class UserBoTests
    {
        private static readonly Faker<ApiContext> contextfaker =
            new Faker<ApiContext>()
            .RuleFor(x => x.Id, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        private static readonly Faker<AccessTokenBo> tokenfaker =
            new Faker<AccessTokenBo>()
            .RuleFor(x => x.AccessToken, y => y.Random.AlphaNumeric(40))
            .RuleFor(x => x.RefreshToken, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Expires, y => DateTime.UtcNow.AddMinutes(45));

        private readonly Faker<UserBo> userfaker =
            new Faker<UserBo>()
            .RuleFor(x => x.Applications, y =>
            {
                var count = y.Random.Int(1, 6);
                return contextfaker.Generate(count).ToArray();
            })
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Token, y => tokenfaker.Generate());
        private readonly Faker<UserBo> faker =
            new Faker<UserBo>()
            .RuleFor(x => x.Applications, y =>
            {
                var isnull = y.Random.Bool();
                if (isnull) return null;
                var count = y.Random.Int(0, 6);
                return contextfaker.Generate(count).ToArray();
            })
            .RuleFor(x => x.UserName, y => y.Company.CompanyName());

        [Fact]
        public void UserBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserBoCanGetIsAuthenicated()
        {
            var item = faker.Generate();
            var actual = item.IsAuthenicated;
            var expected = item.Token != null;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UserBoCanUpdateUserName()
        {
            var items = faker.Generate(2);
            items[0].UserName = items[1].UserName;
            Assert.Equal(items[1].UserName, items[0].UserName);
        }

        [Fact]
        public void UserBoCanGetIsInitialized()
        {
            var items = faker.Generate(10);
            items.ForEach(i =>
            {
                var expected = i.Applications != null && i.Applications.Length > 0;
                Assert.Equal(expected, i.IsInitialized);
            });
        }

        [Fact]
        public void UserBoCanGetAppServiceHeader()
        {
            var items = faker.Generate(10);
            items.ForEach(i =>
            {
                var expected = i.Applications == null || i.Applications.Length == 0;
                var header = i.GetAppServiceHeader();
                Assert.Equal(expected, string.IsNullOrEmpty(header));
            });
        }

        [Theory]
        [InlineData(true, true, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, true, true)]
        public void UserCanGetSessionTimeout(
            bool hasToken,
            bool hasExpirationDate,
            bool isExpired,
            bool expected)
        {
            var user = userfaker.Generate();
            if (!hasToken) { user.Token = null; }
            if (user.Token != null && !hasExpirationDate) { user.Token.Expires = null; }
            if (user.Token != null && isExpired)
            {
                var token = tokenfaker.Generate();
                token.Expires = DateTime.UtcNow.AddMinutes(2);
                user.Token = token;
            }
            var isTimeout = user.IsSessionTimeout();
            Assert.Equal(expected, isTimeout);
        }


        [Theory]
        [InlineData(true, true, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, true, true)]
        public void UserCanGetSessionExpiration(
            bool hasToken,
            bool hasExpirationDate,
            bool isExpired,
            bool expected)
        {
            var user = userfaker.Generate();
            if (!hasToken) { user.Token = null; }
            if (user.Token != null && !hasExpirationDate) { user.Token.Expires = null; }
            if (user.Token != null && isExpired)
            {
                user.Token.Expires = DateTime.UtcNow.AddSeconds(30);
            }
            var isTimeout = user.IsSessionExpired();
            Assert.Equal(expected, isTimeout);
        }

        [Fact]
        public void UserDefaultCanGetSessionTimeout()
        {
            var user = new UserBo();
            var isTimeout = user.IsSessionTimeout();
            Assert.False(isTimeout);
        }
    }
}