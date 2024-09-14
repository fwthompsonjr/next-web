using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MySearchSubstitutionsTests
    {
        private static readonly Faker<MySearchSubstitutions> faker
            = new Faker<MySearchSubstitutions>()
            .RuleFor(x => x.Table, y => y.Random.String(5, 500))
            .RuleFor(x => x.Template, y => y.Random.String(5, 500))
            .RuleFor(x => x.NoDataTemplate, y => y.Random.String(5, 500))
            .RuleFor(x => x.Targets, y => y.Random.Int(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MySearchSubstitutions();
            var test = faker.Generate();
            Assert.NotEqual(original.Table, test.Table);
            Assert.NotEqual(original.Template, test.Template);
            Assert.NotEqual(original.NoDataTemplate, test.NoDataTemplate);
            Assert.NotEqual(original.Targets, test.Targets);
        }
    }
}