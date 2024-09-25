using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using next.core.interfaces;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.services;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController(IApiWrapper wrapsvc, IAccountMapService mapService, IViolationService violations) : BaseController(wrapsvc, violations)
    {
        private readonly IAccountMapService mapSvc = mapService;

        [HttpGet]
        [Route("home")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Index()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetPage("account-home");
        }

        [HttpGet]
        [Route("profile")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Profile()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetPage("account-profile");
        }

        [HttpGet]
        [Route("permissions")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Permissions()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetPage("account-permissions");
        }

        [HttpGet]
        [Route("cache-manager")]
        public async Task<IActionResult> CacheManagement()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
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

        [HttpGet]
        [Route("account-restriction")]
        public async Task<IActionResult> Restrictions()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            const string name = "restriction-manager";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) Redirect("/home");
            var sanitizer = AppContainer.GetSanitizer(name);
            var content = sanitizer.Sanitize(string.Empty);
            content = await AppendStatus(content, true);
            content = await ContentSanitizerRestriction.AppendDetail(content, apiwrapper, session);
            var doc = content.ToHtml();
            content = doc.DocumentNode.OuterHtml;
            return GetResult(content);
        }

        [HttpGet]
        [Route("account-upgrade-limits")]
        public async Task<IActionResult> RestrictionsUpgrade()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) Redirect("/home");
            await ContentSanitizerRestriction.UpgradeRequest(apiwrapper, session);
            return RedirectToAction("Restrictions");
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
