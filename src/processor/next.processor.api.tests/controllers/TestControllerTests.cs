using Microsoft.Extensions.DependencyInjection;
using next.processor.api.Controllers;

namespace next.processor.api.tests.controllers
{
    public class TestControllerTests : ControllerTestBase
    {

        [Fact]
        public void ControllerCanBeContructed()
        {
            var error = Record.Exception(() =>
            {
                var provider = GetProvider();
                var controller = provider.GetService<TestController>();
                Assert.NotNull(controller);
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(0, "")]
        [InlineData(0, "windows")]
        [InlineData(0, "linux")]
        [InlineData(0, "other")]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public async Task ControllerCanExecuteLandingAsync(int contextId, string? context = null)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<TestController>();
                var action = contextId switch
                {
                    0 => await controller.BrowserInstallAsync(context),
                    1 => await controller.InstallAsync(),
                    2 => await controller.VerifyAsync(),
                    3 => await controller.ReadCollinDataAsync(),
                    4 => await controller.ReadDentonDataAsync(),
                    5 => await controller.ReadHarrisDataAsync(),
                    6 => await controller.ReadTarrantDataAsync(),
                    _ => null
                };
                Assert.NotNull(action);
            });
            Assert.Null(error);
        }
    }
}