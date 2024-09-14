using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ContentHtmlTests
    {
        private readonly Faker<ContentHtml> faker =
            new Faker<ContentHtml>()
            .RuleFor(x => x.Index, y => y.Random.Int(1, 500))
            .RuleFor(x => x.Name, y => y.Company.CompanyName())
            .RuleFor(x => x.Content, y => y.Company.CompanyName());

        [Fact]
        public void CreateContentRequestCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ContentHtml();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void CreateContentRequestCanUpdateIndex()
        {
            var items = faker.Generate(2);
            items[0].Index = items[1].Index;
            Assert.Equal(items[1].Index, items[0].Index);
        }

        [Fact]
        public void CreateContentRequestCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void CreateContentRequestCanUpdateContent()
        {
            var items = faker.Generate(2);
            items[0].Content = items[1].Content;
            Assert.Equal(items[1].Content, items[0].Content);
        }
    }
}