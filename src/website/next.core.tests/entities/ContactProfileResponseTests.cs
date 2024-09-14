using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactProfileResponseTests
    {
        private readonly Faker<ContactProfileResponse> faker =
            new Faker<ContactProfileResponse>()
            .RuleFor(x => x.IsOK, y => y.Random.Bool())
            .RuleFor(x => x.ResponseType, y => y.Company.CompanyName())
            .RuleFor(x => x.Data, y => y.Company.CompanyName())
            .RuleFor(x => x.Message, y => y.Company.CompanyName());

        [Fact]
        public void ContactProfileResponseCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactProfileResponse();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactProfileResponsehasDefaultValues()
        {
            var exception = Record.Exception(() =>
            {
                var id = new ContactProfileResponse();
                Assert.False(id.IsOK);
                Assert.Equal("", id.ResponseType);
                Assert.Equal("", id.Data);
                Assert.Equal("", id.Message);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactProfileResponseCanUpdateIsOK()
        {
            var items = faker.Generate(2);
            items[0].IsOK = items[1].IsOK;
            Assert.Equal(items[1].IsOK, items[0].IsOK);
        }

        [Fact]
        public void ContactProfileResponseCanUpdateResponseType()
        {
            var items = faker.Generate(2);
            items[0].ResponseType = items[1].ResponseType;
            Assert.Equal(items[1].ResponseType, items[0].ResponseType);
        }

        [Fact]
        public void ContactProfileResponseCanUpdateData()
        {
            var items = faker.Generate(2);
            items[0].Data = items[1].Data;
            Assert.Equal(items[1].Data, items[0].Data);
        }

        [Fact]
        public void ContactProfileResponseCanUpdateMessage()
        {
            var items = faker.Generate(2);
            items[0].Message = items[1].Message;
            Assert.Equal(items[1].Message, items[0].Message);
        }
    }
}