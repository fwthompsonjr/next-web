using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;

namespace next.web.tests.controllers
{
    public class InvoiceControllerTests : ControllerTestBase
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<InvoiceController>();
            Assert.NotNull(sut);
        }
        [Theory]
        [InlineData("home")]
        [InlineData("purchase")]
        [InlineData("purchase-record")]
        public async Task ControllerCanGetContent(string landing)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = GetProvider().GetRequiredService<InvoiceController>();
                IActionResult? result = landing switch
                {
                    "home" => await sut.Index(),
                    "purchase" => sut.Purchase("12345"),
                    "purchase-record" => await sut.PurchaseRecord(),
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }
    }
}