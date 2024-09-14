using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class InvoiceResponseDataTests
    {
        private static readonly Faker<InvoiceResponseData> faker
            = new Faker<InvoiceResponseData>()
            .RuleFor(x => x.LineId, y => y.Random.String(5, 500))
            .RuleFor(x => x.UserId, y => y.Random.String(5, 500))
            .RuleFor(x => x.ItemType, y => y.Random.String(5, 500))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(5, 500))
            .RuleFor(x => x.UnitPrice, y => y.Random.Int(5, 500))
            .RuleFor(x => x.Price, y => y.Random.Int(5, 500))
            .RuleFor(x => x.ReferenceId, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExternalId, y => y.Random.String(5, 500))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new InvoiceResponseData();
            var test = faker.Generate();
            Assert.NotEqual(original.LineId, test.LineId);
            Assert.NotEqual(original.UserId, test.UserId);
            Assert.NotEqual(original.ItemType, test.ItemType);
            Assert.NotEqual(original.ItemCount, test.ItemCount);
            Assert.NotEqual(original.UnitPrice, test.UnitPrice);
            Assert.NotEqual(original.Price, test.Price);
            Assert.NotEqual(original.ReferenceId, test.ReferenceId);
            Assert.NotEqual(original.ExternalId, test.ExternalId);
            Assert.NotEqual(original.PurchaseDate, test.PurchaseDate);
            Assert.NotEqual(original.IsDeleted, test.IsDeleted);
            Assert.NotEqual(original.CreateDate, test.CreateDate);
        }
    }
}