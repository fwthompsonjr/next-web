using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MyPurchaseBoTests
    {
        private static readonly Faker<MyPurchaseBo> faker
            = new Faker<MyPurchaseBo>()
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.ReferenceId, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExternalId, y => y.Random.String(5, 500))
            .RuleFor(x => x.ItemType, y => y.Random.String(5, 500))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(5, 500))
            .RuleFor(x => x.Price, y => y.Random.Int(5, 500))
            .RuleFor(x => x.StatusText, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MyPurchaseBo();
            var test = faker.Generate();
            Assert.NotEqual(original.PurchaseDate, test.PurchaseDate);
            Assert.NotEqual(original.ReferenceId, test.ReferenceId);
            Assert.NotEqual(original.ExternalId, test.ExternalId);
            Assert.NotEqual(original.ItemType, test.ItemType);
            Assert.NotEqual(original.ItemCount, test.ItemCount);
            Assert.NotEqual(original.Price, test.Price);
            Assert.NotEqual(original.StatusText, test.StatusText);
        }
    }
}