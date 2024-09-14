using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class SearchPreviewModelTests
    {
        private static readonly Faker<SearchPreviewModel> faker
            = new Faker<SearchPreviewModel>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.Name, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new SearchPreviewModel();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.Name, test.Name);
        }
    }
}