using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class ContentSanitizerPaymentTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("http://www.unittests.org")]
        public void ServiceCanBeCreated(string web)
        {
            var content = Properties.Resources.sample_payment_invoice;
            var sut = new ContentSanitizerPayment();
            var tmp = sut.Sanitize(content);
            var remote = Properties.Resources.payment_confirmation_sample;
            tmp = sut.Transform(tmp, remote, web);
            Assert.False(string.IsNullOrEmpty(tmp));
        }
    }
}