using HtmlAgilityPack;
using System.Security.AccessControl;

namespace next.web.core.services
{
    internal class ContentSanitizerMailBox : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var html = base.Sanitize(content);
            var doc = GetDocument(html);
            if (doc == null) return html;
            html = RenameCorrespondenceCss(doc, html);
            html = RenameMailBoxJs(doc, html);
            html = DisplayMenuOptions(doc, html);
            return html;
        }
        private static string RenameCorrespondenceCss(HtmlDocument doc, string fallback)
        {
            const string nodeName = "correspondence-css";
            const string linkName = "<link name=\"correspondence-css\" href=\"/css/correspondence-css.css\" rel=\"stylesheet\">";
            var node = doc.DocumentNode;
            var head = node.SelectSingleNode(HtmlSelectors.HeadTag);
            var find = HtmlSelectors.GetNamedStyleTag(nodeName);
            var homenode = node.SelectSingleNode(find);
            if (head == null || homenode == null) return fallback;
            homenode.ParentNode.RemoveChild(homenode);
            var current = head.InnerHtml;
            current += (Environment.NewLine + linkName);
            head.InnerHtml = current;
            return node.OuterHtml;
        }

        private static string RenameMailBoxJs(HtmlDocument doc, string fallback)
        {
            const string nodeName = "mail-box-js";
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var find = HtmlSelectors.GetNamedSriptTag(nodeName);
            var homenode = node.SelectSingleNode(find);
            if (homenode == null) return fallback;
            homenode.InnerHtml = string.Empty;
            homenode.Attributes["name"].Value = nodeName;
            homenode.Attributes.Add("src", $"/js/{nodeName}.js");
            return node.OuterHtml;
        }
    }
}