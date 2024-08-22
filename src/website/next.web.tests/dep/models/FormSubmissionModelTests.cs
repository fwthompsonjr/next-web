using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class FormSubmissionModelTests
    {
        private readonly static Faker<FormSubmissionModel> faker =
            new Faker<FormSubmissionModel>()
            .RuleFor(x => x.FormName, y => y.Person.Email)
            .RuleFor(x => x.Payload, y => y.Random.AlphaNumeric(15));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate(2);
                Assert.NotEqual(sut[0].FormName, sut[1].FormName);
                Assert.NotEqual(sut[0].Payload, sut[1].Payload);
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
                    0 => new FormSubmissionModel(),
                    _ => faker.Generate()
                };
                if (testId == 1) sut.FormName = string.Empty;
                if (testId == 2) sut.Payload = string.Empty;
                Assert.False(sut.IsValid);
            });
            Assert.Null(error);
        }
    }
}