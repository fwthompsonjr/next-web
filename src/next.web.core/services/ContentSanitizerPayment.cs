using next.web.core.extensions;

namespace next.web.core.services
{
    internal class ContentSanitizerPayment : ContentSanitizerInvoice
    {
        public string Transform(string content, string remote, string baseWebAddress = "")
        {
            const string xpath = "//div[@name='main-content']";
            const string findtarget = "//*[@id='invoice-card-content']";
            content = Sanitize(content);
            var node = content.ToHtml().DocumentNode;
            var remoteNode = remote.ToHtml().DocumentNode;

            var source = remoteNode.SelectSingleNode(xpath);
            if (source == null) return node.OuterHtml;
            var target = node.SelectSingleNode(findtarget);
            if (target == null) return node.OuterHtml;
            target.InnerHtml = source.OuterHtml;

            return node.OuterHtml;
        }
    }
}