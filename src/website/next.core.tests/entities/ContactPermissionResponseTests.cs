using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactPermissionResponseTests
    {
        private static readonly string[] keynames = new[] {
            "Account.Permission.Level",
            "Setting.State.Subscriptions.Active",
            "Setting.State.Subscriptions",
            "Setting.State.County.Subscriptions"
            };

        private readonly Faker<ContactPermissionResponse> faker =
            new Faker<ContactPermissionResponse>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.UserName, y => y.Person.FullName)
            .RuleFor(x => x.KeyName, y => y.PickRandom(keynames))
            .RuleFor(x => x.KeyValue, y => y.Company.CompanyName());

        [Fact]
        public void ContactPermissionResponseCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactPermissionResponse();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactPermissionResponsehasDefaultValues()
        {
            var exception = Record.Exception(() =>
            {
                var id = new ContactPermissionResponse();
                Assert.Equal("", id.Id);
                Assert.Equal("", id.UserName);
                Assert.Equal("", id.KeyName);
                Assert.Equal("", id.KeyValue);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactPermissionResponseCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ContactPermissionResponseCanUpdateUserName()
        {
            var items = faker.Generate(2);
            items[0].UserName = items[1].UserName;
            Assert.Equal(items[1].UserName, items[0].UserName);
        }

        [Fact]
        public void ContactPermissionResponseCanUpdateKeyName()
        {
            var items = faker.Generate(2);
            items[0].KeyName = items[1].KeyName;
            Assert.Equal(items[1].KeyName, items[0].KeyName);
        }

        [Fact]
        public void ContactPermissionResponseCanUpdateKeyValue()
        {
            var items = faker.Generate(2);
            items[0].KeyValue = items[1].KeyValue;
            Assert.Equal(items[1].KeyValue, items[0].KeyValue);
        }
    }
}