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
            return await GetHistory(SearchFilterNames.Active);
        }

        [HttpGet]
        [Route("purchases")]
        public async Task<IActionResult> Purchases()
        {
            return await GetHistory(SearchFilterNames.Purchases);
        }


        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> History()
        {
            return await GetHistory();
        }


        private async Task<IActionResult> GetPage(string viewName)
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "mysearch");
            var fallback = GetResult(content);
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            if (api == null) return fallback;
            fallback = GetResult(content);

            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null) return fallback;

            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = await AppendStatus(content);

            return GetResult(content);
        }

        private async Task<IActionResult> GetHistory(SearchFilterNames searchFilter = SearchFilterNames.History)
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "viewhistory");
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            if (api != null) content = await session.GetHistory(api, content, searchFilter);
            content = await AppendStatus(content);
            return GetResult(content);
        }
    }
}
