using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class FormLoginModelTests
    {
        private readonly static Faker<FormLoginModel> faker =
            new Faker<FormLoginModel>()
            .RuleFor(x => x.UserName, y => y.Person.Email)
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(15));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate(2);
                Assert.NotEqual(sut[0].UserName, sut[1].UserName);
                Assert.NotEqual(sut[0].Password, sut[1].Password);
                Assert.True(sut[0].IsValid);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ModelCanBeValidated(int testId)
        {
            var error = Record.Exception(() =>
            {
                var sut = testId switch
                {
                    0 => new FormLoginModel(),
                    _ => faker.Generate()
                };
                if (testId == 1) sut.UserName = string.Empty;
                if (testId == 2) sut.Password = string.Empty;
                Assert.False(sut.IsValid);
            });
            Assert.Null(error);
        }
    }
}