using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.Controllers;
using next.processor.api.interfaces;
using static Microsoft.VisualStudio.Threading.AsyncReaderWriterLock;

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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void ControllerCanBeGetHome(int available)
        {
            var provider = GetProvider();
            var svc = provider.GetRequiredService<Mock<IQueueExecutor>>();
            svc.Setup(x => x.IsReadyCount()).Returns(available);
            var error = Record.Exception(() =>
            {
                var controller = provider.GetRequiredService<HomeController>();
                _ = controller.Index();
            });
            Assert.Null(error);
        }
    }
}
