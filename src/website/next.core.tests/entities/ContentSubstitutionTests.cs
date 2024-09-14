using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContentSubstitutionTests
    {
        private readonly Faker<ContentSubstitution> faker =
            new Faker<ContentSubstitution>()
            .RuleFor(x => x.Find, y => y.Company.CompanyName())
            .RuleFor(x => x.ReplaceWith, y => y.Company.CompanyName());

        [Fact]
        public void CreateContentRequestCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContentSubstitution();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void CreateContentRequestCanUpdateFind()
        {
            var items = faker.Generate(2);
            items[0].Find = items[1].Find;
            Assert.Equal(items[1].Find, items[0].Find);
        }

        [Fact]
        public void CreateContentRequestCanUpdateReplaceWith()
        {
            var items = faker.Generate(2);
            items[0].ReplaceWith = items[1].ReplaceWith;
            Assert.Equal(items[1].ReplaceWith, items[0].ReplaceWith);
        }
    }
}