using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class FormLocationModelTests
    {
        private static readonly Faker<FormLocationModel> faker =
            new Faker<FormLocationModel>()
            .RuleFor(x => x.Id, y => y.IndexFaker)
            .RuleFor(x => x.Location, y => y.Hacker.Phrase());

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate(2);
                Assert.NotEqual(sut[0].Id, sut[1].Id);
                Assert.NotEqual(sut[0].Location, sut[1].Location);
            });
            Assert.Null(error);
        }
    }
}
