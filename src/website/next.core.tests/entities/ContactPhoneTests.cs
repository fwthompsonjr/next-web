using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactPhoneTests
    {
        private readonly Faker<ContactPhone> faker =
            new Faker<ContactPhone>()
            .RuleFor(x => x.PhoneType, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Phone, y => y.Company.CompanyName());

        [Fact]
        public void ContactPhoneCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactPhone();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactCanUpdatePhone()
        {
            var items = faker.Generate(2);
            items[0].Phone = items[1].Phone;
            Assert.Equal(items[1].Phone, items[0].Phone);
        }

        [Fact]
        public void ContactCanUpdatePhoneType()
        {
            var items = faker.Generate(2);
            items[0].PhoneType = items[1].PhoneType;
            Assert.Equal(items[1].PhoneType, items[0].PhoneType);
        }

        [Fact]
        public void ContactPhoneCanConvertToItem()
        {
            var source = faker.Generate();
            var item = source.ToItem();
            Assert.Equal("Phone", item.Category);
            Assert.Equal(source.Phone, item.Data);
            Assert.Equal(source.PhoneType, item.Code);
        }
    }
}