using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class FetchIntentModelTests
    {
        private readonly Faker<FetchIntentModel> faker =
            new Faker<FetchIntentModel>()
            .RuleFor(x => x.Id, y => y.Random.Int(1, 5000000).ToString());

        [Fact]
        public void FetchIntentModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new FetchIntentModel();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void FetchIntentModelCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }
    }
}