using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.services;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/invoice")]
    public class InvoiceController : BaseController
    {
        private readonly ContentSanitizerSubscription _subscriptionSvc;
        private readonly IPermissionApi? _api;
        public InvoiceController()
        {
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            var svc = AppContainer.GetSanitizer("invoice-subscription");
            if (svc is not ContentSanitizerSubscription subsvc) subsvc = new();
            if (api != null) _api = api;
            _subscriptionSvc = subsvc;
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "blank");
            content = await _subscriptionSvc.GetContent(session, _api, content);
            return GetResult(content);
        }
    }
}
