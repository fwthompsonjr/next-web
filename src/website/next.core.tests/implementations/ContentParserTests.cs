using Bogus;
using next.core.implementations;
using next.core.interfaces;

namespace next.core.tests.implementations
{
    public class ContentParserTests
    {
        private static readonly Faker faker = new();
        private readonly IContentParser parser;

        public ContentParserTests()
        {
            parser = new ContentParser();
        }

        [Fact]
        public void ParserCanParse()
        {
            var test = faker.PickRandom(ContentSamples);
            var actual = parser.BeautfyHTML(test);
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.NotEqual(test, actual);
        }

        private static readonly List<string> ContentSamples = new() {
            "<html></html>",
            "<html><body></body></html>",
            "<html><body><div><p>This is a sentence.</div></body></html>"
        };
    }
}