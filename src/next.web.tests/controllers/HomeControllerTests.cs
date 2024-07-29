using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;

namespace next.web.tests.controllers
{
    public class HomeControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<HomeController>();
            Assert.NotNull(sut);
        }
        [Theory]
        [InlineData("home")]
        [InlineData("error")]
        [InlineData("privacy")]
        public void ControllerCanGetContent(string landing)
        {
            var error = Record.Exception(() =>
            {
                var sut = GetProvider().GetRequiredService<HomeController>();
                var result = landing switch
                {
                    "home" => sut.Index(),
                    "privacy" => sut.Privacy(),
                    "error" => sut.Error(),
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }
    }
}
