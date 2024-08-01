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
            var find = $"//div[@class='{cover}']";
            var parent = doc.DocumentNode.SelectSingleNode(find);
            if (parent == null) return content;

            var header = parent.FirstChild;
            var main = GetMain(doc);
            parent.InsertAfter(main, header);
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

        private static HtmlNode GetMain(HtmlDocument doc)
        {
            var element = doc.CreateElement("main");
            element.Attributes.Add("role", "main");
            element.Attributes.Add("class", "inner cover");
            element.InnerHtml = Properties.Resources.logout_page;
            return element;
        }
        private const string cover = "cover-container d-flex h-100 p-3 mx-auto flex-column";
        private const string coverstyle = "<link rel=\"stylesheet\" name=\"cover-css\" href=\"https://getbootstrap.com/docs/4.0/examples/cover/cover.css\" />";
    }
}
