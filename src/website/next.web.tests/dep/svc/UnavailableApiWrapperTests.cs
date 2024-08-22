using Microsoft.AspNetCore.Http;
using Moq;
using next.web.Services;

namespace next.web.tests.dep.svc
{
    public class UnavailableApiWrapperTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task WrapperCanExecuteMethods(int methodid)
        {
            const string notAvailableMessage = "Service is not available";
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = new UnavailableApiWrapper();
                var mocksession = new Mock<ISession>();
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
                Assert.Equal(503, response.StatusCode);
                Assert.Equal(notAvailableMessage, response.Message);
            });
            Assert.Null(error);
        }
    }
}
