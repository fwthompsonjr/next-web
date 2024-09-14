using next.core.entities;

namespace next.core.tests.entities
{
    public class ErrorContentHtmlTests
    {
        private static readonly object locker = new();
        private static List<ErrorContentHtml>? errors;

        public ErrorContentHtmlTests()
        {
            lock (locker)
            {
                errors ??= ErrorContentHtml.ErrorContentList();
            }
        }

        [Fact]
        public void ErrorContentCanGetList()
        {
            var list = errors;
            var assertion = list != null && list.Count > 0;
            Assert.True(assertion);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2925:\"Thread.Sleep\" should not be used in tests",
            Justification = "Having issue with multi-threaded testing.")]
        public void ErrorContentContainsCode()
        {
            lock (locker)
            {
                var expected = new[] { 500, 503, 424, 400, 401, 404, 409 };
                for (int i = 0; i < expected.Length; i++)
                {
                    int statusCode = expected[i];
                    var list = errors;
                    Assert.NotNull(list);
                    var item = list.Find(x => x.StatusCode == statusCode);
                    if (item == null)
                    {
                        Thread.Sleep(200);
                        item = list.Find(x => x.StatusCode == statusCode);
                    }
                    Assert.NotNull(item);
                }
            }
        }

        [Fact]
        public void ErrorContentHasOneDefaultItem()
        {
            lock (locker)
            {
                Assert.NotNull(errors);
                var list = errors.Where(w => w.IsDefault);
                Assert.NotNull(list);
                Assert.Single(list);
            }
        }
    }
}