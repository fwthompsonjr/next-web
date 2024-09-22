using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MyActiveSearchStagedTests
    {
        private static readonly Faker<MyActiveSearchStaged> faker
            = new Faker<MyActiveSearchStaged>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StagingType, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MyActiveSearchStaged();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.SearchId, test.SearchId);
            Assert.NotEqual(original.LineNbr, test.LineNbr);
            Assert.NotEqual(original.CreateDate, test.CreateDate);
            Assert.NotEqual(original.StagingType, test.StagingType);
        }
    }
}