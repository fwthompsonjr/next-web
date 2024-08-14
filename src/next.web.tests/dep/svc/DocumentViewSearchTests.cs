using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class DocumentViewSearchTests
    {
        [Fact]
        public void ServiceCanSetChildMenu()
        {
            var error = Record.Exception(() =>
            {
                var content = MockObjectProvider.GetContent("mysearch");
                var sut = new DocumentViewSearch();
                content = sut.SetChildMenu(content);
                Assert.False(string.IsNullOrEmpty(content));
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanSetMenu()
        {
            var error = Record.Exception(() =>
            {
                var content = MockObjectProvider.GetContent("mysearch");
                var sut = new DocumentViewSearch();
                content = sut.SetMenu(content);
                Assert.False(string.IsNullOrEmpty(content));
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanSetTab()
        {
            var error = Record.Exception(() =>
            {
                var content = MockObjectProvider.GetContent("mysearch");
                var sut = new DocumentViewSearch();
                content = sut.SetTab(content);
                Assert.False(string.IsNullOrEmpty(content));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ServiceChildCanSetTab(int childId)
        {
            var error = Record.Exception(() =>
            {
                var content = MockObjectProvider.GetContent("mysearch");
                var sut = childId switch
                {
                    0 => new DocumentViewSearchActive(),
                    1 => new DocumentViewSearchPurchases(),
                    _ => new DocumentViewSearch()
                };
                content = sut.SetTab(content);
                Assert.False(string.IsNullOrEmpty(content));
            });
            Assert.Null(error);
        }
    }
}