namespace next.web.core.services
{
    internal class ContentSanitizerHome : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            var html = DisplayMenuOptions(doc, content);
            return html;
        }
    }
}
