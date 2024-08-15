using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace next.web.tests.web
{
    public class ProgramExtensionsTest
    {
        [Fact]
        public void CollectionCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var provider = new ServiceCollection();
                provider.Configure();
            });
            Assert.Null(error);
        }
        [Fact]
        public void AppCanBeBuilt()
        {
            var error = Record.Exception(() =>
            {
                var builder = WebApplication.CreateBuilder();
                var services = builder.Services;
                services.Configure();
                var app = builder.Build();
                app.ConfigureApp();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AppCanConfigureExceptionAndHsts(bool isDevelopment)
        {
            var error = Record.Exception(() =>
            {
                var builder = WebApplication.CreateBuilder();
                var services = builder.Services;
                services.Configure();
                var app = builder.Build();
                app.SetExceptionAndHsts(isDevelopment);
            });
            Assert.Null(error);
        }
    }
}
