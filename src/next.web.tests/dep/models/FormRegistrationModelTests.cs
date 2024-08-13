
using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class FormRegistrationModelTests
    {
        private readonly static Faker<FormRegistrationModel> faker =
            new Faker<FormRegistrationModel>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(15))
            .FinishWith((f, a) =>
            {
                // default model has confirmation match
                a.PasswordConfirmation = a.Password;
            });

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate(2);
                Assert.NotEqual(sut[0].UserName, sut[1].UserName);
                Assert.NotEqual(sut[0].Password, sut[1].Password);
                Assert.NotEqual(sut[0].PasswordConfirmation, sut[1].PasswordConfirmation);
                Assert.True(sut[0].IsValid);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ModelCanBeValidated(int testId)
        {
            var error = Record.Exception(() =>
            {
                var sut = testId switch
                {
                    0 => new FormRegistrationModel(),
                    _ => faker.Generate()
                };
                if (testId == 1) sut.UserName = string.Empty;
                if (testId == 2) sut.Password = string.Empty;
                if (testId == 3) sut.Email = string.Empty;
                if (testId == 4) sut.PasswordConfirmation = string.Empty;
                Assert.False(sut.IsValid);
            });
            Assert.Null(error);
        }
    }
}