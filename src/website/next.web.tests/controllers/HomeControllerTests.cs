using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using next.web.Controllers;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.Models;

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
        [InlineData("home", false)]
        [InlineData("error")]
        [InlineData("privacy")]
        [InlineData("logout")]
        [InlineData("logout", false)]
        [InlineData("test")]
        [InlineData("discount-result")]
        [InlineData("discount-result", false)]
        [InlineData("subscription-result")]
        [InlineData("subscription-result", false)]
        [InlineData("payment-result")]
        [InlineData("payment-result", false)]
        [InlineData("payment-fetch-intent")]
        [InlineData("payment-fetch-intent", false)]
        public void ControllerCanGetContent(string landing, bool authorized = true)
        {
            var error = Record.Exception(() =>
            {
                var sut = GetProvider(authorized).GetRequiredService<HomeController>();
                var request = MockObjectProvider.GetSingle<FetchIntentRequest>();
                var result = landing switch
                {
                    "home" => sut.Index().Result,
                    "privacy" => sut.Privacy(),
                    "error" => sut.Error(),
                    "logout" => sut.Logout(),
                    "test" => sut.Test(),
                    "discount-result" => sut.DiscountLanding("success", "012345").Result,
                    "subscription-result" => sut.UserLevelLanding("success", "012345").Result,
                    "payment-result" => sut.PaymentLanding("success", "012345").Result,
                    "payment-fetch-intent" => sut.FetchIntent(request).Result,
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0, false)]
        [InlineData(1)]
        [InlineData(1, false)]
        [InlineData(2)]
        [InlineData(2, false)]
        [InlineData(3)]
        [InlineData(3, false)]
        [InlineData(4)]
        [InlineData(4, false)]
        [InlineData(5)]
        [InlineData(5, false)]
        public async Task ControllerCanGetDownloadFile(int responseId, bool getFile = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var failure = new Faker().System.Exception();
                var provider = GetProvider();
                var sut = provider.GetRequiredService<HomeController>();
                var request = MockObjectProvider.GetSingle<DownloadJsResponse>();
                var payment = MockObjectProvider.GetSingle<FetchIntentResponse>();
                var mock = provider.GetRequiredService<Mock<ISessionStringWrapper>>();
                var imock = provider.GetRequiredService<Mock<IFetchIntentService>>();
                var payjson = responseId == 5 ? null : payment.ToJsonString();
                request.Content = responseId != 3 ?
                    Properties.Resources.response_excel_file : string.Empty;
                imock.Setup(x => x.GetIntent(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(payjson);
                var json = responseId switch
                {
                    1 => string.Empty,
                    2 => "-not serializable",
                    _ => request.ToJsonString()
                };
                if (responseId != 4)
                {
                    mock.Setup(s => s
                        .GetString(It.IsAny<string>()))
                        .Returns(json);
                }
                else
                {
                    mock.Setup(s => s
                        .GetString(It.IsAny<string>()))
                        .Throws(failure);
                }
                if (getFile) sut.DownloadFile();
                else await sut.Download();
            });
            Assert.Null(error);
        }
    }
}
