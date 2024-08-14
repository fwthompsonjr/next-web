using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;

namespace next.web.tests.controllers
{
    public class SearchControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<SearchController>();
            Assert.NotNull(sut);
        }
        [Theory]
        [InlineData("home")]
        [InlineData("active")]
        [InlineData("purchases")]
        [InlineData("history")]
        public async Task ControllerCanGetContent(string landing)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = GetProvider().GetRequiredService<SearchController>();
                IActionResult? result = landing switch
                {
                    "home" => await sut.Index(),
                    "active" => await sut.Active(),
                    "purchases" => await sut.Purchases(),
                    "history" => await sut.History(),
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }
    }
}