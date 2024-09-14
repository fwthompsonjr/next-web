using HtmlAgilityPack;
using System.Diagnostics.CodeAnalysis;

namespace next.core.utilities
{
    internal static class SearchPageContentHelper
    {
        [ExcludeFromCodeCoverage(Justification = "Tested in integration only")]
        public static string Transform(string raw, string target)
        {
            const string search_parent = "//*[@id='dv-subcontent-search']";
            const string actv = "active";
            const string dnone = "d-none";
            var menus = (new[] { "history", "purchases" }).ToList();
            var links = new[]
            {
                "//*[@id='nvlink-subcontent-search']",
                "//*[@id='nvlink-subcontent-search-history']",
                "//*[@id='nvlink-subcontent-search-purchases']",
            }.ToList();
            var views = new[]
            {
                "//*[@id='dv-search-container']",
                "//*[@id='dv-subcontent-history']",
                "//*[@id='dv-subcontent-purchases']",
            }.ToList();
            if (!menus.Contains(target)) { return raw; }
            var targetId = menus.IndexOf(target) + 1;
            var document = new HtmlDocument();
            document.LoadHtml(raw);
            var docNode = document.DocumentNode;
            links.ForEach(link =>
            {
                var id = links.IndexOf(link);
                var linkNode = docNode.SelectSingleNode(links[id]);
                RemoveClass(linkNode, actv);
                if (id == 0)
                {
                    var search = docNode.SelectSingleNode(search_parent);
                    SetDisplay(search, "display: none");
                }
                if (id != 0 && id == targetId)
                {
                    AppendClass(linkNode, actv);
                }
            });
            views.ForEach(vw =>
            {
                var id = views.IndexOf(vw);
                var linkNode = docNode.SelectSingleNode(vw);
                RemoveClass(linkNode, actv);
                if (id == targetId)
                {
                    RemoveClass(linkNode, dnone);
                    AppendClass(linkNode, actv);
                }
            });
            return docNode.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void AppendClass(HtmlNode? linkNode, string className)
        {
            if (linkNode == null) return;
            var attr = linkNode.Attributes["class"];
            var items = attr.Value.Split(' ').ToList();
            items.Add(className);
            items = items.Distinct().ToList();
            var cls = string.Join(" ", items);
            attr.Value = cls;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void RemoveClass(HtmlNode? linkNode, string className)
        {
            if (linkNode == null) return;
            var attr = linkNode.Attributes["class"];
            attr.Value = attr.Value.Replace(className, string.Empty).Trim();
        }
        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void SetDisplay(HtmlNode? node, string cssName)
        {
            if (node == null) return;
            var attr = node.Attributes["style"];
            if (attr == null)
            {
                var dc = node.OwnerDocument;
                attr = dc.CreateAttribute("style", cssName);
                node.Attributes.Add(attr);
                return;
            }
            attr.Value = cssName;
        }
    }
}
