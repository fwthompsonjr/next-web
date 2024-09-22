using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MyActiveSearchDetailTests
    {
        private static readonly Faker<MyActiveSearchDetail> faker
            = new Faker<MyActiveSearchDetail>()
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.CountyName, y => y.Random.String(5, 500))
            .RuleFor(x => x.StateName, y => y.Random.String(5, 500))
            .RuleFor(x => x.StartDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.EndDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchProgress, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MyActiveSearchDetail();
            var test = faker.Generate();
            Assert.NotEqual(original.CreateDate, test.CreateDate);
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.CountyName, test.CountyName);
            Assert.NotEqual(original.StateName, test.StateName);
            Assert.NotEqual(original.StartDate, test.StartDate);
            Assert.NotEqual(original.EndDate, test.EndDate);
            Assert.NotEqual(original.SearchProgress, test.SearchProgress);
        }
    }
}