using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactIdentityTests
    {
        private readonly Faker<ContactIdentity> faker =
            new Faker<ContactIdentity>()
            .RuleFor(x => x.UserName, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Email, y => y.Company.CompanyName())
            .RuleFor(x => x.Created, y => y.Company.CompanyName())
            .RuleFor(x => x.Role, y => y.Company.CompanyName())
            .RuleFor(x => x.RoleDescription, y => y.Company.CompanyName());

        [Fact]
        public void ContactIdentityCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactIdentity();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactIdentityhasDefaultValues()
        {
            const string notFound = " - n/a -";
            var exception = Record.Exception(() =>
            {
                var id = new ContactIdentity();
                Assert.Equal(notFound, id.UserName);
                Assert.Equal(notFound, id.Email);
                Assert.Equal(notFound, id.Created);
                Assert.Equal("Guest", id.Role);
                Assert.Equal("", id.RoleDescription);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactIdentityCanUpdateUserName()
        {
            var items = faker.Generate(2);
            items[0].UserName = items[1].UserName;
            Assert.Equal(items[1].UserName, items[0].UserName);
        }

        [Fact]
        public void ContactIdentityCanUpdateEmail()
        {
            var items = faker.Generate(2);
            items[0].Email = items[1].Email;
            Assert.Equal(items[1].Email, items[0].Email);
        }

        [Fact]
        public void ContactIdentityCanUpdateCreated()
        {
            var items = faker.Generate(2);
            items[0].Created = items[1].Created;
            Assert.Equal(items[1].Created, items[0].Created);
        }

        [Fact]
        public void ContactIdentityCanUpdateRole()
        {
            var items = faker.Generate(2);
            items[0].Role = items[1].Role;
            Assert.Equal(items[1].Role, items[0].Role);
        }

        [Fact]
        public void ContactIdentityCanUpdateRoleDescription()
        {
            var items = faker.Generate(2);
            items[0].RoleDescription = items[1].RoleDescription;
            Assert.Equal(items[1].RoleDescription, items[0].RoleDescription);
        }
    }
}