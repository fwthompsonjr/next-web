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
            Assert.True(Uri.IsWellFormedUriString(actual, UriKind.Absolute));
        }

        [Fact]
        public void CollectionContainsPostAddresses()
        {
            var actual = PostAddressProvider.PostAddresses();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData("initialize")]
        [InlineData("update")]
        [InlineData("fetch")]
        [InlineData("start")]
        [InlineData("status")]
        [InlineData("finalize")]
        public void CollectionContainsAddress(string prefix)
        {
            var list = PostAddressProvider.PostAddresses();
            var actual = list?.Find(x => x.Name == prefix);
            Assert.NotNull(actual);
            Assert.True(Uri.IsWellFormedUriString(actual.Address, UriKind.Absolute));
        }
    }
}
