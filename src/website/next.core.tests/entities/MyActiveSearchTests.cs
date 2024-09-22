using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MyActiveSearchTests
    {
        private static readonly Faker<MyActiveSearchDetail> dfaker
            = new Faker<MyActiveSearchDetail>()
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.CountyName, y => y.Random.String(5, 500))
            .RuleFor(x => x.StateName, y => y.Random.String(5, 500))
            .RuleFor(x => x.StartDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.EndDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchProgress, y => y.Random.String(5, 500));

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
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500));

        private static readonly Faker<MyActiveSearchStaged> stagedfaker
            = new Faker<MyActiveSearchStaged>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StagingType, y => y.Random.String(5, 500));

        private static readonly Faker<MyActiveSearch> faker
            = new Faker<MyActiveSearch>()
            .RuleFor(x => x.Details, y =>
            {
                var n = y.Random.Int(2, 5);
                return dfaker.Generate(n);
            })
            .RuleFor(x => x.History, y =>
            {
                var n = y.Random.Int(2, 5);
                var nn = y.Random.Int(3, 7);
                var nnn = y.Random.Int(1, 3);
                var searches = itemfaker.Generate(n);
                var statuses = statusfaker.Generate(nn);
                var staged = stagedfaker.Generate(nnn);
                return new MyActiveSearchHistory
                {
                    Searches = searches,
                    Statuses = statuses,
                    Staged = staged
                };
            });

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MyActiveSearch();
            var test = faker.Generate();
            Assert.NotEqual(original.Details, test.Details);
            Assert.NotEqual(original.History, test.History);
        }
    }
}