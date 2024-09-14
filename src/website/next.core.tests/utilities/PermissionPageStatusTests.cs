using Bogus;
using next.core.entities;
using next.core.interfaces;
using next.core.utilities;

namespace next.core.tests.utilities
{
    public class PermissionPageStatusTests
    {
        private readonly Faker faker = new();

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
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var api = new PermissionPageStatus(faker.Internet.DomainName());
                Assert.NotNull(api.InternetUtility);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("login", 200)]
        [InlineData("refresh", 200)]
        [InlineData("password", 200)]
        [InlineData("change-password", 200)]
        [InlineData("non-existing", 404)]
        public void ServiceShouldGetPostAddress(string pageName, int expectedCode)
        {
            var mock = new ActiveInternetStatus();
            var api = new MockPermissionPageStatus(
                faker.Internet.DomainName(), mock);
            var response = api.TestPostAddress(pageName, userfaker.Generate());
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
        }

        [Theory]
        [InlineData("list", 200)]
        [InlineData("read-me", 200)]
        [InlineData("non-existing", 404)]
        public void ServiceShouldGetAddress(string pageName, int expectedCode)
        {
            var mock = new ActiveInternetStatus();
            var api = new MockPermissionPageStatus(
                faker.Internet.DomainName(), mock);
            var response = api.TestGetAddress(pageName);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
        }

        [Theory]
        [InlineData("login", 400)]
        [InlineData("refresh", 400)]
        [InlineData("password", 400)]
        [InlineData("change-password", 400)]
        public void ServiceShouldNotGetPostAddressWhenNotInitialized(string pageName, int expectedCode)
        {
            var user = userfaker.Generate();
            user.Applications = Array.Empty<ApiContext>();
            var mock = new ActiveInternetStatus();
            var api = new MockPermissionPageStatus(
                faker.Internet.DomainName(), mock);
            var response = api.TestPostAddress(pageName, user);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }

        private sealed class MockPermissionPageStatus : PermissionPageStatus
        {
            public MockPermissionPageStatus(string baseUri) : base(baseUri)
            {
            }

            public MockPermissionPageStatus(string baseUri, IInternetStatus status) : base(baseUri, status)
            {
            }

            public ApiResponse TestPostAddress(string name, UserBo user)
            {
                return PostAddress(name, user);
            }

            public ApiResponse TestGetAddress(string name)
            {
                return GetAddress(name);
            }
        }
    }
}