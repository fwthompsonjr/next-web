using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class LocalCountyProviderTests
    {

        [Fact]
        public void CollectionContainsSequence()
        {
            var actual = LocalCountyProvider.Items();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData("denton")]
        [InlineData("harris")]
        [InlineData("tarrant")]
        [InlineData("collin")]
        public void CollectionCanFindName(string prefix)
        {
            var actual = LocalCountyProvider.FindItem(prefix);
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("denton")]
        [InlineData("harris")]
        [InlineData("tarrant")]
        [InlineData("collin")]
        public void CollectionContainsName(string prefix)
        {
            var oic = StringComparison.OrdinalIgnoreCase;
            var list = LocalCountyProvider.Items();
            var actual = list.Find(x => x.Name.Equals(prefix, oic));
            Assert.NotNull(actual);
        }
    }
}