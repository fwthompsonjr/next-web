using HtmlAgilityPack;
using System.Text;

namespace next.web.core.services
{
    internal class ContentSanitizerLogout : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            content = AppendMain(content, doc);
            content = AppendStyleTag(content, doc);
            var html = RemoveMenuOptions(doc, content);
            return html;
        }

        private static string AppendMain(string content, HtmlDocument doc)
        {
            const string closeHeader = "</header>";
            var find = $"//div[@class='{cover}']";
            var parent = doc.DocumentNode.SelectSingleNode(find);
            if (parent == null) return content;
            var current = parent.InnerHtml;
            var injected = string.Concat(
                closeHeader, 
                Environment.NewLine, 
                Properties.Resources.logout_page);
            current = current.Replace(closeHeader, injected);
            parent.InnerHtml = current;
            return doc.DocumentNode.OuterHtml;
        }

        private static string AppendStyleTag(string content, HtmlDocument doc)
        {
            var link = $"//link[@name='cover-css']";
            var style = doc.DocumentNode.SelectSingleNode(link);
            if (style != null) return content;
            var head = doc.DocumentNode.SelectSingleNode("//head");
            var txt = new StringBuilder(head.InnerHtml);
            txt.AppendLine();
            txt.AppendLine(coverstyle);
            head.InnerHtml = txt.ToString();
            return doc.DocumentNode.OuterHtml;
        }
        private const string cover = "cover-container d-flex h-100 p-3 mx-auto flex-column";
        private const string coverstyle = "<link rel=\"stylesheet\" name=\"cover-css\" href=\"https://getbootstrap.com/docs/4.0/examples/cover/cover.css\" />";
    }
}
