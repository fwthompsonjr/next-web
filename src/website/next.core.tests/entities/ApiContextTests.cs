using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ApiContextTests
    {
        private readonly Faker<ApiContext> faker =
            new Faker<ApiContext>()
            .RuleFor(x => x.Id, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        [Fact]
        public void ApiContextCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ApiContext();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ApiContextCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ApiContextCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }
    }
}