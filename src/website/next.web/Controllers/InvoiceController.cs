using next.core.entities;
using next.core.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.services;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;

namespace next.web.Controllers
{
    [Route("/invoice")]
    public class InvoiceController : BaseController
    {
        private readonly ContentSanitizerInvoice _subscriptionSvc;
        private readonly ContentSanitizerPayment _paymentSvc;
        private readonly IPermissionApi? _api;
        public InvoiceController(IApiWrapper apiwrapper) : base(apiwrapper)
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
            content = await AppendStatus(content);
            content = GetHttpRedirect(content, session);
            return GetResult(content);
        }

        [HttpGet("purchase")]
        [SuppressMessage("Major Code Smell",
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
                var user = session.GetUser();
                var content = await GetAuthenicatedPage(session, "blank");
                if (!Guid.TryParse(id, out var _) || _api == null || user == null)
                {
                    content = _subscriptionSvc.Sanitize(content);
                    return GetResult(content);
                }
                var response = await GetPurchaseRecord(landing, id, user, content);
                return response;
            }
            finally
            {
                session.Remove(PurchaseRecordId);
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member. Tested from public accessor. Integration only")]
        private async Task<IActionResult> GetPurchaseRecord(string landing, string id, UserBo user, string content)
        {
            if (_api == null) return GetResult(content);
            var app = await _api.Post("search-get-invoice", new { Id = id }, user);
            if (app == null || app.StatusCode != 200) return GetResult(content);
            var detail = app.Message.ToInstance<GenerateInvoiceResponse>();
            if (detail == null) return GetResult(content);
            var remote = await GetRemoteContent(landing, detail.ExternalId ?? id);
            var address = GetWebAddress(Request);
            content = _paymentSvc.Transform(content, remote, address);
            content = await AppendStatus(content);
            return GetResult(content);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member. Tested from public accessor. Integration only")]
        private static string GetWebAddress(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}";
        }

        [ExcludeFromCodeCoverage(Justification = "Private member. Tested from public accessor. Integration only")]
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
                baseAddress = $"{baseAddress}?id={id}";
            }
            return baseAddress;
        }
        private const string PurchaseRecordId = "purchase_record_id";
    }
}
