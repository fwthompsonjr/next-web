using next.core.entities;
using next.core.implementations;

namespace next.core.tests.implementations
{
    public class ErrorContentProviderTests
    {
        private static readonly object locker = new();
        private static readonly ErrorContentProvider contentProvider = new();

        public ErrorContentProviderTests()
        {
            lock (locker)
            {
                _ = ErrorContentHtml.ErrorContentList();
            }
        }

        [Fact]
        public void ProviderCanBeCreated()
        {
            Assert.NotNull(contentProvider);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S2925:\"Thread.Sleep\" should not be used in tests",
            Justification = "Observing conditional pass on this test due to locking")]
        public void ProviderHasContentNames()
        {
            Exception? exception = default;
            int retries = 3;
            while (retries > 0)
            {
                exception = Record.Exception(() =>
                {
                    var actual = contentProvider.ContentNames;
                    Assert.NotEmpty(actual);
                });
                if (exception == null) break;
                Thread.Sleep(150);
                retries--;
            }
            Assert.Null(exception);
        }

        [Fact]
        public void ProviderCanGetContent()
        {
            lock (locker)
            {
                var expected = new[] { 500, 503, 424, 400, 401, 404, 409, 100, 200 };
                for (int i = 0; i < expected.Length; i++)
                {
                    int index = expected[i];
                    var valid = contentProvider.IsValid(index);
                    var actual = contentProvider.GetContent(index);
                    if (valid) Assert.NotNull(actual);
                    else Assert.Null(actual);
                }
            }
        }
    }
}