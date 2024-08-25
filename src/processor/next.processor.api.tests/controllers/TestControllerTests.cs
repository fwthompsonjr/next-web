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
        [InlineData(1)]
        [InlineData(2)]
        public async Task ControllerCanExecuteLandingAsync(int contextId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<TestController>();
                var action = contextId switch
                {
                    0 => await controller.BrowserInstallAsync(),
                    1 => await controller.InstallAsync(),
                    2 => await controller.VerifyAsync(),
                    _ => null
                };
                Assert.NotNull(action);
            });
            Assert.Null(error);
        }
    }
}
/*
using Microsoft.Extensions.DependencyInjection;
using next.processor.api.Controllers;

namespace next.processor.api.tests.controllers
{
    public class HomeControllerTests : ControllerTestBase
    {

        
    }
}


*/