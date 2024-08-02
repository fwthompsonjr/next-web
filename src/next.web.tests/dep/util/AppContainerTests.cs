using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using next.web.core.interfaces;
using next.web.core.util;

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
                Assert.False(string.IsNullOrWhiteSpace(AppContainer.PostLoginPage));
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
        [Theory]
        [InlineData("home")]
        [InlineData("blank")]
        [InlineData("introduction")]
        [InlineData("myaccount")]
        [InlineData("mysearch")]
        [InlineData("mailbox")]
        [InlineData("default")]
        [InlineData("post-login")]
        [InlineData("not-mapped")]
        public void ContainerCanGetSanitizer(string name)
        {
            var error = Record.Exception(() =>
            {
                AppContainer.Build();
                var actual = AppContainer.GetSanitizer(name);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("form-login", true)]
        [InlineData("permissions-subscription-group", true)]
        [InlineData("blank")]
        [InlineData("not-mapped")]
        public void ContainerCanGetJsHandler(string name, bool expected = false)
        {
            var error = Record.Exception(() =>
            {
                AppContainer.Build();
                var actual = AppContainer.ServiceProvider?.GetKeyedService<IJsHandler>(name);
                var isFound = actual != null;
                Assert.Equal(expected, isFound);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("account-home", true)]
        [InlineData("account-profile", true)]
        [InlineData("account-permissions", true)]
        [InlineData("blank")]
        [InlineData("not-mapped")]
        public void ContainerCanGetDocumentView(string name, bool expected = false)
        {
            var error = Record.Exception(() =>
            {
                AppContainer.Build();
                var actual = AppContainer.GetDocumentView(name);
                var isFound = actual != null;
                Assert.Equal(expected, isFound);
            });
            Assert.Null(error);
        }
    }
}
