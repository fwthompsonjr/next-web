using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MyActiveSearchHistoryTests
    {
        private static readonly Faker<MyActiveSearchStaged> stagedfaker
            = new Faker<MyActiveSearchStaged>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StagingType, y => y.Random.String(5, 500));

        private static readonly Faker<MyActiveSearchItem> itemfaker
            = new Faker<MyActiveSearchItem>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.EndDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StartDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(5, 500));

        private static readonly Faker<MyActiveSearchStatus> statusfaker
            = new Faker<MyActiveSearchStatus>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.Line, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500000))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500));

        private static readonly Faker<MyActiveSearchHistory> faker
            = new Faker<MyActiveSearchHistory>()
            .RuleFor(x => x.Searches, y => itemfaker.Generate(y.Random.Int(1, 4)))
            .RuleFor(x => x.Statuses, y => statusfaker.Generate(y.Random.Int(1, 4)))
            .RuleFor(x => x.Staged, y => stagedfaker.Generate(y.Random.Int(1, 4)));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MyActiveSearchHistory();
            var test = faker.Generate();
            Assert.NotEqual(original.Searches, test.Searches);
            Assert.NotEqual(original.Statuses, test.Statuses);
            Assert.NotEqual(original.Staged, test.Staged);
        }
    }
}