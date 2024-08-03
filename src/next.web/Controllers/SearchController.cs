using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/search")]
    public class SearchController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "myaccount");
            return new ContentResult
            {
                ContentType = "text/html",
                Content = content
            };
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> Active()
        {
            return await Index();
        }

        [HttpGet]
        [Route("purchases")]
        public async Task<IActionResult> Purchases()
        {
            return await Index();
        }


        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> History()
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "viewhistory");
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            if (api != null)
            {
                content = await session.GetHistory(api, content);
            }
            return new ContentResult
            {
                ContentType = "text/html",
                Content = content
            };
        }
    }
}
