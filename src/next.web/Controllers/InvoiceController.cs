using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.services;
using next.web.core.util;
using System.Web;

namespace next.web.Controllers
{
    [Route("/invoice")]
    public class InvoiceController : BaseController
    {
        private readonly ContentSanitizerInvoice _subscriptionSvc;
        private readonly ContentSanitizerPayment _paymentSvc;
        private readonly IPermissionApi? _api;
        public InvoiceController()
        {
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            var svc = AppContainer.GetSanitizer("invoice-subscription");
            var svr = AppContainer.GetSanitizer("payment-checkout");
            if (svc is not ContentSanitizerInvoice subsvc) subsvc = new();
            if (svr is not ContentSanitizerPayment pmtsvc) pmtsvc = new();
            if (api != null) _api = api;
            _subscriptionSvc = subsvc;
            _paymentSvc = pmtsvc;
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "blank");
            var address = GetWebAddress(Request);
            content = await _subscriptionSvc.GetContent(session, _api, content, address);
            return GetResult(content);
        }

        [HttpGet("purchase")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", 
            "S6967:ModelState.IsValid should be called in controller actions", 
            Justification = "For http-get query parm is validated")]
        public IActionResult Purchase([FromQuery] string? id)
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            session.SetString(PurchaseRecordId, id ?? string.Empty);
            return RedirectToAction("PurchaseRecord");
        }

        [HttpGet("purchase-record")]
        public async Task<IActionResult> PurchaseRecord()
        {
            const string landing = "payment-checkout";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var id = session.GetString(PurchaseRecordId) ?? string.Empty;
            try
            {
                var content = await GetAuthenicatedPage(session, "blank");
                if (!Guid.TryParse(id, out var _))
                {
                    content = _subscriptionSvc.Sanitize(content);
                    return GetResult(content);
                }
                var remote = await GetRemoteContent(landing, id);
                var address = GetWebAddress(Request);
                content = _paymentSvc.Transform(content, remote, address);
                return GetResult(content);
            }
            finally
            {
                session.Remove(PurchaseRecordId);
            }
        }
        private static string GetWebAddress(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}";
        }

        private static async Task<string> GetRemoteContent(string landing, string? id)
        {
            var target = GetRemoteUri(landing, id);
            try
            {
                using var client = new HttpClient();
                var html = await client.GetStringAsync(target);
                return html;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetRemoteUri(string landing, string? id)
        {
            const string remoteServer = "http://api.legallead.co";

            var baseAddress = $"{remoteServer}/{landing}";
            if (!string.IsNullOrEmpty(id) && baseAddress.Contains('?'))
            {
                baseAddress = $"{baseAddress}&id={id}";
            }
            if (!string.IsNullOrEmpty(id) && !baseAddress.Contains('?'))
            {
                baseAddress = $"{baseAddress}?id='{HttpUtility.UrlEncode(id)}'";
            }
            return baseAddress;
        }
        private const string PurchaseRecordId = "purchase_record_id";
    }
}
