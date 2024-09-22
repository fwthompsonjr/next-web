using Bogus;
using Moq;
using Newtonsoft.Json;
using next.core.entities;
using next.core.interfaces;
using next.core.utilities;
using System.Text.Json;

namespace next.core.tests.utilities
{
    public class PermissionPageClientTests
    {
        private static readonly Faker<ApiContext> contextfaker =
            new Faker<ApiContext>()
            .RuleFor(x => x.Id, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        private readonly Faker<UserBo> userfaker =
            new Faker<UserBo>()
            .RuleFor(x => x.Applications, y =>
            {
                var count = y.Random.Int(1, 6);
                return contextfaker.Generate(count).ToArray();
            })
            .RuleFor(x => x.UserName, y => y.Company.CompanyName());

        [Fact]
        public void ClientCanGetHttpWrapper()
        {
            var api = new MockPermissionPageClient();
            var expected = api.GetWrapper();
            Assert.NotNull(expected);
        }

        [Theory]
        [InlineData("non-existing", 404)]
        public async Task ClientGetNeedsValidPageName(string pageName, int expectedCode)
        {
            var api = new MockPermissionPageClient();
            var response = await api.Get(pageName);
            Assert.NotNull(response);
            Assert.Equal(expectedCode, response.StatusCode);
        }

        [Theory]
        [InlineData("list", 200)]
        [InlineData("read-me", 200)]
        public async Task ClientGetWithValidPageName(string pageName, int expectedCode)
        {
            var api = new MockPermissionPageClient();
            var mock = api.MqClient;
            mock.Setup(s => s.GetStringAsync(
                It.IsAny<HttpClient>(),
                It.IsAny<string>())).ReturnsAsync("success");
            var response = await api.Get(pageName);
            Assert.NotNull(response);
            Assert.Equal(expectedCode, response.StatusCode);
        }

        [Theory]
        [InlineData("list", 500)]
        [InlineData("read-me", 500)]
        public async Task ClientGetNullResponse(string pageName, int expectedCode)
        {
            string noresponse = string.Empty;
            var api = new MockPermissionPageClient();
            var mock = api.MqClient;
            mock.Setup(s => s.GetStringAsync(
                It.IsAny<HttpClient>(),
                It.IsAny<string>())).ReturnsAsync(noresponse);
            var response = await api.Get(pageName);
            Assert.NotNull(response);
            Assert.Equal(expectedCode, response.StatusCode);
        }

        [Theory]
        [InlineData("login", 200)]
        [InlineData("refresh", 200)]
        [InlineData("password", 200)]
        [InlineData("change-password", 200)]
        [InlineData("non-existing", 404)]
        public async Task ClientPostWithValidPageName(string pageName, int expectedCode)
        {
            var user = userfaker.Generate();
            var api = new MockPermissionPageClient();
            var apiresponse = new ApiResponse { StatusCode = 200, Message = "OK" };
            var serialized = JsonConvert.SerializeObject(apiresponse);
            var message = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(serialized, System.Text.Encoding.UTF8, "application/json")
            };
            var mock = api.MqClient;
            mock.Setup(s => s.PostAsJsonAsync<object>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<JsonSerializerOptions>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(message);
            var postresponse = await api.Post(pageName, new(), user);
            Assert.NotNull(postresponse);
            Assert.Equal(expectedCode, postresponse.StatusCode);
        }

        [Theory]
        [InlineData("login")]
        [InlineData("refresh")]
        [InlineData("password")]
        [InlineData("change-password")]
        public async Task ClientPostWithNullApiResponse(string pageName)
        {
            var user = userfaker.Generate();
            var api = new MockPermissionPageClient();
            var message = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Error occurred", System.Text.Encoding.UTF8, "application/json")
            };
            var mock = api.MqClient;
            mock.Setup(s => s.PostAsJsonAsync<object>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<JsonSerializerOptions>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(message);
            var postresponse = await api.Post(pageName, new(), user);
            Assert.NotNull(postresponse);
            Assert.Equal(500, postresponse.StatusCode);
        }

        private sealed class MockPermissionPageClient : PermissionPageClient
        {
            private static readonly Faker faker = new();
            private readonly Mock<IHttpClientWrapper> mqClient = new();

            public MockPermissionPageClient() : base(faker.Internet.DomainName(), new ActiveInternetStatus())
            {
            }

            public Mock<IHttpClientWrapper> MqClient => mqClient;

            public IHttpClientWrapper GetWrapper()
            {
                return base.GetHttpClient();
            }

            protected override IHttpClientWrapper GetHttpClient()
            {
                return MqClient.Object;
            }
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }
    }
}