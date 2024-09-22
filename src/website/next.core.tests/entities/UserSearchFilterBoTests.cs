using next.core.entities;

namespace next.core.tests.entities
{
    public class UserSearchFilterBoTests
    {
        [Theory]
        [InlineData(10, "")]
        [InlineData(1, "")]
        [InlineData(2, "")]
        [InlineData(3, "")]
        [InlineData(4, "")]
        [InlineData(5, "")]
        [InlineData(0, "Harris")]
        [InlineData(0, "")]
        [InlineData(0, "None")]
        [InlineData(3, "Collin")]
        public void FilterCanGetCaption(int index, string caption)
        {
            var sut = new UserSearchFilterBo { Index = index, County = caption };
            var actual = sut.GetCaption();
            Assert.False(string.IsNullOrEmpty(actual));
        }
    }
}
