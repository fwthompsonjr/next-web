using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class ItemNameProviderTests
    {

        [Fact]
        public void CollectionContainsItemSequence()
        {
            var actual = ItemNameProvider.ItemSequence();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData("start")]
        [InlineData("get_parameter")]
        [InlineData("convert_parameter")]
        [InlineData("execute_search")]
        [InlineData("translate_excel")]
        [InlineData("serialize")]
        public void CollectionContainsName(string prefix)
        {
            var list = ItemNameProvider.ItemSequence();
            var actual = list?.Find(x => x.Name == prefix);
            Assert.NotNull(actual);
        }
    }
}