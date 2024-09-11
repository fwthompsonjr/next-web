using Bogus;
using legallead.jdbc.entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.Controllers;
using next.processor.api.interfaces;
using next.processor.api.utility;
using next.processor.models;

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
                var fkr = new Faker();
                var count = fkr.Random.Int(1, 10);
                var dcount = fkr.Random.Int(1, 5);
                var summary = statusbofaker.Generate(count);
                var mockchange = new Mock<IStatusChanger>();
                var apisvc = new Mock<IApiWrapper>();
                var svc = new Mock<IQueueExecutor>();
                var drill = new DrillDownModel();
                var detail = countybofaker.Generate(dcount);
                svc.Setup(x => x.InstallerCount()).Returns(6);
                svc.Setup(x => x.GetDetails()).Returns(CommonKeys);
                apisvc.Setup(x => x.FetchSummaryAsync()).ReturnsAsync(summary);
                apisvc.Setup(x => x.FetchStatusAsync(It.IsAny<int>())).ReturnsAsync(detail);
                mockinstaller.Setup(x => x.InstallAsync()).ReturnsAsync(true);
                collection.AddSingleton(svc);
                collection.AddSingleton(apisvc);
                collection.AddSingleton(apisvc.Object);
                collection.AddSingleton(mockchange);
                collection.AddSingleton(mockchange.Object);
                collection.AddSingleton(drill);
                collection.AddKeyedSingleton("linux-firefox", mockinstaller.Object);
                collection.AddKeyedSingleton("linux-geckodriver", mockinstaller.Object);
                collection.AddKeyedSingleton("verification", mockinstaller.Object);
                collection.AddKeyedSingleton("read-collin", mockinstaller.Object);
                collection.AddKeyedSingleton("read-denton", mockinstaller.Object);
                collection.AddKeyedSingleton("read-harris", mockinstaller.Object);
                collection.AddKeyedSingleton("read-tarrant", mockinstaller.Object);
                collection.AddSingleton(a =>
                {

                    var controller = new HomeController(
                        svc.Object,
                        TheSettingsProvider.GetConfiguration(),
                        mockchange.Object,
                        apisvc.Object,
                        drill)
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
                collection.AddSingleton(a =>
                {
                    var controller = new DataController(drill)
                    {
                        ControllerContext = controllerContext
                    };
                    return controller;
                });
                return collection.BuildServiceProvider();
            }
        }


        private static readonly Faker<StatusSummaryBo> statusbofaker =
            new Faker<StatusSummaryBo>()
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Total, y => y.Random.Int(0, 50000));

        private static readonly Faker<StatusSummaryByCountyBo> countybofaker =
            new Faker<StatusSummaryByCountyBo>()
            .RuleFor(x => x.Region, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Oldest, y => y.Date.Recent())
            .RuleFor(x => x.Newest, y => y.Date.Recent())
            .RuleFor(x => x.Count, y => y.Random.Int(0, 50000));

        private static readonly Dictionary<string, object> CommonKeys = new()
        {
            { "1", "Number one" },
            { "2", "Number two" },
            { "3", "Number three" }
        };

        private static readonly object locker = new();
    }
}
