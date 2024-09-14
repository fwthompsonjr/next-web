using Bogus;
using next.core.entities;
using next.core.interfaces;
using next.core.utilities;
using System.Net.NetworkInformation;

namespace next.core.tests.utilities
{
    public class PermissionApiTests
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
                var api = new PermissionApi(faker.Internet.DomainName());
                Assert.NotNull(api.InternetUtility);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanInjectStatusProvider()
        {
            var mock = new ActiveInternetStatus();
            var exception = Record.Exception(() =>
            {
                var api = new PermissionApi(
                    faker.Internet.DomainName(), mock);
                Assert.NotNull(api.InternetUtility);
                Assert.Equal(api.InternetUtility, mock);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCheckAddressIsFalseWithEmptyBaseUrl()
        {
            const int expectedCode = 503;
            const string expectedMessage = "Base api address is missing or not defined.";
            var mock = new ActiveInternetStatus();
            var check = faker.Random.AlphaNumeric(6);
            var api = new PermissionApi(
                string.Empty, mock);
            var response = api.CheckAddress(check);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Fact]
        public void ServiceCheckAddressIsFalseNonDefinedEndpoint()
        {
            const int expectedCode = 404;
            const string expectedMessage = "Invalid page address.";
            var mock = new ActiveInternetStatus();
            var check = faker.Random.AlphaNumeric(6);
            var api = new PermissionApi(
                faker.Internet.DomainName(), mock);
            var response = api.CheckAddress(check);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Theory]
        [InlineData("list")]
        [InlineData("login")]
        [InlineData("read-me")]
        [InlineData("register")]
        public void ServiceCheckAddressIsFalseUnAvailableEndpoint(string pageName)
        {
            const int expectedCode = 503;
            const string expectedMessage = "Page is not available.";
            var mock = new ActiveInternetStatus();
            var api = new InactivePageAddress(
                faker.Internet.DomainName(), mock);
            var response = api.CheckAddress(pageName);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Theory]
        [InlineData("list")]
        [InlineData("read-me")]
        public void ServiceCheckAddressIsTrueAvailableEndpoint(string pageName)
        {
            const int expectedCode = 200;
            var mock = new ActiveInternetStatus();
            var api = new PermissionApi(
                faker.Internet.DomainName(), mock);
            var response = api.CheckAddress(pageName);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
        }

        [Theory]
        [InlineData("list", 200)]
        [InlineData("read-me", 200)]
        [InlineData("user-permissions-list", 200)]
        [InlineData("user-us-county-list", 200)]
        [InlineData("user-us-state-list", 200)]
        [InlineData("non-existing", 404)]
        public async Task ServiceShouldGetEndpoint(string pageName, int expectedCode)
        {
            var mock = new ActiveInternetStatus();
            var api = new PermissionApi(
                faker.Internet.DomainName(), mock);
            var response = await api.Get(pageName);
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
        }

        [Theory]
        [InlineData("login", 200)]
        [InlineData("refresh", 200)]
        [InlineData("password", 200)]
        [InlineData("change-password", 200)]
        [InlineData("setting-application-key", 200)]
        [InlineData("non-existing", 404)]
        public async Task ServiceShouldGetPostEndpoint(string pageName, int expectedCode)
        {
            var mock = new ActiveInternetStatus();
            var api = new PermissionApi(
                faker.Internet.DomainName(), mock);
            var response = await api.Post(pageName, new(), userfaker.Generate());
            Assert.Equal(expectedCode, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
        }

        [Theory]
        [InlineData("google.com", null, true)]
        [InlineData("address-mock-valid", true, true)]
        [InlineData("address-mock-false", false, false)]
        [InlineData("exception", false, false)]
        public void ServiceCanCheckValidUri(string uri, bool? connectExpression, bool expected)
        {
            if (uri.Equals(""))
            {
                var exception = Record.Exception(() =>
                {
                    _ = ActivePageAddress.CanConnect(uri, connectExpression);
                });
                Assert.NotNull(exception);
                return;
            }
            var actual = ActivePageAddress.CanConnect(uri, connectExpression);
            Assert.Equal(expected, actual);
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }

        private sealed class InactivePageAddress : PermissionApi
        {
            public InactivePageAddress(string baseUri) : base(baseUri)
            {
            }

            public InactivePageAddress(string baseUri, IInternetStatus status) : base(baseUri, status)
            {
            }

            protected override bool GetConnectionStatus(string name, string address)
            {
                return false;
            }
        }

        private sealed class ActivePageAddress : PermissionApi
        {
            private static readonly object locker = new();

            public ActivePageAddress(string baseUri) : base(baseUri)
            {
            }

            public ActivePageAddress(string baseUri, IInternetStatus status) : base(baseUri, status)
            {
            }

            protected override bool GetConnectionStatus(string name, string address)
            {
                return true;
            }

            public static bool CanConnect(string address, bool? expected = null)
            {
                lock (locker)
                {
                    if (address.Equals("exception"))
                    {
                        return CanConnectToPage(address, new ExceptionPinger());
                    }
                    if (expected == null)
                    {
                        return CanConnectToPage(address);
                    }
                    var actual = expected.GetValueOrDefault();
                    IPingAddress addressPinger = actual ? new ActivePinger() : new InActivePinger();
                    return CanConnectToPage(address, addressPinger);
                }
            }

            private sealed class ActivePinger : IPingAddress
            {
                public IPStatus CheckStatus(string address)
                {
                    return IPStatus.Success;
                }
            }

            private sealed class InActivePinger : IPingAddress
            {
                private static readonly Faker faker = new();

                public IPStatus CheckStatus(string address)
                {
                    var items = Enum.GetValues(typeof(IPStatus))
                        .Cast<IPStatus>()
                        .Where(s => s != IPStatus.Success);
                    return faker.PickRandom(items);
                }
            }

            private sealed class ExceptionPinger : IPingAddress
            {
                public IPStatus CheckStatus(string address)
                {
                    throw new Exception();
                }
            }
        }
    }
}