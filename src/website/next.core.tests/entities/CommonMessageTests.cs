using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class CommonMessageTests
    {
        private static readonly Faker<CommonMessage> faker
            = new Faker<CommonMessage>()
            .RuleFor(x => x.Id, y => y.Random.Int(5, 15))
            .RuleFor(x => x.Name, y => y.Random.String(5, 50))
            .RuleFor(x => x.Message, y => y.Random.String(5, 50))
            .RuleFor(x => x.Color, y => y.Random.String(5, 10));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new CommonMessage();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.Name, test.Name);
            Assert.NotEqual(original.Message, test.Message);
            Assert.NotEqual(original.Color, test.Color);
        }
    }
}