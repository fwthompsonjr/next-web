using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactEmailTests
    {
        private readonly Faker<ContactEmail> faker =
            new Faker<ContactEmail>()
            .RuleFor(x => x.EmailType, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Email, y => y.Company.CompanyName());

        [Fact]
        public void ContactEmailCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactEmail();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactCanUpdateEmail()
        {
            var items = faker.Generate(2);
            items[0].Email = items[1].Email;
            Assert.Equal(items[1].Email, items[0].Email);
        }

        [Fact]
        public void ContactCanUpdateEmailType()
        {
            var items = faker.Generate(2);
            items[0].EmailType = items[1].EmailType;
            Assert.Equal(items[1].EmailType, items[0].EmailType);
        }

        [Fact]
        public void ContactEmailCanConvertToItem()
        {
            var source = faker.Generate();
            var item = source.ToItem();
            Assert.Equal("Email", item.Category);
            Assert.Equal(source.Email, item.Data);
            Assert.Equal(source.EmailType, item.Code);
        }
    }
}