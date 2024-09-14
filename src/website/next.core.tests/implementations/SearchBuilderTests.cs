using next.core.entities;
using next.core.implementations;
using next.core.interfaces;
using next.core.utilities;
using Newtonsoft.Json;

namespace next.core.tests.implementations
{
    public class SearchBuilderTests
    {
        [Fact]
        public void BuilderCanBeConstructed()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchBuilder(GetApi());
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BuilderCanGetAll()
        {
            var exception = Record.Exception(() =>
            {
                var sut = new SearchBuilder(GetApi());
                for (var i = 0; i < 2; i++)
                {
                    _ = sut.GetConfiguration();
                }
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BuilderCanGetHtml()
        {
            var exception = Record.Exception(() =>
            {
                var sut = new SearchBuilder(GetApi());
                var html = sut.GetHtml();
                Assert.False(string.IsNullOrEmpty(html));
            });
            Assert.Null(exception);
        }

        [Fact]
        public void TestCanDeserialize()
        {
            var exception = Record.Exception(() =>
            {
                var content = Properties.Resources.state_config_response;
                var obj = JsonConvert.DeserializeObject<StateSearchConfiguration[]>(content);
                Assert.NotNull(obj);
                Assert.Single(obj);
            });
            Assert.Null(exception);
        }

        private static IPermissionApi GetApi()
        {
            return new MyMockApi();
        }

        private sealed class MyMockApi : PermissionApi
        {
            private static readonly IInternetStatus internetStatus = new ActiveInternetStatus();
            private readonly ApiResponse response;

            public MyMockApi() : base("www.testcase.com", internetStatus)
            {
                response = new ApiResponse
                {
                    StatusCode = 200,
                    Message = Properties.Resources.state_config_response
                };
            }

            public override async Task<ApiResponse> Get(string name)
            {
                var obj = await Task.FromResult(response);
                return obj;
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