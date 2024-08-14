using next.web.core.extensions;
using next.web.core.models;

namespace next.web.tests.dep.extensions
{
    public class UserContextBoTests
    {
        [Fact]
        public void UserCanSave()
        {
            var error = Record.Exception(() =>
            {
                var mock = MockUserSession.GetInstance();
                var session = mock.MqSession.Object;
                var context = session.GetContextUser();
                Assert.NotNull(context);
                context.Save(session);
            });
            Assert.Null(error);
        }
        [Fact]
        public void UserCanSavePermissionChanged()
        {
            var error = Record.Exception(() =>
            {
                var mock = MockUserSession.GetInstance();
                var session = mock.MqSession.Object;
                var change = MockObjectProvider.GetSingle<PermissionChangedResponse>();
                session.Save(change);
            });
            Assert.Null(error);
        }

        [Fact]
        public async Task UserCanSaveAsync()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var mock = MockUserSession.GetInstance();
                var session = mock.MqSession.Object;
                var api = new MockAccountApi(200);
                var context = session.GetContextUser();
                Assert.NotNull(context);
                await context.Save(session, api);
            });
            Assert.Null(error);
        }
    }
}
