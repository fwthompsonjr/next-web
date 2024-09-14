using Bogus;
using next.core.entities;
using next.core.interfaces;
using next.core.utilities;

namespace next.core.tests.utilities
{
    public class PermissionApiGetTests
    {
        private readonly Faker faker = new();

        [Fact]
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var api = new PermissionPageClient(faker.Internet.DomainName());
                Assert.NotNull(api.InternetUtility);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("read-me", true)]
        [InlineData("list", true)]
        [InlineData("application-state-configuration", true)]
        [InlineData("login", false)]
        [InlineData("misspelled", false)]
        public void ServiceCanGetPageUrl(string landing, bool expected)
        {
            var mock = new ActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var actual = service.CanGet(landing).Key;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", true)]
        [InlineData("list", true)]
        [InlineData("application-state-configuration", true)]
        [InlineData("login", false)]
        [InlineData("misspelled", false)]
        public async Task ServiceCanGetPageUrlWithUser(string landing, bool expected)
        {
            var user = new UserBoMock(true);
            var mock = new ActiveInternetStatus();
            var service = new PermissionApi(faker.Internet.DomainName(), mock);
            var resp = await service.Get(landing, user);
            var actual = resp?.StatusCode == 200;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me")]
        [InlineData("list")]
        [InlineData("login")]
        [InlineData("application-state-configuration")]
        [InlineData("misspelled")]
        public async Task ServiceCanNotGetPageWithOutUser(string landing)
        {
            const bool expected = false;
            var user = new UserBoMock(false);
            var mock = new ActiveInternetStatus();
            var service = new PermissionApi(faker.Internet.DomainName(), mock);
            var resp = await service.Get(landing, user);
            var actual = resp?.StatusCode == 200;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", false)]
        [InlineData("list", false)]
        [InlineData("login", false)]
        [InlineData("misspelled", false)]
        public void ServiceCanNotGetPageUrlWithoutInternet(string landing, bool expected)
        {
            var mock = new InActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var actual = service.CanGet(landing).Key;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", false)]
        [InlineData("listing", false)]
        [InlineData("misspelled", false)]
        [InlineData("application-state-configuration", false)]
        [InlineData("login", true)]
        [InlineData("register", true)]
        [InlineData("refresh", true)]
        [InlineData("change-password", true)]
        [InlineData("get-contact-detail", true)]
        [InlineData("get-contact-identity", true)]
        [InlineData("edit-contact-address", true)]
        [InlineData("edit-contact-email", true)]
        [InlineData("edit-contact-name", true)]
        [InlineData("edit-contact-phone", true)]
        [InlineData("permissions-change-password", true)]
        [InlineData("permissions-set-discount", true)]
        [InlineData("permissions-set-permission", true)]
        [InlineData("message-body", true)]
        [InlineData("message-count", true)]
        [InlineData("message-list", true)]
        public void ServiceCanGetPostPageUrl(string landing, bool expected)
        {
            var mock = new ActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var user = GetUser(faker);
            var actual = service.CanPost(landing, new(), user).Key;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", false)]
        [InlineData("list", false)]
        [InlineData("application-state-configuration", false)]
        [InlineData("misspelled", false)]
        [InlineData("login", false)]
        [InlineData("register", false)]
        [InlineData("change-password", false)]
        [InlineData("get-contact-detail", false)]
        [InlineData("get-contact-identity", false)]
        [InlineData("edit-contact-address", false)]
        [InlineData("edit-contact-email", false)]
        [InlineData("edit-contact-name", false)]
        [InlineData("edit-contact-phone", false)]
        [InlineData("permissions-change-password", false)]
        [InlineData("permissions-set-discount", false)]
        [InlineData("permissions-set-permission", false)]
        public void ServiceCanNotGetPostPageWithNonInitializedUser(string landing, bool expected)
        {
            var mock = new ActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var user = GetUser(faker, false);
            var actual = service.CanPost(landing, new(), user).Key;
            Assert.Equal(expected, actual);
        }

        private static UserBo GetUser(Faker faker, bool isInitialized = true)
        {
            var user = new UserBo()
            {
                UserName = faker.Person.Email,
                Applications = new ApiContext[] { new() { Id = Guid.Empty.ToString(), Name = "next.core.tests" } }
            };
            if (isInitialized) return user;
            user.Applications = Array.Empty<ApiContext>();
            return user;
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }

        private sealed class InActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return false;
            }
        }

        private sealed class UserBoMock : UserBo
        {
            private readonly bool _authenicated;

            public UserBoMock(bool autenicationResponse)
            { _authenicated = autenicationResponse; }

            public override bool IsAuthenicated => _authenicated;
        }
    }
}