using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class FormStatusFilterTests
    {
        private readonly static Faker<FormStatusFilter> faker =
            new Faker<FormStatusFilter>()
            .RuleFor(x => x.StatusId, y => y.Random.Int(1, 5000))
            .RuleFor(x => x.CountyName, y => y.Person.Email)
            .RuleFor(x => x.Heading, y => y.Random.AlphaNumeric(15));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate(2);
                Assert.NotEqual(sut[0].StatusId, sut[1].StatusId);
                Assert.NotEqual(sut[0].CountyName, sut[1].CountyName);
                Assert.NotEqual(sut[0].Heading, sut[1].Heading);
            });
            Assert.Null(error);
        }
    }
}