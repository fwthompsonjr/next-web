using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class BeginSearchCountyTests
    {
        private static readonly Faker<BeginSearchCounty> faker
            = new Faker<BeginSearchCounty>()
            .RuleFor(x => x.Value, y => y.Random.Int(5, 500))
            .RuleFor(x => x.Name, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new BeginSearchCounty();
            var test = faker.Generate();
            Assert.NotEqual(original.Name, test.Name);
            Assert.NotEqual(original.Value, test.Value);
        }
    }
}
