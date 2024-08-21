using Bogus;
using next.processor.api.models;
using next.processor.api.extensions;

namespace next.processor.api.tests.models
{
    public class QueueUpdateRequestTests
    {
        [Fact]
        public void QueueUpdateRequestCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueueUpdateRequest();
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueUpdateRequestCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueUpdateRequestFieldsCanGetAndSet()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(2);
                var a = collection[0];
                var b = collection[1];
                Assert.NotEqual(a.Id, b.Id);
                Assert.NotEqual(a.SearchId, b.SearchId);
                Assert.NotEqual(a.Message, b.Message);
                a.StatusId = b.StatusId;
                _ = a.IsValid();
                _ = a.ConvertFrom();
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueueUpdateRequest> faker =
            new Faker<QueueUpdateRequest>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Message, y => y.Random.AlphaNumeric(250))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2));
    }
}
