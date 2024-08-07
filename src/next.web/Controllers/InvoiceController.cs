using Microsoft.AspNetCore.Mvc;
using next.web.core.services;

namespace next.web.Controllers
{
    [Route("/invoice")]
    public class InvoiceController : BaseController
    {
        private readonly ContentSanitizerInvoice sanitizer = new();

        [HttpGet("permissions")]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "blank");
            content = sanitizer.Sanitize(content);
            return GetResult(content);
        }
    }
}
