using Moq;
using next.web.core.services;
using legallead.desktop.interfaces;
using legallead.desktop.entities;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerInvoiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var content = Properties.Resources.sample_payment_invoice;
            var sut = new ContentSanitizerInvoice();
            var tmp = sut.Sanitize(content);
            Assert.False(string.IsNullOrEmpty(tmp));
        }

        [Theory]
        [InlineData("")]
        [InlineData("http://www.unittests.org")]
        public async Task ServiceCanGetContent(string web)
        {
            var mock = MockUserSession.GetInstance();
            var api = new ApiMock();
            var content = Properties.Resources.sample_payment_invoice;
            var sut = new InvoiceMock();
            var tmp = sut.Sanitize(content);
            tmp = await sut.GetContent(mock.MqSession.Object, api, tmp, web);
            Assert.False(string.IsNullOrEmpty(tmp));
        }

        private sealed class ApiMock : IPermissionApi
        {
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

            public Task<ApiResponse> Get(string name, UserBo user)
            {
                throw new NotImplementedException();
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

            public Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class InvoiceMock : ContentSanitizerInvoice
        {
            protected override Task<string> GetInvoiceHtml(string navigateTo)
            {
                var content = Properties.Resources.sample_payment_invoice;
                var sut = new ContentSanitizerPayment();
                var tmp = sut.Sanitize(content);
                var remote = Properties.Resources.payment_confirmation_sample;
                tmp = sut.Transform(tmp, remote);
                return Task.FromResult(tmp);
            }
        }

    }
}