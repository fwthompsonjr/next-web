using Bogus;
using Newtonsoft.Json;
using next.core.entities;
using next.core.implementations;
using next.core.interfaces;

namespace next.core.tests.implementations
{
    public class UserPermissionsMapperTests
    {
        [Theory]
        [InlineData(2, 2)]
        [InlineData(null, 2)]
        [InlineData(null, 5)]
        public async Task PermissionsCanBeMapped(int? countiesCount = 5, int? statesCount = 5)
        {
            var user = GetUser(true);
            var homepage = GetHome();
            var api = new MockApi(countiesCount, statesCount);
            var mapper = new UserPermissionsMapper();
            var response = await mapper.Map(api, user, homepage);
            Assert.False(string.IsNullOrEmpty(response));
        }


        private static UserBo GetUser(bool isInitialized)
        {
            var faker = new Faker();
            var user = new UserBo()
            {
                UserName = faker.Person.Email,
                Applications = new ApiContext[] { new() { Id = Guid.Empty.ToString(), Name = "next.core.tests" } }
            };
            if (isInitialized) return user;
            user.Applications = Array.Empty<ApiContext>();
            return user;
        }

        private static string GetHome()
        {
            if (!string.IsNullOrEmpty(_content)) return _content;
            var content = new ContentHtmlNames();
            var home = content.GetContent("home");
            if (home == null) return string.Empty;
            _content = home.Content;
            return _content;
        }

        private static string? _content;
        private static class PermissionProvider
        {
            private static readonly string[] keynames = new[] {
            "Account.Permission.Level",
            "Setting.State.Subscriptions.Active",
            "Setting.State.Subscriptions",
            "Setting.State.County.Subscriptions"
            };

            private static readonly Faker<ContactPermissionResponse> faker =
                new Faker<ContactPermissionResponse>()
                .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(15))
                .RuleFor(x => x.UserName, y => y.Person.FullName)
                .RuleFor(x => x.KeyName, y => y.PickRandom(keynames))
                .RuleFor(x => x.KeyValue, y => y.Company.CompanyName());

            public static List<ContactPermissionResponse> GetResponses()
            {
                var permissions = faker.Generate(keynames.Length);
                permissions.ForEach(p =>
                {
                    var id = permissions.IndexOf(p);
                    p.KeyValue = keynames[id];
                });
                return permissions;
            }

        }

        private static class StateResponseProvider
        {

            private static readonly Faker<ContactUsStateResponse> faker =
                new Faker<ContactUsStateResponse>()
                .RuleFor(x => x.Id, y => y.Random.Int(100, 5000).ToString())
                .RuleFor(x => x.ShortName, y => y.Random.AlphaNumeric(2))
                .RuleFor(x => x.Name, y => y.Company.CompanyName())
                .RuleFor(x => x.IsActive, y => y.Random.Bool());

            private static readonly Faker<ContactUsStateCountyResponse> countyfaker =
                new Faker<ContactUsStateCountyResponse>()
                .RuleFor(x => x.Index, y => y.Random.Int(100, 5000))
                .RuleFor(x => x.StateCode, y => y.Random.AlphaNumeric(2))
                .RuleFor(x => x.Name, y => y.Company.CompanyName())
                .RuleFor(x => x.IsActive, y => y.Random.Bool());


            public static List<ContactUsStateResponse> GetStates(int count)
            {
                return faker.Generate(count);
            }
            public static List<ContactUsStateCountyResponse> GetCounties(int count)
            {
                return countyfaker.Generate(count);
            }
        }

        private sealed class MockApi : IPermissionApi
        {
            private readonly int? _counties;
            private readonly int? _states;
            public MockApi(int? countyCount, int? stateCount)
            {
                _counties = countyCount;
                _states = stateCount;
            }

            public async Task<ApiResponse> Get(string name, UserBo user)
            {
                var answer = await Task.Run(() =>
                {
                    var failure = new ApiResponse() { StatusCode = 400, Message = "Undefined" };
                    var known = new[] { "user-permissions-list", "user-us-county-list", "user-us-state-list" };
                    if (!known.Contains(name)) return failure;
                    var response = new ApiResponse { StatusCode = 200 };
                    if (known[0].Equals(name)) response.Message = JsonConvert.SerializeObject(PermissionProvider.GetResponses());
                    if (known[1].Equals(name) && !_counties.HasValue) return failure;
                    if (known[1].Equals(name)) response.Message = JsonConvert.SerializeObject(StateResponseProvider.GetCounties(_counties.GetValueOrDefault()));
                    if (known[2].Equals(name) && !_states.HasValue) return failure;
                    if (known[2].Equals(name)) response.Message = JsonConvert.SerializeObject(StateResponseProvider.GetStates(_states.GetValueOrDefault()));
                    return response;
                });
                return answer;
            }

            public Task<ApiResponse> Get(string name)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Get(string name, Dictionary<string, string> parameters)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Get(string name, UserBo user, Dictionary<string, string> parameters)
            {
                throw new NotImplementedException();
            }

            public IInternetStatus? InternetUtility => throw new NotImplementedException();

            public KeyValuePair<bool, ApiResponse> CanGet(string name)
            {
                throw new NotImplementedException();
            }

            public KeyValuePair<bool, ApiResponse> CanPost(string name, object payload, UserBo user)
            {
                throw new NotImplementedException();
            }

            public ApiResponse CheckAddress(string name)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                throw new NotImplementedException();
            }
        }
    }
}
