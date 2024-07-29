﻿using legallead.desktop.interfaces;
using legallead.desktop.utilities;
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
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(typeof(IPermissionApi))]
        [InlineData(typeof(IAuthorizedUserService))]
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
    }
}
