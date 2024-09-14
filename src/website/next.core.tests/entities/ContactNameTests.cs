using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactNameTests
    {
        private readonly Faker<ContactName> faker =
            new Faker<ContactName>()
            .RuleFor(x => x.NameType, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        [Fact]
        public void ContactNameCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactName();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void ContactCanUpdateNameType()
        {
            var items = faker.Generate(2);
            items[0].NameType = items[1].NameType;
            Assert.Equal(items[1].NameType, items[0].NameType);
        }

        [Fact]
        public void ContactNameCanConvertToItem()
        {
            var source = faker.Generate();
            var item = source.ToItem();
            Assert.Equal("Name", item.Category);
            Assert.Equal(source.Name, item.Data);
            Assert.Equal(source.NameType, item.Code);
        }
    }
}