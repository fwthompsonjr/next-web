using Bogus;
using next.core.utilities;

namespace next.core.tests.utilities
{
    public class DownloadStatusMessagingTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(206)]
        [InlineData(400)]
        [InlineData(401)]
        [InlineData(402)]
        [InlineData(422)]
        public void SutCanGetStatusMessage(int status)
        {
            var description = new Faker().Lorem.Sentence(12);
            var message = DownloadStatusMessaging.GetMessage(status, description);
            Assert.NotEmpty(message);
        }
    }
}
