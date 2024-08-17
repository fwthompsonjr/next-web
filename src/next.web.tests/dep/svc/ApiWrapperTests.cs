using Microsoft.Extensions.DependencyInjection;
using next.web.core.util;
using next.web.Services;
using legallead.desktop.interfaces;
using Moq;

namespace next.web.tests.dep.svc
{
    public class ApiWrapperTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(0, false)]
        [InlineData(1)]
        [InlineData(1, false)]
        [InlineData(2)]
        [InlineData(2, false)]
        [InlineData(3)]
        [InlineData(3, false)]
        [InlineData(4)]
        [InlineData(4, false)]
        public async Task WrapperCanExecuteMethods(int methodid, bool authorized = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var parser = AppContainer.ServiceProvider?
                    .GetService<IContentParser>() ?? new Mock<IContentParser>().Object;
                var expected = authorized ? 200 : 401;
                var api = new MockAccountApi(expected);
                var sut = new ApiWrapper(api, parser);
                var mock = MockUserSession.GetInstance(authorized);
                var mocksession = mock.MqSession;
                var session = mocksession.Object;
                Dictionary<string, string> parameters = new()
                {
                    { "home", "item" },
                    { "id", "0" }
                };
                var response = methodid switch
                {
                    4 => await sut.Post("a", parameters, session),
                    3 => await sut.Get("a", session, parameters),
                    2 => await sut.Get("a", parameters),
                    1 => await sut.Get("a", session),
                    _ => await sut.Get("a")
                };

                Assert.Equal(expected, response.StatusCode);
            });
            Assert.Null(error);
        }
    }
}