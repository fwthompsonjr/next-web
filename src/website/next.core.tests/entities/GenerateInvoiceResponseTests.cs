using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class GenerateInvoiceResponseTests
    {
        private static readonly Faker<GenerateInvoiceResponse> faker
            = new Faker<GenerateInvoiceResponse>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.PaymentIntentId, y => y.Random.String(5, 500))
            .RuleFor(x => x.Description, y => y.Random.String(5, 500))
            .RuleFor(x => x.ClientSecret, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExternalId, y => y.Random.String(5, 500))
            .RuleFor(x => x.SuccessUrl, y => y.Random.String(5, 500))
            .RuleFor(x => x.Data, y => new List<InvoiceResponseData>());

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new GenerateInvoiceResponse();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.PaymentIntentId, test.PaymentIntentId);
            Assert.NotEqual(original.Description, test.Description);
            Assert.NotEqual(original.ClientSecret, test.ClientSecret);
            Assert.NotEqual(original.ExternalId, test.ExternalId);
            Assert.NotEqual(original.SuccessUrl, test.SuccessUrl);
            Assert.NotEqual(original.Data, test.Data);
        }
    }
}