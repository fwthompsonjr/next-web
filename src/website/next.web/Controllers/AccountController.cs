using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.services;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController(IApiWrapper wrapper, IAccountMapService mapService) : BaseController(wrapper)
    {
        private readonly IAccountMapService mapSvc = mapService;

        [HttpGet]
        [Route("home")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Index()
        {
            return await GetPage("account-home");
        }

        [HttpGet]
        [Route("profile")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Profile()
        {
            return await GetPage("account-profile");
        }

        [HttpGet]
        [Route("permissions")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Permissions()
        {
            return await GetPage("account-permissions");
        }

        [HttpGet]
        [Route("cache-manager")]
        public async Task<IActionResult> CacheManagement()
        {
            const string name = "cache-manager";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) Redirect("/home");
            var sanitizer = AppContainer.GetSanitizer(name);
            var content = sanitizer.Sanitize(string.Empty);
            content = await AppendStatus(content, true);
            var doc = content.ToHtml();
            session.InjectSessionKeys(doc);
            content = doc.DocumentNode.OuterHtml;
            return GetResult(content);
        }

        private async Task<IActionResult> GetPage(string viewName)
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "myaccount");
            if (mapSvc is AccountMapService svc && svc.Api == null)
            {
                svc.Api = apiwrapper;
            }
            content = mapSvc.GetHtml(content, viewName);
            content = await mapSvc.Transform(content, session);
            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null)
            {
                return GetResult(content);
            }

            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = viewer.SetTab(content);
            content = await AppendStatus(content);
            content = GetHttpRedirect(content, session);

            return GetResult(content);
        }
    }
}
