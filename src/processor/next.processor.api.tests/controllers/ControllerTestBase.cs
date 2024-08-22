using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.Controllers;

namespace next.processor.api.tests.controllers
{
    public abstract class ControllerTestBase
    {
        protected static IServiceProvider GetProvider()
        {
            lock (locker)
            {
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

                var collection = new ServiceCollection();

                collection.AddScoped(a =>
                {
                    var controller = new HomeController()
                    {
                        ControllerContext = controllerContext
                    };
                    return controller;
                });
                return collection.BuildServiceProvider();
            }
        }

        private static readonly object locker = new();
    }
}
