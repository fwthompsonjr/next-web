using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.Controllers;
using next.processor.api.interfaces;
using next.processor.api.utility;

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
                var mockinstaller = new Mock<IWebContainerInstall>();
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

                var mockchange = new Mock<IStatusChanger>();
                var svc = new Mock<IQueueExecutor>();
                svc.Setup(x => x.InstallerCount()).Returns(6);
                svc.Setup(x => x.GetDetails()).Returns(CommonKeys);
                mockinstaller.Setup(x => x.InstallAsync()).ReturnsAsync(true);
                collection.AddSingleton(svc);
                collection.AddSingleton(mockchange);
                collection.AddSingleton(mockchange.Object);
                collection.AddKeyedSingleton("linux-firefox", mockinstaller.Object);
                collection.AddKeyedSingleton("geckodriver", mockinstaller.Object);
                collection.AddKeyedSingleton("verification", mockinstaller.Object);
                collection.AddKeyedSingleton("read-collin", mockinstaller.Object);
                collection.AddKeyedSingleton("read-denton", mockinstaller.Object);
                collection.AddKeyedSingleton("read-harris", mockinstaller.Object);
                collection.AddKeyedSingleton("read-tarrant", mockinstaller.Object);
                collection.AddSingleton(a =>
                {

                    var controller = new HomeController(
                        svc.Object,
                        SettingsProvider.GetConfiguration(),
                        mockchange.Object)
                    {
                        ControllerContext = controllerContext
                    };
                    return controller;
                });
                collection.AddSingleton(a =>
                {
                    var controller = new TestController(a)
                    {
                        ControllerContext = controllerContext
                    };
                    return controller;
                });
                return collection.BuildServiceProvider();
            }
        }


        private static readonly Dictionary<string, object> CommonKeys = new()
        {
            { "1", "Number one" },
            { "2", "Number two" },
            { "3", "Number three" }
        };

        private static readonly object locker = new();
    }
}
