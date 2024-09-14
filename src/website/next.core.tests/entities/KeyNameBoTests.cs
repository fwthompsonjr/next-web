using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class KeyNameBoTests
    {
        private static readonly Faker<KeyNameBo> faker
            = new Faker<KeyNameBo>()
            .RuleFor(x => x.Key, y => y.Random.String(5, 500))
            .RuleFor(x => x.Value, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new KeyNameBo();
            var test = faker.Generate();
            Assert.NotEqual(original.Key, test.Key);
            Assert.NotEqual(original.Value, test.Value);
        }
    }
}