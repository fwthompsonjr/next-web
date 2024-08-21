using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class QueueSearchItemTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueueSearchItem();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelHasExpectedFields()
        {
            var error = Record.Exception(() =>
            {
                var test = faker.Generate(2);
                var a = test[0];
                var b = test[1];
                Assert.NotEqual(a.WebId, b.WebId);
                Assert.NotEqual(a.State, b.State);
                Assert.NotEqual(a.County, b.County);
                Assert.NotEqual(a.StartDate, b.StartDate);
                Assert.NotEqual(a.EndDate, b.EndDate);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueueSearchItem> faker =
            new Faker<QueueSearchItem>()
            .RuleFor(x => x.WebId, y => y.Random.Int(1, 500000))
            .RuleFor(x => x.State, y => y.Random.AlphaNumeric(35))
            .RuleFor(x => x.County, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30).ToString())
            .RuleFor(x => x.EndDate, y => y.Date.Recent(90).ToString());
    }
}