using next.core.entities;
using next.core.implementations;
using next.core.interfaces;

namespace next.core.tests.implementations
{
    public class ContentHtmlNamesTests
    {
        [Fact]
        public void ContentHtmlNamesCanBeCreated()
        {
            var sut = GetInstance();
            Assert.NotNull(sut);
        }

        [Fact]
        public void ContentHtmlNamesContentReplacements()
        {
            var sut = ContentHtmlNames.ContentReplacements;
            Assert.NotNull(sut);
            Assert.NotEmpty(sut);
        }

        [Theory]
        [InlineData("bootstrap.min.css")]
        [InlineData("css/error.css")]
        [InlineData("my-account-custom-css")]
        [InlineData("my-account-subcontent-css")]
        [InlineData("common-cef-handler")]
        [InlineData("<p>My Account</p>")]
        [InlineData("<p>My Password</p>")]
        [InlineData("<p>My Profile</p>")]
        [InlineData("<p>My Permissions</p>")]
        [InlineData("common-footer")]
        [InlineData("common-headings")]
        [InlineData("<p>Login form</p>")]
        [InlineData("<p>Welcome form</p>")]
        [InlineData("<p>Registration form</p>")]
        [InlineData("js-include-common-reload")]
        [InlineData("common-client-include")]
        [InlineData("home-form-validation")]
        [InlineData("my-account-navigation")]
        [InlineData("my-account-profile-valid")]
        [InlineData("inject: permissions-validation")]
        [InlineData("<p>Invoice form</p>")]
        public void ContentReplacementExistsAndIsNotEmpty(string find)
        {
            var sut = ContentHtmlNames.ContentReplacements;
            var item = sut.Find(x => x.Key.Contains(find));
            Assert.NotNull(item);
            Assert.False(string.IsNullOrWhiteSpace(item.Value));
        }

        [Fact]
        public void ContentHtmlNamesContainsNames()
        {
            var sut = GetInstance();
            Assert.NotNull(sut.Names);
            Assert.NotEmpty(sut.Names);
        }

        [Fact]
        public void ContentHtmlNamesContainsContentNames()
        {
            var sut = GetInstance();
            Assert.NotNull(sut.ContentNames);
            Assert.NotEmpty(sut.ContentNames);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ContentHtmlNamesContainsSearchUi(bool isDefault)
        {
            var sut = isDefault ? new ContentHtmlNames() : GetInstance();
            if (isDefault) Assert.Null(sut.SearchUi);
            else Assert.NotNull(sut.SearchUi);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("missing", false)]
        [InlineData("introduction", true)]
        [InlineData("Introduction", true)]
        [InlineData("home", true)]
        [InlineData("Home", true)]
        [InlineData("Invoice", true)]
        [InlineData("MyAccount", true)]
        [InlineData("MySearch", true)]
        public void ContentHtmlNamesIsValid(string test, bool expected)
        {
            var sut = GetInstance();
            var actual = sut.IsValid(test);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("missing", false)]
        [InlineData("base", true)]
        [InlineData("Base", true)]
        [InlineData("introduction", true)]
        [InlineData("Introduction", true)]
        [InlineData("home", true)]
        [InlineData("Home", true)]
        [InlineData("Blank", true)]
        [InlineData("blank", true)]
        [InlineData("myaccount", true)]
        [InlineData("MyAccount", true)]
        [InlineData("myaccounthome", true)]
        [InlineData("MyAccountHome", true)]
        [InlineData("myaccountpermissions", true)]
        [InlineData("MyAccountPermissions", true)]
        [InlineData("mysearch", true)]
        [InlineData("MySearch", true)]
        [InlineData("MySearchtemplate", true)]
        [InlineData("commondialogue", true)]
        [InlineData("commondialoguescript", true)]
        [InlineData("invoice", true)]
        [InlineData("MySearchactive", true)]
        [InlineData("mailbox", true)]
        [InlineData("viewhistory", true)]
        public void ContentHtmlNamesCanGetContent(string test, bool expected)
        {
            var sut = GetInstance();
            var actual = sut.GetContent(test);
            if (expected)
            {
                Assert.NotNull(actual);
                Assert.False(string.IsNullOrWhiteSpace(actual.Content));
            }
            else
                Assert.Null(actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("missing", false)]
        [InlineData("base", true)]
        [InlineData("Base", true)]
        [InlineData("introduction", true)]
        [InlineData("Introduction", true)]
        [InlineData("home", true)]
        [InlineData("Home", true)]
        [InlineData("test", false)]
        public void ContentHtmlNamesCanGetContentStream(string test, bool expected)
        {
            var sut = GetInstance();
            using var reader = new StreamReader(sut.GetContentStream(test));
            var content = reader.ReadToEnd();
            var actual = !string.IsNullOrEmpty(content);
            Assert.Equal(expected, actual);
        }


        private static ContentHtmlNames GetInstance()
        {
            var mock = new MockSearchBuilder();
            return new ContentHtmlNames
            {
                SearchUi = mock
            };
        }

        private static readonly string SearchContent = Properties.Resources.state_config_response;

        private class MockSearchBuilder : ISearchBuilder
        {
            public StateSearchConfiguration[]? GetConfiguration()
            {
                return Array.Empty<StateSearchConfiguration>();
            }

            public string GetHtml()
            {
                return SearchContent;
            }
        }
    }
}