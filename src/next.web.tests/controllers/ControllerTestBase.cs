using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using next.web.Controllers;
using next.web.core.interfaces;
using next.web.core.util;
using next.web.Services;

namespace next.web.tests.controllers
{
    public abstract class ControllerTestBase
    {
        private static IServiceProvider? _serviceProvider;
        protected static IServiceProvider GetProvider(bool authorized = true)
        {
            if (_serviceProvider != null) { return _serviceProvider; }

            //Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );
            var mock = MockUserSession.GetInstance(authorized);
            httpContext.Session = mock.MqSession.Object;
            //Controller needs a controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var statusCode = authorized ? 200 : 401;
            var mwrapper = new Mock<ISessionStringWrapper>();
            var iwrapper = new Mock<IFetchIntentService>();
            var homeLogger = new Mock<ILogger<HomeController>>();
            var apiWrapper = new Mock<IApiWrapper>();
            var concrete = new ApiWrapper(new MockAccountApi(statusCode));
            apiWrapper.Setup(x => x.Post(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<ISession>(),
                It.IsAny<string?>())).Callback(async (string a, object obj, ISession session, string? js) =>
                {
                    await concrete.Post(a, obj, session, js);
                });
            var collection = new ServiceCollection();
            collection.AddScoped(s => request);
            collection.AddScoped(s => mock);
            collection.AddScoped(s => homeLogger);
            collection.AddScoped(s => homeLogger.Object);
            collection.AddScoped(s => mwrapper);
            collection.AddScoped(s => mwrapper.Object);
            collection.AddScoped(s => iwrapper);
            collection.AddScoped(s => iwrapper.Object);
            collection.AddScoped(s => apiWrapper);
            collection.AddScoped(s => apiWrapper.Object);
            collection.AddScoped(a =>
            {
                var logger = a.GetRequiredService<ILogger<HomeController>>();
                var controller = new HomeController(logger, apiWrapper.Object, mwrapper.Object, iwrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new AccountController(apiWrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new MailController(apiWrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new SearchController(apiWrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new InvoiceController(apiWrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new DataController(apiWrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            var svc = AppContainer.ServiceProvider?.GetServices<IJsHandler>().ToList();
            svc?.ForEach(s => s.Wrapper = concrete);
            _serviceProvider = collection.BuildServiceProvider();
            return _serviceProvider;
        }
    }
}
