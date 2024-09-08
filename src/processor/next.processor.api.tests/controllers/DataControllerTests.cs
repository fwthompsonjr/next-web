using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.processor.api.Controllers;
using next.processor.api.interfaces;

namespace next.processor.api.tests.controllers
{
    public class DataControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeContructed()
        {
            var error = Record.Exception(() =>
            {
                var provider = GetProvider();
                var controller = provider.GetService<DataController>();
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
            var status = available switch
            {
                0 => "TOTAL",
                1 => "ERROR",
                2 => "PURCHASED",
                3 => "UNMAPPED",
                _ => "SUBMITTED"
            };
            var error = Record.Exception(() =>
            {
                var controller = provider.GetRequiredService<DataController>();
                _ = controller.Index(status);
            });
            Assert.Null(error);
        }
    }
}