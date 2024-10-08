using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;
using next.web.core.models;

namespace next.web.tests.controllers
{
    public class AppControllerTests : ControllerTestBase
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ControllerCanBeConstructed(bool authorized)
        {
            var sut = GetProvider(authorized).GetRequiredService<AppController>();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData("dallas", false)]
        [InlineData("denton", true)]
        public void ControllerCanGetCounty(string name, bool expected)
        {

            var sut = GetProvider(false).GetRequiredService<AppController>();
            var response = sut.GetCounty(new() { Name = name });
            if (response is not OkObjectResult result)
            {
                Assert.Fail("Controller response not matched to expected.");
                return;
            }
            if (result.Value is not AuthorizedCountyModel model)
            {
                Assert.Fail("Controller response not matched to expected type.");
                return;
            }
            Assert.Equal(expected, string.IsNullOrEmpty(model.Name));
            Assert.Equal(expected, string.IsNullOrEmpty(model.Code));
        }
    }
}
