using HtmlAgilityPack;

namespace next.web.core.services
{
    internal class ContentSanitizerSearch : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var html = base.Sanitize(content);
            var doc = GetDocument(html);
            if (doc == null) return html;
            html = RenameSearchFormJs(doc, html);
            html = RemoveDuplicateLink(doc, "base-menu", html);
            html = DisplayMenuOptions(doc, html);
            return html;
        }

        private static string RenameSearchFormJs(HtmlDocument doc, string fallback)
        {
            const string nodeNames = "search-form-js";
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var ndes = nodeNames.Split(';').ToList();
            ndes.ForEach(nodeName =>
            {
                var find = HtmlSelectors.GetNamedSriptTag(nodeName);
                var homenode = node.SelectSingleNode(find);
                if (homenode != null)
                {
                    homenode.InnerHtml = string.Empty;
                    homenode.Attributes["name"].Value = nodeName;
                    homenode.Attributes.Add("src", $"/js/{nodeName}.js");
                }
            });
            return node.OuterHtml;
        }

        private static string RemoveDuplicateLink(HtmlDocument doc, string name, string fallback)
        {
            var node = doc.DocumentNode;
            var selector = $"//link[@name='{name}']";
            var found = node.SelectNodes(selector);
            if (found == null || found.Count <= 1) return fallback;
            found.ToList().ForEach(nodeName =>
            {
                var id = found.IndexOf(nodeName);
                if (id != 0) { nodeName.ParentNode.RemoveChild(nodeName); }
            });
            return node.OuterHtml;
        }
    }
}