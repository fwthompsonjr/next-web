using Bogus;
using legallead.desktop.entities;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web.tests
{

#pragma warning disable CS8600 // null values used to manage With overloads in builder pattern

    internal class MockUserSession
    {
        public static MockUserSession GetInstance()
        {
            var session = new MockUserSession()
                .With((UserContextBo)null)
                .With((UserIdentityBo)null)
                .With((MyPurchaseBo)null)
                .With((MySearchRestrictions)null)
                .With((UserSearchQueryBo)null)
                .With((MailItem)null)
                .With((PermissionChangedResponse)null);
            return session;
        }
        public MockUserSession With(UserContextBo? bo)
        {
            bo ??= fakeUser.Generate();
            var keyname = SessionKeyNames.UserBo;
            var bytes = Encoding.UTF8.GetBytes(bo.ToJsonString());
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }
        public MockUserSession With(UserIdentityBo? bo)
        {
            bo ??= MockObjectProvider.GetSingle<UserIdentityBo>();
            var keyname = SessionKeyNames.UserIdentity;
            var timed = new UserTimedCollection<UserIdentityBo>(bo, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            var bytes = Encoding.UTF8.GetBytes(json);
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }

        public MockUserSession With(MailItem? item)
        {
            var count = new Faker().Random.Int(100, 500);
            var list = MockObjectProvider.GetList<MailItem>(count) ?? [];
            if (item != null)
            {
                list.ForEach(i => i.UserId = item.UserId);
            }
            var keyname = SessionKeyNames.UserMailbox;
            var timed = new UserTimedCollection<List<MailItem>>(list, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            var bytes = Encoding.UTF8.GetBytes(json);
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }
        public MockUserSession With(MyPurchaseBo? item)
        {
            var count = new Faker().Random.Int(10, 100);
            var list = MockObjectProvider.GetList<MyPurchaseBo>(count) ?? [];
            var keyname = SessionKeyNames.UserSearchPurchases;
            var timed = new UserTimedCollection<List<MyPurchaseBo>>(list, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            var bytes = Encoding.UTF8.GetBytes(json);
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }
        public MockUserSession With(MySearchRestrictions? bo)
        {
            bo ??= MockObjectProvider.GetSingle<MySearchRestrictions>();
            var keyname = SessionKeyNames.UserRestriction;
            var timed = new UserTimedCollection<MySearchRestrictions>(bo, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            var bytes = Encoding.UTF8.GetBytes(json);
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }

        public MockUserSession With(UserSearchQueryBo? item)
        {
            var count = new Faker().Random.Int(100, 500);
            var list = MockObjectProvider.GetList<UserSearchQueryBo>(count) ?? [];
            if (item != null)
            {
                list.ForEach(i => i.UserId = item.UserId);
            }
            var keyname = SessionKeyNames.UserSearchHistory;
            var timed = new UserTimedCollection<List<UserSearchQueryBo>>(list, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            var bytes = Encoding.UTF8.GetBytes(json);
            MqSession.Setup(s => s.TryGetValue(It.Is<string>(s => s.Equals(keyname)), out bytes)).Returns(true);
            return this;
        }

        public MockUserSession With(PermissionChangedResponse? bo)
        {
            bo ??= MockObjectProvider.GetSingle<PermissionChangedResponse>();
            var keyname = SessionKeyNames.UserPermissionChanged;
            var timed = new UserTimedCollection<PermissionChangedResponse>(bo, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            var bytes = Encoding.UTF8.GetBytes(json);
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

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
}
