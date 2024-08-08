using HtmlAgilityPack;
using next.web.core.extensions;
using System.Text;

namespace next.web.core.services
{
    internal class ContentSanitizerDownload : ContentSanitizerBase
    {
        public static string AppendContext(string content, List<KeyValuePair<string, string>> context)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            var node = doc.DocumentNode;
            context.ForEach(item =>
            {
                var element = node.SelectSingleNode(item.Key);
                if (element != null) element.InnerHtml = item.Value;
            });
            return node.OuterHtml;
        }
        public override string Sanitize(string content)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            content = AppendMain(content, doc);
            content = AppendStyleInvoice(content, doc);
            content = AppendStyleTag(content, doc);
            content = DisplayParentMenus(content);
            content = ApplyYearToFooter(content);
            return content;
        }

        protected static string ApplyYearToFooter(string content)
        {
            const string findfooter = "//footer[@class='mastfoot mt-auto']";
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var footer = node.SelectSingleNode(findfooter);
            if (footer == null) return content;
            var dv = footer.SelectSingleNode("div[@class='inner']");
            if (dv == null) return content;
            var p = dv.SelectSingleNode("p");
            if (p == null) return content;
            var text = p.InnerHtml.Split('-')[^1].Trim();
            var dte = DateTime.UtcNow.Year.ToString();
            p.InnerHtml = $"&copy; {dte} - {text}";
            return node.OuterHtml;
        }

        protected static string DisplayParentMenus(string content)
        {
            const string cls = "class";
            const string positions = "10,20,30";
            const string findmenu = "//div[@data-position-index='~0']";
            var doc = content.ToHtml();
            var indexes = positions.Split(',').ToList();
            var node = doc.DocumentNode;
            indexes.ForEach(index =>
            {
                var finder = findmenu.Replace("~0", index);
                var menu = node.SelectSingleNode(finder);
                if (menu != null)
                {
                    menu.Attributes[cls].Value = "row";
                }
            });

            return node.OuterHtml;
        }

        protected static string AppendMain(string content, HtmlDocument doc)
        {
            const string closeHeader = "</header>";
            var find = $"//div[@class='{cover}']";
            var parent = doc.DocumentNode.SelectSingleNode(find);
            if (parent == null) return content;
            var current = parent.InnerHtml;
            var injected = string.Concat(
                closeHeader,
                Environment.NewLine,
                Properties.Resources.download_file_template);
            current = current.Replace(closeHeader, injected);
            parent.InnerHtml = current;
            return doc.DocumentNode.OuterHtml;
        }

        protected static string AppendStyleTag(string content, HtmlDocument doc)
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

        protected static string AppendStyleInvoice(string content, HtmlDocument doc)
        {
            var link = $"//link[@name='invoice-css']";
            var style = doc.DocumentNode.SelectSingleNode(link);
            if (style != null) return content;
            var head = doc.DocumentNode.SelectSingleNode("//head");
            var txt = new StringBuilder(head.InnerHtml);
            txt.AppendLine();
            txt.AppendLine(invoicestyle);
            head.InnerHtml = txt.ToString();
            return doc.DocumentNode.OuterHtml;
        }
        protected const string cover = "cover-container d-flex h-100 p-3 mx-auto flex-column";
        protected const string coverstyle = "<link rel=\"stylesheet\" name=\"cover-css\" href=\"https://getbootstrap.com/docs/4.0/examples/cover/cover.css\" />";
        protected const string invoicestyle = "<link rel=\"stylesheet\" name=\"invoice-css\" href=\"/css/invoice-css.css\" />";

    }
}