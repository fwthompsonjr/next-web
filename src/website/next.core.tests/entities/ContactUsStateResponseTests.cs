using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactUsStateResponseTests
    {
        private readonly Faker<ContactUsStateResponse> faker =
            new Faker<ContactUsStateResponse>()
            .RuleFor(x => x.Id, y => y.Random.Int(100, 5000).ToString())
            .RuleFor(x => x.ShortName, y => y.Random.AlphaNumeric(2))
            .RuleFor(x => x.Name, y => y.Company.CompanyName())
            .RuleFor(x => x.IsActive, y => y.Random.Bool());

        [Fact]
        public void ContactUsStateResponseCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactUsStateResponse();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactUsStateResponsehasDefaultValues()
        {
            var exception = Record.Exception(() =>
            {
                var id = new ContactUsStateResponse();
                Assert.False(id.IsActive);
                Assert.Equal("", id.Id);
                Assert.Equal("", id.Name);
                Assert.Equal("", id.ShortName);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactUsStateResponseCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void ContactUsStateResponseCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ContactUsStateResponseCanUpdateShortName()
        {
            var items = faker.Generate(2);
            items[0].ShortName = items[1].ShortName;
            Assert.Equal(items[1].ShortName, items[0].ShortName);
        }

        [Fact]
        public void ContactUsStateResponseCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ContactUsCountyResponseCanGetUiControlId(int changeIndex)
        {
            var item = faker.Generate();
            if (changeIndex == 0) { item.ShortName = string.Empty; }
            var uxid = item.UiControlId;
            if (changeIndex == 0)
                Assert.True(string.IsNullOrEmpty(uxid));
            else
                Assert.False(string.IsNullOrEmpty(uxid));
        }
    }
}