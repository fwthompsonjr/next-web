using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class CountyParameterModelTests
    {
        private static readonly Faker<CountyParameterModel> faker
            = new Faker<CountyParameterModel>()
            .RuleFor(x => x.Name, y => y.Random.String(5, 500))
            .RuleFor(x => x.Text, y => y.Random.String(5, 500))
            .RuleFor(x => x.Value, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new CountyParameterModel();
            var test = faker.Generate();
            Assert.NotEqual(original.Name, test.Name);
            Assert.NotEqual(original.Text, test.Text);
            Assert.NotEqual(original.Value, test.Value);
        }
    }
}