using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class QueuePersistenceRequestTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueuePersistenceRequest();
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
                Assert.NotEqual(a.Id, b.Id);
                Assert.NotEqual(a.Content, b.Content);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueuePersistenceRequest> faker =
            new Faker<QueuePersistenceRequest>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.Content, y =>
            {
                var content = y.Lorem.Sentence(5);
                return System.Text.Encoding.UTF8.GetBytes(content);
            });
    }
}