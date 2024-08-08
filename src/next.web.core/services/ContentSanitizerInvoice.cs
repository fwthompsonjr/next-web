using HtmlAgilityPack;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web.core.services
{
    internal class ContentSanitizerInvoice : ContentSanitizerBase
    {

        public virtual async Task<string> GetContent(ISession session, IPermissionApi? api, string content, string baseWebAddress = "")
        {
            const string findcheckout = "//script[@name='checkout-stripe-js']";
            var key = SessionKeyNames.UserPermissionChanged;
            var html = Sanitize(content);
            var user = session.GetUser();
            var obj = session.Retrieve<PermissionChangedResponse>(key);
            if (api == null || user == null || obj == null) return html;
            var navigateTo = GetInvoiceUri(obj);
            if (none.Equals(navigateTo)) return html;
            var remote = await GetInvoiceHtml(navigateTo);
            if (none.Equals(remote)) return html;
            var doc = html.ToHtml();
            var doc2 = remote.ToHtml();
            var node = doc2.DocumentNode;
            var paymentElement = node.SelectSingleNode("//*[@id='payment-element']");
            if (paymentElement != null) { paymentElement.InnerHtml = string.Concat(Environment.NewLine, "<!--Stripe.js injects the Payment Element-->"); }
            var invoiceNodes = node.SelectSingleNode("//*[@id='dv-subcontent-invoice']");
            if (invoiceNodes == null || invoiceNodes.ChildNodes.Count == 0) return html;
            // remove fallback title
            var title = doc.DocumentNode.SelectSingleNode("//*[@id='invoice-fallback-title']");
            title?.ParentNode.RemoveChild(title);

            // populate invoice description
            var detail = invoiceNodes.ChildNodes.ToList()
                .Find(x => x.Name.Equals("div", StringComparison.OrdinalIgnoreCase))?.InnerHtml ?? string.Empty;
            var target = doc.DocumentNode.SelectSingleNode("//*[@id='invoice-card-content']");
            if (target == null) return html;
            target.InnerHtml = detail;
            // inject checkout script
            var checkoutJs = node.SelectSingleNode(findcheckout);
            if (checkoutJs == null) return doc.DocumentNode.OuterHtml;
            var js = checkoutJs.OuterHtml;
            var body = doc.DocumentNode.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return doc.DocumentNode.OuterHtml;
            detail = string.Concat(body.InnerHtml, Environment.NewLine, js);
            if (!string.IsNullOrEmpty(baseWebAddress))
            {
                detail = detail.Replace("http://api.legallead.co", baseWebAddress);
            }
            body.InnerHtml = detail;
            // update invoice heading
            var heading = doc.DocumentNode.SelectSingleNode("//span[@automationid='invoice-label']");
            if (heading != null) heading.InnerHtml = heading.InnerHtml.Replace("Legal Lead", "Oxford Lead");
            return doc.DocumentNode.OuterHtml;
        }

        public override string Sanitize(string content)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            content = AppendMain(content, doc);
            content = AppendStyleInvoice(content, doc);
            content = AppendStyleTag(content, doc);
            content = AppendStyleStripe(content, doc);
            content = DisplayParentMenus(content);
            content = ApplyYearToFooter(content);
            return content;
        }

        protected static async Task<string> GetInvoiceHtml(string navigateTo)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync(navigateTo);
                return response;
            }
            catch (Exception)
            {
                return none;
            }
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
        protected static string AppendStyleStripe(string content, HtmlDocument doc)
        {
            const string common = "<!-- common styles -->";
            var link = $"//script[@name='stripe-services']";
            var style = doc.DocumentNode.SelectSingleNode(link);
            if (style != null) return content;
            var head = doc.DocumentNode.SelectSingleNode("//head");
            var txt = new StringBuilder(head.InnerHtml);
            var inject = string.Join(Environment.NewLine, stripeScript);
            txt.Replace(common, inject);
            head.InnerHtml = txt.ToString();
            return doc.DocumentNode.OuterHtml;
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
                Properties.Resources.invoice_page);
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

        protected static string GetInvoiceUri(PermissionChangedResponse? response)
        {
            const string landingName = "Subscription";
            const string guest = "guest";
            if (response == null || response.Dto == null) return none;
            var levelName = response.Dto.LevelName ?? string.Empty;
            var navigateTo = response.Dto.InvoiceUri ?? none;
            if (!navigateTo.Equals(none, StringComparison.OrdinalIgnoreCase)) { return navigateTo; }
            if (!levelName.Equals(guest, StringComparison.OrdinalIgnoreCase)) { return navigateTo; }
            return BuildUri(landingName, response.Dto);
        }
        protected static string BuildUri(string landing, PermissionChangedItem dto)
        {
            var landingName = paymentLandings.Find(x => x.Equals(landing, StringComparison.OrdinalIgnoreCase));
            if (landingName == null) { return none; }
            if (string.IsNullOrWhiteSpace(dto.ExternalId)) return none;
            if (string.IsNullOrWhiteSpace(dto.SessionId)) return none;
            var id = paymentLandings.IndexOf(landingName);
            var hostname = AppContainer.PermissionApiBase;
            if (string.IsNullOrEmpty(hostname)) return none;
            var url = string.Concat(paymentUrls[id], "?id={1}&sessionid={2}");
            return string.Format(url, hostname, dto.ExternalId, dto.SessionId);
        }
        protected const string cover = "cover-container d-flex h-100 p-3 mx-auto flex-column";
        protected const string coverstyle = "<link rel=\"stylesheet\" name=\"cover-css\" href=\"https://getbootstrap.com/docs/4.0/examples/cover/cover.css\" />";
        protected const string invoicestyle = "<link rel=\"stylesheet\" name=\"invoice-css\" href=\"/css/invoice-css.css\" />";
        protected const string none = "NONE";
        protected static readonly List<string> paymentLandings = ["Discounts", "Subscription"];
        protected static readonly string[] paymentUrls = ["{0}discount-checkout", "{0}subscription-checkout"];
        protected static readonly string[] stripeScript = [
            "<!-- stripe -->",
            "<script name=\"stripe-services\" src=\"https://js.stripe.com/v3/\"></script>",
            "<!-- common styles -->"
        ];
    }
}