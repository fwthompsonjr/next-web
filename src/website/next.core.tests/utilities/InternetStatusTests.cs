using next.core.interfaces;
using next.core.utilities;

namespace next.core.tests.utilities
{
    public class InternetStatusTests
    {
        private readonly IInternetStatus status = new InternetStatus();

        [Fact]
        public void SystemCanGetInternetStatus()
        {
            var exception = Record.Exception(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    _ = status.GetConnectionStatus();
                }
            });
            Assert.Null(exception);
        }
    }
}