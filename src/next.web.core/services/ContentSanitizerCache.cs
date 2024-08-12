using HtmlAgilityPack;
using next.web.core.extensions;
using System.Text;
using System.Windows.Markup;

namespace next.web.core.services
{
    internal class ContentSanitizerCache : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(CacheContent);
            if (doc == null) return content;
            AppendMenu(doc);
            UnhideMenuOptions(doc);
            return doc.DocumentNode.OuterHtml;
        }

        private static void UnhideMenuOptions(HtmlDocument doc)
        {
            const char space = ' ';
            const string dnone = "d-none";
            const string cls = "class";
            const string findnames = "//div[@name='left-menu-account']";
            var node = doc.DocumentNode;
            var dvs = node.SelectNodes(findnames).ToList();
            dvs.ForEach(d =>
            {
                var attr = d.Attributes.FirstOrDefault(x => x.Name == cls);
                if (attr != null)
                {
                    var clss = attr.Value.Split(space).ToList();
                    clss.Remove(dnone);
                    attr.Value = string.Join(" ", clss);
                }
            });
        }

        private static void AppendMenu(HtmlDocument doc)
        {
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode("//body");
            if(body == null) return;
            var txt = string.Concat(
                TheMenu,
                Environment.NewLine,
                body.InnerHtml);
            body.InnerHtml = txt;
        }

        private static string? cacheContent;
        private static string GetCacheContent => Properties.Resources.user_cache;
        private static string CacheContent => cacheContent ??= GetCacheContent;

        private static string? bseMenu;
        private static string GetTheMenu => Properties.Resources.base_menu;
        private static string TheMenu => bseMenu ??= GetTheMenu;
    }
}