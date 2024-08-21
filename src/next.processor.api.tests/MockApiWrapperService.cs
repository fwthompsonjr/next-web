using Moq;
using next.processor.api.interfaces;
using next.processor.api.services;

namespace next.processor.api.tests
{
    internal class MockApiWrapperService : ApiWrapperService
    {
        public Mock<IHttpClientWrapper> MockClient { get; set; } = new();
        public override IHttpClientWrapper GetClientWrapper(HttpClient client)
        {
            return MockClient.Object;
        }
        public IHttpClientWrapper GetFakeWrapper()
        {
            var mock = new Mock<HttpClient>();
            return base.GetClientWrapper(mock.Object);
        }
    }
}
