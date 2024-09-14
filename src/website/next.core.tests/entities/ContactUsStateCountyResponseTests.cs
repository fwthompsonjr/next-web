using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContactUsStateCountyResponseTests
    {
        private readonly Faker<ContactUsStateCountyResponse> faker =
            new Faker<ContactUsStateCountyResponse>()
            .RuleFor(x => x.Index, y => y.Random.Int(100, 5000))
            .RuleFor(x => x.StateCode, y => y.Random.AlphaNumeric(2))
            .RuleFor(x => x.Name, y => y.Company.CompanyName())
            .RuleFor(x => x.IsActive, y => y.Random.Bool());

        [Fact]
        public void ContactUsStateCountyResponseCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContactUsStateCountyResponse();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactUsStateCountyResponsehasDefaultValues()
        {
            var exception = Record.Exception(() =>
            {
                var id = new ContactUsStateCountyResponse();
                Assert.False(id.IsActive);
                Assert.Equal(0, id.Index);
                Assert.Equal("", id.Name);
                Assert.Equal("", id.StateCode);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ContactUsStateCountyResponseCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void ContactUsStateCountyResponseCanUpdateIndex()
        {
            var items = faker.Generate(2);
            items[0].Index = items[1].Index;
            Assert.Equal(items[1].Index, items[0].Index);
        }

        [Fact]
        public void ContactUsStateCountyResponseCanUpdateStateCode()
        {
            var items = faker.Generate(2);
            items[0].StateCode = items[1].StateCode;
            Assert.Equal(items[1].StateCode, items[0].StateCode);
        }

        [Fact]
        public void ContactUsStateCountyResponseCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }

        [Fact]
        public void ContactUsStateCountyResponseCanGetIndexCode()
        {
            var exception = Record.Exception(() =>
            {
                var item = faker.Generate();
                _ = item.IndexCode;
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ContactUsStateCountyResponseCanGetUiControlId(int changeIndex)
        {
            var item = faker.Generate();
            if (changeIndex == 0) { item.Name = string.Empty; }
            if (changeIndex == 1) { item.StateCode = string.Empty; }
            var uxid = item.UiControlId;
            if (changeIndex != 2)
                Assert.True(string.IsNullOrEmpty(uxid));
            else
                Assert.False(string.IsNullOrEmpty(uxid));
        }
    }
}