using next.core.entities;
using next.core.interfaces;
using next.core.utilities;

namespace next.core.tests.utilities
{
    public class DesktopCoreServiceProviderTests
    {
        [Fact]
        public void CanGetDesktopServiceProvider()
        {
            var exception = Record.Exception(() =>
            {
                _ = DesktopCoreServiceProvider.Provider;
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(typeof(IContentParser))]
        [InlineData(typeof(IContentHtmlNames))]
        [InlineData(typeof(IInternetStatus))]
        [InlineData(typeof(MenuConfiguration))]
        [InlineData(typeof(IErrorContentProvider))]
        [InlineData(typeof(IUserProfileMapper))]
        [InlineData(typeof(IUserPermissionsMapper))]
        [InlineData(typeof(ICopyrightBuilder))]
        [InlineData(typeof(IQueueSettings))]
        [InlineData(typeof(IQueueStarter))]
        [InlineData(typeof(IQueueStopper))]
        [InlineData(typeof(IQueueFilter))]
        [InlineData(typeof(IMailPersistence))]
        [InlineData(typeof(IMailReader))]
        [InlineData(typeof(IHistoryReader))]
        public void CanGetRegisteredType(Type type)
        {
            // DesktopCoreServiceProvider.
            var exception = Record.Exception(() =>
            {
                var provider = DesktopCoreServiceProvider.Provider;
                var sut = provider?.GetService(type);
                Assert.NotNull(sut);
            });
            Assert.Null(exception);
        }
    }
}