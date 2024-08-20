using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class PostAddressProviderTests
    {
        [Fact]
        public void CollectionContainsBaseUri()
        {
            var actual = PostAddressProvider.BaseApiAddress();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        [Fact]
        public void CollectionContainsPostAddresses()
        {
            var actual = PostAddressProvider.PostAddresses();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }
    }
}
