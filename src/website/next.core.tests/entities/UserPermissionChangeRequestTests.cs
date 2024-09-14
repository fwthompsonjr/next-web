using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class UserPermissionChangeRequestTests
    {
        private readonly Faker<UserPermissionChangeRequest> faker =
            new Faker<UserPermissionChangeRequest>()
            .RuleFor(x => x.Subscription, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.Discounts, y => y.Person.FullName)
            .RuleFor(x => x.Changes, y => y.Company.CompanyName());

        [Fact]
        public void UserPermissionChangeRequestCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserPermissionChangeRequest();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserPermissionChangeRequesthasDefaultValues()
        {
            var exception = Record.Exception(() =>
            {
                var id = new UserPermissionChangeRequest();
                Assert.Equal("", id.Subscription);
                Assert.Equal("", id.Discounts);
                Assert.Equal("", id.Changes);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserPermissionChangeRequestCanUpdateSubscription()
        {
            var items = faker.Generate(2);
            items[0].Subscription = items[1].Subscription;
            Assert.Equal(items[1].Subscription, items[0].Subscription);
        }

        [Fact]
        public void UserPermissionChangeRequestCanUpdateDiscounts()
        {
            var items = faker.Generate(2);
            items[0].Discounts = items[1].Discounts;
            Assert.Equal(items[1].Discounts, items[0].Discounts);
        }

        [Fact]
        public void UserPermissionChangeRequestCanUpdateChanges()
        {
            var items = faker.Generate(2);
            items[0].Changes = items[1].Changes;
            Assert.Equal(items[1].Changes, items[0].Changes);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public void UserPermissionChangeRequestCanGetCanSubmit(int actionId, bool expected)
        {
            var item = faker.Generate();
            if (actionId > 0) item.Subscription = string.Empty;
            if (actionId > 1) item.Discounts = string.Empty;
            if (actionId > 2) item.Changes = string.Empty;
            Assert.Equal(expected, item.CanSubmit);
        }

        [Theory]
        [InlineData(0, "Subscription")]
        [InlineData(1, "Discounts")]
        [InlineData(2, "Changes")]
        [InlineData(3, "")]
        public void UserPermissionChangeGetSubmissionName(int actionId, string expected)
        {
            var item = faker.Generate();
            switch (actionId)
            {
                case 0:
                    item.Discounts = "";
                    item.Changes = "";
                    break;

                case 1:
                    item.Subscription = "";
                    item.Changes = "";
                    break;

                case 2:
                    item.Subscription = "";
                    item.Discounts = "";
                    break;

                default:
                    item = new();
                    break;
            }
            Assert.Equal(expected, item.SubmissionName);
        }
    }
}