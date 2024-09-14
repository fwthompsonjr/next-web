using next.core.entities;
using next.core.implementations;
using next.core.interfaces;
using next.core.utilities;

namespace next.core.tests.implementations
{
    public class MailReaderTests
    {
        [Theory]
        [InlineData(true, "", 200)]
        [InlineData(false, "", 200)]
        [InlineData(true, "bc263a41-ba01-463e", 200)]
        [InlineData(true, "bc263a41-ba01-463e-aead-95609c958deb", 200)]
        [InlineData(true, "bc263a41-ba01-463e-aead-95609c958deb", 404)]
        public void ReaderCanGetBody(bool isAuthenticated, string messageId, int statusCode)
        {
            var problems = Record.Exception(() =>
            {
                var user = new TestUserBo(isAuthenticated);
                var reader = new MailReader();
                var response = new ApiResponse { Message = "", StatusCode = statusCode };
                var api = new PositiveRespondingApi(response);
                _ = reader.GetBody(api, user, messageId);
            });
            Assert.Null(problems);
        }

        [Theory]
        [InlineData(true, 200)]
        [InlineData(false, 200)]
        [InlineData(true, 404)]
        public void ReaderCanGetCount(bool isAuthenticated, int statusCode)
        {
            var problems = Record.Exception(() =>
            {
                var user = new TestUserBo(isAuthenticated);
                var reader = new MailReader();
                var response = new ApiResponse { Message = "", StatusCode = statusCode };
                var api = new PositiveRespondingApi(response);
                _ = reader.GetCount(api, user);
            });
            Assert.Null(problems);
        }

        [Theory]
        [InlineData(true, 200)]
        [InlineData(false, 200)]
        [InlineData(true, 404)]
        public void ReaderCanGetMessages(bool isAuthenticated, int statusCode)
        {
            var problems = Record.Exception(() =>
            {
                var user = new TestUserBo(isAuthenticated);
                var reader = new MailReader();
                var response = new ApiResponse { Message = "", StatusCode = statusCode };
                var api = new PositiveRespondingApi(response);
                _ = reader.GetMessages(api, user);
            });
            Assert.Null(problems);
        }

        private sealed class TestUserBo : UserBo
        {
            private readonly bool isAuthenticated;
            public TestUserBo(bool authenticated)
            {
                isAuthenticated = authenticated;
            }
            public override bool IsAuthenicated => isAuthenticated;
        }
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

            public PositiveRespondingApi(ApiResponse response) : base("http://test.api", new ActiveInternetStatus())
            {
                _message = response;
            }

            public override async Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                var response = await Task.Run(() => { return _message; });
                return response;
            }
        }
    }
}
