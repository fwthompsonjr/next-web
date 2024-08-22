using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;

namespace next.web.tests.controllers
{
    public class MyAccountControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<AccountController>();
            Assert.NotNull(sut);
        }
        [Theory]
        [InlineData("home")]
        [InlineData("profile")]
        [InlineData("permissions")]
        [InlineData("cache-manager")]
        public async Task ControllerCanGetContent(string landing)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = GetProvider().GetRequiredService<AccountController>();
                IActionResult? result = landing switch
                {
                    "home" => await sut.Index(),
                    "profile" => await sut.Profile(),
                    "permissions" => await sut.Permissions(),
                    "cache-manager" => await sut.CacheManagement(),
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }
    }
}