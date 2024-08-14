
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using legallead.desktop.entities;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.util;

namespace next.web.tests.dep.extensions
{
    public class SessionRetrieveTests
    {
        [Fact]
        public async Task SessionCanGetMail()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.Session.RetrieveMail(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Fact]
        public async Task SessionCanGetRestriction()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.Session.RetrieveRestriction(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Fact]
        public async Task SessionCanGetHistory()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.Session.RetrieveHistory(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Fact]
        public async Task SessionCanGetPurchases()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.Session.RetrievePurchases(sut.PermissionApi);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(SearchFilterNames.Purchases)]
        [InlineData(SearchFilterNames.Active)]
        [InlineData(SearchFilterNames.History)]
        public async Task SessionCanGetFilter(SearchFilterNames filterName)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = sut.Session.RetrieveFilter(filterName);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(SearchFilterNames.Purchases)]
        [InlineData(SearchFilterNames.Active)]
        [InlineData(SearchFilterNames.History)]
        public async Task SessionCanUpdateFilter(SearchFilterNames filterName)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                var filter = MockObjectProvider.GetSingle<UserSearchFilterBo>();
                sut.Session.UpdateFilter(filter, filterName);
            });
            Assert.Null(error);
        }

        [Fact]
        public async Task SessionCanGetIdentity()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.Session.RetrieveIdentity(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task SessionCanGetMailBody(string messageId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.User.ToUserBo().GetMailBody(sut.PermissionApi, messageId);
            });
            Assert.Null(error);
        }

        [Fact]
        public async Task SessionCanGetUserId()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = await Initialize();
                _ = await sut.User.ToUserBo().GetUserId(sut.PermissionApi);
            });
            Assert.Null(error);
        }

        private static async Task<Harness> Initialize()
        {
            var mock = MockUserSession.GetInstance();
            var session = mock.MqSession.Object;
            var api = new MockAccountApi(200);
            var context = session.GetContextUser();
            Assert.NotNull(context);
            await context.Save(session, api);
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
