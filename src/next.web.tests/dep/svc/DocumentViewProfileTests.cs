using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class DocumentViewProfileTests
    {
        [Fact]
        public void ServiceHasCorrectInheritance()
        {
            var error = Record.Exception(() =>
            {
                var sut = new DocumentViewProfile();
                Assert.IsAssignableFrom<DocumentViewProfile>(sut);
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanSetChildMenu()
        {
            var error = Record.Exception(() =>
            {
                var content = MockObjectProvider.GetContent("myaccount");
                var sut = new DocumentViewProfile();
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
                var content = MockObjectProvider.GetContent("myaccount");
                var sut = new DocumentViewProfile();
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
                var content = MockObjectProvider.GetContent("myaccount");
                var sut = new DocumentViewProfile();
                content = sut.SetTab(content);
                Assert.False(string.IsNullOrEmpty(content));
            });
            Assert.Null(error);
        }
    }
}