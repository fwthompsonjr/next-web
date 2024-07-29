using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using next.web.Controllers;

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

            //Controller needs a controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var homeLogger = new Mock<ILogger<HomeController>>();
            var collection = new ServiceCollection();
            collection.AddScoped(s => request);
            collection.AddScoped(s => homeLogger);
            collection.AddScoped(s => homeLogger.Object);
            collection.AddScoped(a =>
            {
                var logger = a.GetRequiredService<ILogger<HomeController>>();
                return new HomeController(logger)
                {
                    ControllerContext = controllerContext
                };
            });
            _serviceProvider = collection.BuildServiceProvider();
            return _serviceProvider;
        }
    }
}
