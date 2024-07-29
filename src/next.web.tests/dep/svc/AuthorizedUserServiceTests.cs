using Bogus;
using legallead.desktop.entities;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using next.web.core.services;
using next.web.core.util;
using System.Security.Principal;
using System.Text;

namespace next.web.tests.dep.svc
{
    public class AuthorizedUserServiceTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, false)]
        [InlineData(true, true, false)]
        public void ServiceCanBeCreated(bool hasContext, bool isAuthorized = true, bool hasSession = true)
        {
            var error = Record.Exception(() =>
            {
                var context = hasContext ? GetHttpContext(isAuthorized, hasSession) : null;
                var obj = new AuthorizedUserService(context);
                _ = obj.UserName;
                _ = obj.Current;
                obj.Populate(testKeyName, testKeyResponse);
                _ = obj.Retrieve(testKeyName);
            });
            Assert.Null(error);
        }

        private static IHttpContextAccessor GetHttpContext(bool isAuthorized, bool hasSession = true)
        {
            var mock = new Mock<IHttpContextAccessor>();
            var request = new Mock<HttpRequest>();
            var session = new Mock<ISession>();
            var identity = new Mock<IIdentity>();
            var muser = JsonConvert.SerializeObject(faker.Generate());
            var expectedUser = Encoding.UTF8.GetBytes(muser);
            var expectedValue = Encoding.UTF8.GetBytes(testKeyResponse);
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));

            session.Setup(s => 
                s.TryGetValue(It.Is<string>(s => s == testKeyName), out expectedValue))
                .Returns(true);
            session.Setup(s =>
                s.TryGetValue(It.Is<string>(s => s == SessionKeyNames.UserBo), out expectedUser))
                .Returns(true);

            identity.SetupGet(x => x.AuthenticationType).Returns("forms");
            identity.SetupGet(x => x.IsAuthenticated).Returns(isAuthorized);
            identity.SetupGet(x => x.Name).Returns("user-name");

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );
            if (hasSession) httpContext.Session = session.Object;
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(identity.Object);
            mock.SetupGet(m => m.HttpContext).Returns(httpContext);
            return mock.Object;
        }

        private static Faker<UserBo> faker =
            new Faker<UserBo>()
            .RuleFor(x => x.UserName, y => y.Person.UserName);

        private const string testKeyName = "abcd";
        private const string testKeyResponse = "efgh";
    }
}
