using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class AccessTokenBoTests
    {
        private readonly Faker<AccessTokenBo> faker =
            new Faker<AccessTokenBo>()
            .RuleFor(x => x.AccessToken, y => y.Random.AlphaNumeric(40))
            .RuleFor(x => x.RefreshToken, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Expires, y => y.Date.Recent());

        [Fact]
        public void AccessTokenBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new AccessTokenBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void AccessTokenBoCanUpdateAccessToken()
        {
            var items = faker.Generate(2);
            items[0].AccessToken = items[1].AccessToken;
            Assert.Equal(items[1].AccessToken, items[0].AccessToken);
        }

        [Fact]
        public void AccessTokenBoCanUpdateRefreshToken()
        {
            var items = faker.Generate(2);
            items[0].RefreshToken = items[1].RefreshToken;
            Assert.Equal(items[1].RefreshToken, items[0].RefreshToken);
        }

        [Fact]
        public void AccessTokenBoCanUpdateExpires()
        {
            var items = faker.Generate(2);
            items[0].Expires = items[1].Expires;
            Assert.Equal(items[1].Expires, items[0].Expires);
        }
    }
}