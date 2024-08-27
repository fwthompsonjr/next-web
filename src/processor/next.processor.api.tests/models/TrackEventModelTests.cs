using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class TrackEventModelTests
    {
        private static readonly Faker<TrackEventModel> faker
            = new Faker<TrackEventModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Content, y => y.Lorem.Paragraphs(4))
            .RuleFor(x => x.ExpirationDate, y => y.Date.Recent(120));

        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new TrackEventModel();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(error);
        }
    }
}
/*

*/