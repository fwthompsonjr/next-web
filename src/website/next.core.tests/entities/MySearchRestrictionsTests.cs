using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MySearchRestrictionsTests
    {
        private static readonly Faker<MySearchRestrictions> faker
            = new Faker<MySearchRestrictions>()
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.Reason, y => y.Random.String(5, 500))
            .RuleFor(x => x.MaxPerMonth, y => y.Random.Int(5, 500))
            .RuleFor(x => x.MaxPerYear, y => y.Random.Int(5, 500))
            .RuleFor(x => x.ThisMonth, y => y.Random.Int(5, 500))
            .RuleFor(x => x.ThisYear, y => y.Random.Int(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MySearchRestrictions();
            var test = faker.Generate();
            Assert.NotEqual(original.IsLocked, test.IsLocked);
            Assert.NotEqual(original.Reason, test.Reason);
            Assert.NotEqual(original.MaxPerMonth, test.MaxPerMonth);
            Assert.NotEqual(original.MaxPerYear, test.MaxPerYear);
            Assert.NotEqual(original.ThisMonth, test.ThisMonth);
            Assert.NotEqual(original.ThisYear, test.ThisYear);
        }
    }
}