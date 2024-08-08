using next.web.core.extensions;

namespace next.web.core.services
{
    internal class ContentSanitizerPayment : ContentSanitizerInvoice
    {
        public string Transform(string content, string remote, string baseWebAddress = "")
        {
            const string xpath = "//div[@name='main-content']";
            const string findtarget = "//*[@id='invoice-card']";
            const string findcheckout = "//script[@name='checkout-stripe-js']";
            content = Sanitize(content);
            var node = content.ToHtml().DocumentNode;
            var remoteNode = remote.ToHtml().DocumentNode;

            var source = remoteNode.SelectSingleNode(xpath);
            if (source == null) return node.OuterHtml;
            var target = node.SelectSingleNode(findtarget);
            if (target == null) return node.OuterHtml;
            target.InnerHtml = source.OuterHtml;
            var attr = target.Attributes.FirstOrDefault(a => a.Name == "class");
            if (attr != null) target.Attributes.Remove(attr);
            var script = remoteNode.SelectSingleNode(findcheckout);
            if (script == null) return node.OuterHtml;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return node.OuterHtml;

            var detail = string.Concat(body.InnerHtml, Environment.NewLine, script.OuterHtml);
            if (!string.IsNullOrEmpty(baseWebAddress))
            {
                detail = detail.Replace("http://api.legallead.co", baseWebAddress);
            }
            body.InnerHtml = detail;

            return node.OuterHtml;
        }
    }
}