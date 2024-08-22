using HtmlAgilityPack;

namespace next.web.core.services
{
    internal class ContentSanitizerConfirmation : ContentSanitizerInvoice
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            content = AppendLayout(content, doc);
            content = AppendStyleInvoice(content, doc);
            content = AppendStyleTag(content, doc);
            content = DisplayParentMenus(content);
            content = ApplyYearToFooter(content);
            return content;
        }
        private static string AppendLayout(string content, HtmlDocument doc)
        {
            const string closeHeader = "</header>";
            var find = $"//div[@class='{cover}']";
            var parent = doc.DocumentNode.SelectSingleNode(find);
            if (parent == null) return content;
            var current = parent.InnerHtml;
            var injected = string.Concat(
                closeHeader,
                Environment.NewLine,
                Properties.Resources.payment_confirmation_page);
            current = current.Replace(closeHeader, injected);
            parent.InnerHtml = current;
            return doc.DocumentNode.OuterHtml;
        }
    }
}
