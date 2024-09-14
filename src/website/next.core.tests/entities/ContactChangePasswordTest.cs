using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactChangePasswordTests
    {
        private static readonly Faker<ContactChangePassword> faker
            = new Faker<ContactChangePassword>()
            .RuleFor(x => x.UserName, y => y.Random.String(5, 500))
            .RuleFor(x => x.OldPassword, y => y.Random.String(5, 500))
            .RuleFor(x => x.NewPassword, y => y.Random.String(5, 500))
            .RuleFor(x => x.ConfirmPassword, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new ContactChangePassword();
            var test = faker.Generate();
            Assert.NotEqual(original.UserName, test.UserName);
            Assert.NotEqual(original.OldPassword, test.OldPassword);
            Assert.NotEqual(original.NewPassword, test.NewPassword);
            Assert.NotEqual(original.ConfirmPassword, test.ConfirmPassword);
        }
    }
}