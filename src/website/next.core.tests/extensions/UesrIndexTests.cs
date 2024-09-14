using Bogus;
using next.core.entities;
using next.core.extensions;
using next.core.interfaces;
using next.core.utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace next.core.tests.extensions
{
    public class UesrIndexTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true, 404)]
        public async Task UserCanGetUserId(
            bool hasUserGuid = true,
            bool canSerialize = true,
            bool hasMessage = true,
            int statusCode = 200)
        {
            var provider = GetProvider();
            var user = provider.GetRequiredService<UserBo>();
            object profileResponse = canSerialize ?
                provider.GetRequiredService<ContactProfileResponse>() :
                new { NotProfile = true };
            if (!hasUserGuid && profileResponse is ContactProfileResponse contact)
            {
                contact.Message = "abcdefgh";
                profileResponse = contact;
            }
            if (!hasMessage && profileResponse is ContactProfileResponse contact1)
            {
                contact1.Message = "";
                profileResponse = contact1;
            }
            var response = new ApiResponse
            {
                StatusCode = statusCode,
                Message = JsonConvert.SerializeObject(profileResponse)
            };
            var mock = new PositiveRespondingApi("http://test.api", response);
            var problems = await Record.ExceptionAsync(async () =>
            {
                _ = await user.GetUserId(mock);
            });
            Assert.Null(problems);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true, 404)]
        public async Task UserCansetUserId(
            bool hasUserGuid = true,
            bool canSerialize = true,
            bool hasMessage = true,
            int statusCode = 200)
        {
            var provider = GetProvider();
            var user = provider.GetRequiredService<UserBo>();
            var filter = provider.GetRequiredService<IQueueFilter>();
            object profileResponse = canSerialize ?
                provider.GetRequiredService<ContactProfileResponse>() :
                new { NotProfile = true };
            if (!hasUserGuid && profileResponse is ContactProfileResponse contact)
            {
                contact.Message = "abcdefgh";
                profileResponse = contact;
            }
            if (!hasMessage && profileResponse is ContactProfileResponse contact1)
            {
                contact1.Message = "";
                profileResponse = contact1;
            }
            var response = new ApiResponse
            {
                StatusCode = statusCode,
                Message = JsonConvert.SerializeObject(profileResponse)
            };
            var mock = new PositiveRespondingApi("http://test.api", response);
            var problems = await Record.ExceptionAsync(async () =>
            {
                await user.SetUserId(mock, filter);
            });
            Assert.Null(problems);
        }

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

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }

        private sealed class PositiveRespondingApi : PermissionApi
        {
            private readonly ApiResponse _message;

            public PositiveRespondingApi(string baseUri, ApiResponse response) : base(baseUri, new ActiveInternetStatus())
            {
                _message = response;
            }

            public override async Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                var response = await Task.Run(() => { return _message; });
                return response;
            }
        }
        private ServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            var faker = new Faker();
            var user = userfaker.Generate();
            var filter = new Mock<IQueueFilter>();
            var response = new ContactProfileResponse
            {
                IsOK = true,
                Message = faker.Random.Guid().ToString(),
            };
            user.Token = tokenfaker.Generate();

            services.AddSingleton(user);
            services.AddSingleton(filter);
            services.AddSingleton(filter.Object);
            services.AddSingleton(response);

            return services.BuildServiceProvider();
        }
    }
}
