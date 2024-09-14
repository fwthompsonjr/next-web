using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class UserSearchQueryBoTests
    {

        private static readonly Faker<UserSearchQueryBo> faker
            = new Faker<UserSearchQueryBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Person.UserName)
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Past(1))
            .RuleFor(x => x.EndDate, y => y.Date.Future(1))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.EstimatedRowCount, y => y.Random.Int(2, 25))
            .RuleFor(x => x.SearchProgress, y => y.Hacker.Phrase().Replace(' ', '-'))
            .RuleFor(x => x.StateCode, y => y.Random.AlphaNumeric(22))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(22));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new UserSearchQueryBo();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.Name, test.Name);
            Assert.NotEqual(original.UserId, test.UserId);
            Assert.NotEqual(original.StartDate, test.StartDate);
            Assert.NotEqual(original.EndDate, test.EndDate);
            Assert.NotEqual(original.CreateDate, test.CreateDate);
            Assert.NotEqual(original.EstimatedRowCount, test.EstimatedRowCount);
            Assert.NotEqual(original.StateCode, test.StateCode);
            Assert.NotEqual(original.CountyName, test.CountyName);
            Assert.NotEqual(original.SearchProgress, test.SearchProgress);
        }

        [Theory]
        [InlineData(-1, true)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, false)]
        [InlineData(7, true)]
        [InlineData(0, true, false)]
        [InlineData(1, false, false)]
        [InlineData(2, false, false)]
        [InlineData(3, false, false)]
        [InlineData(4, false, false)]
        [InlineData(5, false, false)]
        [InlineData(6, false, false)]
        [InlineData(6, false, false, false)]
        public void ModelCanGetFromIndexer(int indx, bool isEmpty, bool hasObject = true, bool statusHasDash = true)
        {
            var test = hasObject ? faker.Generate() : new();
            if (!statusHasDash) test.SearchProgress = "abcdefg";
            var actual = test[indx];
            Assert.Equal(isEmpty, string.IsNullOrEmpty(actual));
        }
    }
}