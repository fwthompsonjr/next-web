using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;

namespace next.web.core.util
{
    internal static class ContentProvider
    {
        private static IContentHtmlNames _localContentProvider;

        public static readonly IContentHtmlNames LocalContentProvider =
            _localContentProvider ??= GetContentHtmlNames();

        private static IContentHtmlNames GetContentHtmlNames()
        {
            var provider = DesktopCoreServiceProvider.Provider;
            var obj = provider.GetService(typeof(IContentHtmlNames));
            if (obj is not IContentHtmlNames icontenthtml) return new ContentHtmlNames();
            return icontenthtml;
        }
    }
}