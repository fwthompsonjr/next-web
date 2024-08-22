using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class BatchNameProviderTests
    {

        [Fact]
        public void CollectionContainsBatchSequence()
        {
            var actual = BatchNameProvider.BatchSequence();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData("fetch")]
        [InlineData("process")]
        [InlineData("complete")]
        public void CollectionContainsName(string prefix)
        {
            var list = BatchNameProvider.BatchSequence();
            var actual = list?.Find(x => x.Name == prefix);
            Assert.NotNull(actual);
        }
    }
}