using Bogus;
using Microsoft.AspNetCore.Http;
using Moq;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web.tests
{
    internal class MockUserSession
    {

        public MockUserSession With(UserContextBo? bo)
        {
            bo ??= fakeUser.Generate();
            var keyname = SessionKeyNames.UserBo;
            var bytes = Encoding.UTF8.GetBytes(bo.ToJsonString());
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }
        public Mock<ISession> MqSession { get; set; } = new();

        private static readonly Faker<UserContextBo> fakeUser =
            new Faker<UserContextBo>()
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserName, y => y.Person.Email)
            .RuleFor(x => x.AppId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.AppName, y => y.Lorem.Sentence(4))
            .RuleFor(x => x.Token, y => y.Random.AlphaNumeric(36))
            .RuleFor(x => x.RefreshToken, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.Expires, y => y.Date.Future(1));

    }
}
