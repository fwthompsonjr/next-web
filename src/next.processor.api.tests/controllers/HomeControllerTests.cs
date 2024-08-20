using Microsoft.Extensions.DependencyInjection;
using next.processor.api.Controllers;

namespace next.processor.api.tests.controllers
{
    public class HomeControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeContructed()
        {
            var error = Record.Exception(() =>
            {
                var provider = GetProvider();
                var controller = provider.GetService<HomeController>();
                Assert.NotNull(controller);
            });
            Assert.Null(error);
        }

        [Fact]
        public void ControllerCanBeGetHome()
        {
            var error = Record.Exception(() =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<HomeController>();
                _ = controller.Index();
            });
            Assert.Null(error);
        }
    }
}
