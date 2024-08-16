using next.web.core.extensions;
using next.web.core.models;

namespace next.web.tests.dep.extensions
{
    public class UserContextBoTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UserCanSave(bool authorized)
        {
            var error = Record.Exception(() =>
            {
                var mock = MockUserSession.GetInstance(authorized);
                var session = mock.MqSession.Object;
                var context = session.GetContextUser() ?? new();
                context.Save(session);
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UserCanSavePermissionChanged(bool authorized)
        {
            var error = Record.Exception(() =>
            {
                var mock = MockUserSession.GetInstance(authorized);
                var session = mock.MqSession.Object;
                var change = MockObjectProvider.GetSingle<PermissionChangedResponse>();
                session.Save(change);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserCanSaveAsync(bool authorized)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var mock = MockUserSession.GetInstance(authorized);
                var session = mock.MqSession.Object;
                var api = new MockAccountApi(200);
                var context = session.GetContextUser() ?? new();
                await context.Save(session, api);
            });
            Assert.Null(error);
        }
    }
}
