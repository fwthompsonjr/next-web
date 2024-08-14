using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;

namespace next.web.tests.controllers
{
    public class MailControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<MailController>();
            Assert.NotNull(sut);
        }
        [Theory]
        [InlineData("home")]
        public async Task ControllerCanGetContent(string landing)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = GetProvider().GetRequiredService<MailController>();
                IActionResult? result = landing switch
                {
                    "home" => await sut.Index(),
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }
    }
}