using Moq;
using next.processor.api.interfaces;
using next.processor.api.services;

namespace next.processor.api.tests.services
{
    public class BaseWebInstallTests
    {
        [Fact]
        public void ServiceIsInstalledShouldBeFalse()
        {
            var mock = new Mock<IWebInstallOperation>();
            var service = new TestingWebInstall(mock.Object);
            var actual = service.IsInstalled;
            Assert.False(actual);
        }

        [Theory]
        [InlineData("1.1.1")]
        [InlineData("129.0.2")]
        [InlineData("alphabet")]
        public void ServiceCanGetUri(string version)
        {
            var mock = new Mock<IWebInstallOperation>();
            var service = new TestingWebInstall(mock.Object);
            var address = service.GetDownload(version);
            Assert.NotNull(address);
        }

        private sealed class TestingWebInstall(IWebInstallOperation web) : BaseWebInstall(web)
        {
            public override Task<bool> InstallAsync()
            {
                throw new NotImplementedException();
            }

            public string GetDownload(string version)
            {
                return GetFireFoxDownloadUri(version);
            }
        }
    }
}
