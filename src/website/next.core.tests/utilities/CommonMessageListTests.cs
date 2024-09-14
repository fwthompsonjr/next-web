using next.core.utilities;

namespace next.core.tests.utilities
{
    public class CommonMessageListTests
    {
        [Fact]
        public void CommonMessageHasMessages()
        {
            var sut = new CommonMessageList();
            var mssg = sut.Messages;
            Assert.NotEmpty(mssg);
        }
    }
}
