using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class StatusNameProviderTests
    {

        [Fact]
        public void CollectionContainsStatusSequence()
        {
            var actual = StatusNameProvider.StatusSequence();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        public void CollectionContainsName(string prefix)
        {
            var list = StatusNameProvider.StatusSequence();
            var actual = list?.Find(x => x.Name == prefix);
            Assert.NotNull(actual);
        }
    }
}