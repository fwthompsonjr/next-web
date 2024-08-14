using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using next.web.Controllers;
using next.web.core.interfaces;

namespace next.web.tests.controllers
{
    public abstract class ControllerTestBase
    {
        private static IServiceProvider? _serviceProvider;
        protected static IServiceProvider GetProvider()
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
            var mock = MockUserSession.GetInstance();
            httpContext.Session = mock.MqSession.Object;
            //Controller needs a controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var mwrapper = new Mock<ISessionStringWrapper>();
            var iwrapper = new Mock<IFetchIntentService>();
            var homeLogger = new Mock<ILogger<HomeController>>();
            var collection = new ServiceCollection();
            collection.AddScoped(s => request);
            collection.AddScoped(s => mock);
            collection.AddScoped(s => homeLogger);
            collection.AddScoped(s => homeLogger.Object);
            collection.AddScoped(s => mwrapper);
            collection.AddScoped(s => mwrapper.Object);
            collection.AddScoped(s => iwrapper);
            collection.AddScoped(s => iwrapper.Object);
            collection.AddScoped(a =>
            {
                var logger = a.GetRequiredService<ILogger<HomeController>>();
                var controller = new HomeController(logger, mwrapper.Object, iwrapper.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new AccountController()
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new MailController()
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new SearchController()
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            collection.AddScoped(a =>
            {
                var controller = new InvoiceController()
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            _serviceProvider = collection.BuildServiceProvider();
            return _serviceProvider;
        }
    }
}
