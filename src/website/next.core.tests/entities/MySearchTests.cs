using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MySearchTests
    {
        private static readonly Faker<MySearchDetail> dfaker
            = new Faker<MySearchDetail>()
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.CountyName, y => y.Random.String(5, 500))
            .RuleFor(x => x.StateName, y => y.Random.String(5, 500))
            .RuleFor(x => x.StartDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.EndDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchProgress, y => y.Random.String(5, 500));

        private static readonly Faker<MySearchItem> itemfaker
            = new Faker<MySearchItem>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.EndDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StartDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(5, 500));

        private static readonly Faker<MySearchStatus> statusfaker
            = new Faker<MySearchStatus>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.Line, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500));

        private static readonly Faker<MySearchStaged> stagedfaker
            = new Faker<MySearchStaged>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StagingType, y => y.Random.String(5, 500));

        private static readonly Faker<MySearch> faker
            = new Faker<MySearch>()
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
                return new MySearchHistory
                {
                    Searches = searches,
                    Statuses = statuses,
                    Staged = staged
                };
            });

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MySearch();
            var test = faker.Generate();
            Assert.NotEqual(original.Details, test.Details);
            Assert.NotEqual(original.History, test.History);
        }
    }
}