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
            return await GetPage("mysearch-home");
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> Active()
        {
            return await GetPage("mysearch-active");
        }

        [HttpGet]
        [Route("purchases")]
        public async Task<IActionResult> Purchases()
        {
            return await GetPage("mysearch-purchases");
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


        private async Task<IActionResult> GetPage(string viewName)
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "mysearch");
            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = RemoveHeaderDuplicate(content)
                };
            }

            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = viewer.SetTab(content);

            return new ContentResult
            {
                ContentType = "text/html",
                Content = RemoveHeaderDuplicate(content)
            };
        }
    }
}
