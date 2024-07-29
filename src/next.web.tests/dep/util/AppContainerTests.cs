using legallead.desktop.interfaces;
using next.web.core.util;
using legallead.desktop.utilities;

namespace next.web.tests.dep.util
{
    public class AppContainerTests
    {
        [Fact]
        public void ContainerCanBuild()
        {
            var error = Record.Exception(() =>
            {
                AppContainer.Build();
                Assert.NotNull(AppContainer.ServiceProvider);
                Assert.False(string.IsNullOrWhiteSpace(AppContainer.PaymentSessionKey));
                Assert.False(string.IsNullOrWhiteSpace(AppContainer.PermissionApiBase));
                Assert.False(string.IsNullOrWhiteSpace(AppContainer.InitialViewName));
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(typeof(IPermissionApi))]
        [InlineData(typeof(ISearchBuilder))]
        [InlineData(typeof(IContentParser))]
        [InlineData(typeof(IErrorContentProvider))]
        [InlineData(typeof(IUserProfileMapper))]
        [InlineData(typeof(IUserPermissionsMapper))]
        [InlineData(typeof(ICopyrightBuilder))]
        [InlineData(typeof(IQueueStopper))]
        [InlineData(typeof(IQueueStarter))]
        [InlineData(typeof(IMailPersistence))]
        [InlineData(typeof(IMailReader))]
        [InlineData(typeof(IHistoryReader))]
        [InlineData(typeof(IUserMailboxMapper))]
        [InlineData(typeof(CommonMessageList))]
        [InlineData(typeof(IHistoryPersistence))]
        public void ContainerCanProvideService(Type serviceType)
        {
            var error = Record.Exception(() =>
            {
                AppContainer.Build();
                var actual = AppContainer.ServiceProvider?.GetService(serviceType);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}
