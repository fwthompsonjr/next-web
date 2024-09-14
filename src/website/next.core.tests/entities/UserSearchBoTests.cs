using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class UserSearchBoTests
    {

        private static readonly Faker<CountyParameterModel> parmfaker
            = new Faker<CountyParameterModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.Text, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.Value, y => y.Random.AlphaNumeric(5));

        private static readonly Faker<UserSearchCounty> countyfaker
            = new Faker<UserSearchCounty>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(22))
            .RuleFor(x => x.Value, y => y.Random.Int(1, 500));

        private static readonly Faker<UserSearchBo> faker
            = new Faker<UserSearchBo>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.SessionId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Past(1).Ticks)
            .RuleFor(x => x.EndDate, y => y.Date.Future(1).Ticks)
            .RuleFor(x => x.State, y => y.Random.AlphaNumeric(2))
            .RuleFor(x => x.County, y => countyfaker.Generate())
            .RuleFor(x => x.Parameters, y =>
            {
                var n = y.Random.Int(2, 10);
                return parmfaker.Generate(n);
            }
            );

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new UserSearchBo();
            var test = faker.Generate();
            Assert.NotEqual(original.UserName, test.UserName);
            Assert.NotEqual(original.SessionId, test.SessionId);
            Assert.NotEqual(original.StartDate, test.StartDate);
            Assert.NotEqual(original.EndDate, test.EndDate);
            Assert.NotEqual(original.State, test.State);
            Assert.NotEqual(original.County, test.County);
            Assert.NotEqual(original.Parameters, test.Parameters);
            Assert.NotNull(test.SearchStarted);
            Assert.Null(test.ParameterChanged);
        }

        [Fact]
        public void ModelCanSetParameterChanged()
        {
            var test = faker.Generate();
            var parms = faker.Generate().Parameters;
            var counter = 0;
            test.ParameterChanged = () => { counter++; };
            test.Parameters = parms;
            Assert.True(counter > 0);
        }
    }
}