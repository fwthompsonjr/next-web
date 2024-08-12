using HtmlAgilityPack;
using next.web.core.extensions;
using System.Text;

namespace next.web.core.services
{
    internal class ContentSanitizerCache : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(CacheContent);
            if (doc == null) return content;
            return doc.DocumentNode.OuterHtml;
        }
        private static string? cacheContent;
        private static string GetCacheContent => Properties.Resources.user_cache;
        private static string CacheContent => cacheContent ??= GetCacheContent;
    }
}