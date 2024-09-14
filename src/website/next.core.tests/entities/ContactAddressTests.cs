using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactAddressTests
    {
        private readonly Faker<ContactAddress> faker =
            new Faker<ContactAddress>()
            .RuleFor(x => x.AddressType, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Address, y => y.Company.CompanyName());

        [Fact]
        public void ContactAddressCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactAddress();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactCanUpdateAddress()
        {
            var items = faker.Generate(2);
            items[0].Address = items[1].Address;
            Assert.Equal(items[1].Address, items[0].Address);
        }

        [Fact]
        public void ContactCanUpdateAddressType()
        {
            var items = faker.Generate(2);
            items[0].AddressType = items[1].AddressType;
            Assert.Equal(items[1].AddressType, items[0].AddressType);
        }

        [Fact]
        public void ContactAddressCanConvertToItem()
        {
            var source = faker.Generate();
            var item = source.ToItem();
            Assert.Equal("Address", item.Category);
            Assert.Equal(source.Address, item.Data);
            Assert.Equal(source.AddressType, item.Code);
        }
    }
}