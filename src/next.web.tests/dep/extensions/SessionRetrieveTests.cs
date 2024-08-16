
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.util;

namespace next.web.tests.dep.extensions
{
    public class SessionRetrieveTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SessionCanGetMail(bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.Session.RetrieveMail(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SessionCanGetRestriction(bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.Session.RetrieveRestriction(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SessionCanGetHistory(bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.Session.RetrieveHistory(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SessionCanGetPurchases(bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.Session.RetrievePurchases(sut.PermissionApi);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(SearchFilterNames.Purchases)]
        [InlineData(SearchFilterNames.Active)]
        [InlineData(SearchFilterNames.History)]
        [InlineData(SearchFilterNames.Purchases, false)]
        [InlineData(SearchFilterNames.Active, false)]
        [InlineData(SearchFilterNames.History, false)]
        public async Task SessionCanGetFilter(SearchFilterNames filterName, bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = sut.Session.RetrieveFilter(filterName);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(SearchFilterNames.Purchases)]
        [InlineData(SearchFilterNames.Active)]
        [InlineData(SearchFilterNames.History)]
        [InlineData(SearchFilterNames.Purchases, false)]
        [InlineData(SearchFilterNames.Active, false)]
        [InlineData(SearchFilterNames.History, false)]
        public async Task SessionCanUpdateFilter(SearchFilterNames filterName, bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                var filter = MockObjectProvider.GetSingle<UserSearchFilterBo>();
                sut.Session.UpdateFilter(filter, filterName);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SessionCanGetIdentity(bool authorized)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.Session.RetrieveIdentity(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("0", true)]
        [InlineData("0", false)]
        [InlineData("", true)]
        [InlineData("", false)]
        [InlineData("00000000-0000-0000-0000-000000000000", true)]
        [InlineData("00000000-0000-0000-0000-000000000000", false)]
        public async Task SessionCanGetMailBody(string messageId, bool authorized)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.User.ToUserBo().GetMailBody(sut.PermissionApi, messageId);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SessionCanGetUserId(bool authorized)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize(authorized);
                _ = await sut.User.ToUserBo().GetUserId(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        private static async Task<Harness> Initialize(bool authorized = true)
        {
            var mock = MockUserSession.GetInstance();
            var session = mock.MqSession.Object;
            var id = authorized ? 200 : 401;
            var api = new MockAccountApi(id);
            var context = session.GetContextUser() ?? new();
            if (authorized) await context.Save(session, api);
            return new Harness(session, api, context);
        }

        private sealed class Harness(ISession session, IPermissionApi api, UserContextBo user)
        {
            public ISession Session { get; set; } = session;
            public IPermissionApi PermissionApi { get; set; } = api;
            public UserContextBo User { get; set; } = user;
        }
    }
}
