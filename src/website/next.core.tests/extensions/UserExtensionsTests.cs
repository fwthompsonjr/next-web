using Bogus;
using Newtonsoft.Json;
using next.core.entities;
using next.core.extensions;
using next.core.interfaces;
using next.core.utilities;

namespace next.core.tests.extensions
{
    public class UserExtensionsTests
    {
        private readonly Faker<AccessTokenBo> tokenfaker =
            new Faker<AccessTokenBo>()
            .RuleFor(x => x.AccessToken, y => y.Random.AlphaNumeric(40))
            .RuleFor(x => x.RefreshToken, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Expires, y => y.Date.Future());

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
        public void AppendRequiresUserToken()
        {
            using var client = new HttpClient();
            var user = userfaker.Generate();
            user.Token = null;
            client.AppendAuthorization(user);
            var headers = client.DefaultRequestHeaders;
            var added = headers.Any(a => a.Key == "Authorization");
            Assert.False(added);
        }

        [Fact]
        public void AppendRequiresUserAccessToken()
        {
            using var client = new HttpClient();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.AccessToken = string.Empty;
            client.AppendAuthorization(user);
            var headers = client.DefaultRequestHeaders;
            var added = headers.Any(a => a.Key == "Authorization");
            Assert.False(added);
        }

        [Fact]
        public void AppendRequiresUserExpiryDate()
        {
            using var client = new HttpClient();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = null;
            client.AppendAuthorization(user);
            var headers = client.DefaultRequestHeaders;
            var added = headers.Any(a => a.Key == "Authorization");
            Assert.False(added);
        }

        [Fact]
        public void AppendWithValidUserWillAddHeader()
        {
            using var client = new HttpClient();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = DateTime.UtcNow.AddMinutes(5);
            client.AppendAuthorization(user);
            var headers = client.DefaultRequestHeaders;
            var added = headers.Any(a => a.Key == "Authorization");
            Assert.True(added);
        }

        [Fact]
        public void ValidUserShouldHaveASessionId()
        {
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = DateTime.UtcNow.AddMinutes(5);
            var actual = user.GetSessionId();
            Assert.False(string.IsNullOrWhiteSpace(actual));
            Assert.False(actual.Contains('-'));
        }

        [Fact]
        public void ExpiringUserShouldHaveASessionId()
        {
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = DateTime.UtcNow.AddSeconds(45);
            var actual = user.GetSessionId();
            Assert.False(string.IsNullOrWhiteSpace(actual));
            Assert.True(actual.Contains('-'));
        }

        [Fact]
        public async Task ExtendTokenWithoutApiShouldExit()
        {
            IPermissionApi? api = null;
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = DateTime.UtcNow.AddMinutes(5);
            await user.ExtendToken(api);
            Assert.NotNull(user.Token);
        }

        [Fact]
        public async Task ExtendTokenWithoutUserTokenExit()
        {
            var api = GetInvalidMockApi();
            var user = userfaker.Generate();
            user.Token = null;
            await user.ExtendToken(api);
            Assert.Null(user.Token);
        }

        [Fact]
        public async Task ExtendTokenWithoutAccessTokenExit()
        {
            var api = GetInvalidMockApi();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = DateTime.UtcNow.AddMinutes(5);
            user.Token.AccessToken = string.Empty;
            await user.ExtendToken(api);
            Assert.NotNull(user.Token);
        }

        [Fact]
        public async Task ExtendTokenWithValidTokenWillExit()
        {
            var api = GetInvalidMockApi();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            var bearer = user.Token.AccessToken;
            user.Token.Expires = DateTime.UtcNow.AddMinutes(5);
            await user.ExtendToken(api);
            Assert.Equal(bearer, user.Token.AccessToken);
        }

        [Fact]
        public async Task ExtendTokenWithExpiredTokenSends500Response()
        {
            var api = GetInvalidMockApi();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            var bearer = user.Token.AccessToken;
            user.Token.Expires = DateTime.UtcNow.AddMinutes(-5);
            await user.ExtendToken(api);
            Assert.NotEqual(bearer, user.Token?.AccessToken);
        }

        [Fact]
        public async Task ExtendTokenWithNearlyExpiredTokenSends200Response()
        {
            var api = GetMockApi();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            var bearer = user.Token.AccessToken;
            user.Token.Expires = DateTime.UtcNow.AddSeconds(15);
            await user.ExtendToken(api);
            Assert.NotEqual(bearer, user.Token?.AccessToken);
        }

        [Fact]
        public async Task ExtendTokenWithInvalidApiResponseResponseIsNull()
        {
            var api = GetExpiredMockApi();
            var user = userfaker.Generate();
            user.Token = tokenfaker.Generate();
            user.Token.Expires = DateTime.UtcNow.AddMinutes(-5);
            await user.ExtendToken(api);
            Assert.Null(user.Token);
        }

        private IPermissionApi GetMockApi()
        {
            var faker = new Faker();
            var token = tokenfaker.Generate();
            var mock = new ActiveInternetStatus();
            return new PositiveRespondingApi(
                faker.Internet.DomainName(), mock, token);
        }

        private IPermissionApi GetExpiredMockApi()
        {
            var faker = new Faker();
            var token = tokenfaker.Generate();
            var mock = new ActiveInternetStatus();
            return new ExpiriedRespondingApi(
                faker.Internet.DomainName(), mock, token);
        }

        private static IPermissionApi GetInvalidMockApi()
        {
            var faker = new Faker();
            var mock = new ActiveInternetStatus();
            return new InvalidRespondingApi(
                faker.Internet.DomainName(), mock);
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }

        private sealed class InvalidRespondingApi : PermissionApi
        {
            public InvalidRespondingApi(string baseUri) : base(baseUri)
            {
            }

            public InvalidRespondingApi(string baseUri, IInternetStatus status) : base(baseUri, status)
            {
            }

            public override async Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                var response = await Task.Run(() => { return new ApiResponse { StatusCode = 500 }; });
                return response;
            }
        }

        private sealed class PositiveRespondingApi : PermissionApi
        {
            private readonly AccessTokenBo accessToken;

            public PositiveRespondingApi(string baseUri, IInternetStatus status, AccessTokenBo token) : base(baseUri, status)
            {
                accessToken = token;
            }

            public override async Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                var message = JsonConvert.SerializeObject(accessToken);
                var obj = new ApiResponse { StatusCode = 200, Message = message };
                var response = await Task.Run(() => { return obj; });
                return response;
            }
        }

        private sealed class ExpiriedRespondingApi : PermissionApi
        {
            private readonly AccessTokenBo accessToken;

            public ExpiriedRespondingApi(string baseUri, IInternetStatus status, AccessTokenBo token) : base(baseUri, status)
            {
                token.Expires = null;
                accessToken = token;
            }

            public override async Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                var message = JsonConvert.SerializeObject(accessToken);
                var obj = new ApiResponse { StatusCode = 200, Message = message };
                var response = await Task.Run(() => { return obj; });
                return response;
            }
        }
    }
}