using Bogus;
using Bogus.DataSets;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class QueueProcessResponsesTests
    {
        [Fact]
        public void QueueProcessResponsesCanBeCreated()
        {
            var population = recordfaker.Generate(12);
            var error = Record.Exception(() =>
            {
                _ = new QueueProcessResponses(population);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void QueueProcessResponsesCanIterate(int recordCount)
        {
            var error = Record.Exception(() =>
            {
                var idx = 0;
                var collection = recordfaker.Generate(recordCount);
                if (recordCount == 10) collection.ForEach(x => x.Id = null); // test null id
                if (recordCount == 15) collection.ForEach(x => x.Id = string.Empty); // test empty id
                var sut = new QueueProcessResponses(collection);
                while (sut.IterateNext()) { idx++; }
                Assert.Equal(recordCount, idx);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueuedRecord> recordfaker =
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