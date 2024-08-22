using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class MessageNameProviderTests
    {

        [Fact]
        public void CollectionContainsMessageSequence()
        {
            var actual = MessageNameProvider.MessageSequence();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        public void CollectionContainsName(string prefix)
        {
            var list = MessageNameProvider.MessageSequence();
            var actual = list?.Find(x => x.Name == prefix);
            Assert.NotNull(actual);
        }
    }
}