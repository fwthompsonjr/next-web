using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.services;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/invoice")]
    public class InvoiceController : BaseController
    {
        private readonly ContentSanitizerSubscription sanitizer = new();

        [HttpGet("permissions")]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "blank");
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            content = await sanitizer.GetContent(session, api, content);
            return GetResult(content);
        }
    }
}
