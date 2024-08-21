using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class QueuedRecordTests
    {
        [Fact]
        public void QueuedRecordCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueuedRecord();
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueuedRecordCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueuedRecordFieldsCanGetAndSet()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(2);
                var a = collection[0];
                var b = collection[1];
                Assert.NotEqual(a.Id, b.Id);
                Assert.NotEqual(a.Name, b.Name);
                Assert.NotEqual(a.UserId, b.UserId);
                Assert.NotEqual(a.StartDate, b.StartDate);
                Assert.NotEqual(a.EndDate, b.EndDate);
                Assert.NotEqual(a.ExpectedRows, b.ExpectedRows);
                Assert.NotEqual(a.CreateDate, b.CreateDate);
                Assert.NotEqual(a.Payload, b.Payload);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueuedRecord> faker =
            new Faker<QueuedRecord>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .RuleFor(x => x.Payload, y => y.Lorem.Sentence(2));
    }
}
