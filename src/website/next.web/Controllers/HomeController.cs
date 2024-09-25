using Microsoft.AspNetCore.Mvc;
using next.core.interfaces;
using next.web.core.interfaces;
using next.web.core.services;
using next.web.core.util;
using next.web.Models;
using System.Diagnostics;

namespace next.web.Controllers
{
    [Route("/")]
    public partial class HomeController : BaseController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality",
            "IDE0052:Remove unread private members",
            Justification = "This item is planned for future use and part of MVC pattern.")]
        private readonly ILogger<HomeController> _logger;
        private readonly ISessionStringWrapper? _sessionStringWrapper;
        private readonly IFetchIntentService _intentSvc;
        public HomeController(ILogger<HomeController> logger,
            IApiWrapper apiWrapper,
            IViolationService violations,
            ISessionStringWrapper? wrapper = null,
            IFetchIntentService? intentSvc = null
            ) : base(apiWrapper, violations)
        {
            _logger = logger;
            if (wrapper != null) _sessionStringWrapper = wrapper;
            if (intentSvc == null) _intentSvc = new FetchIntentService();
            else _intentSvc = intentSvc;
        }
        [HttpGet("home")]
        public async Task<IActionResult> Index()
        {
            var isViolation = IsViolation(HttpContext);
            var helper = AppContainer.GetSanitizer("post-login");
            var content = Introduction;
            var session = HttpContext.Session;
            if (string.IsNullOrWhiteSpace(content)) return View();
            if (IsSessionAuthenicated(session) && helper is ContentSanitizerHome home)
            {
                content = home.Sanitize(content);
                content = await AppendStatus(content);
            }
            if (isViolation)
            {
                content = ContentSanitizerHome.ApplyViolation(content);
            }
            content = GetHttpRedirect(content, session);
            return new ContentResult
            {
                ContentType = "text/html",
                Content = RemoveHeaderDuplicate(content)
            };
        }
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var helper = AppContainer.GetSanitizer("logout");
            var content = GetPageOrDefault("blank");
            var session = HttpContext.Session;
            if (HttpContext != null && session != null) session.Clear();
            if (helper is ContentSanitizerLogout home)
            {
                content = home.Sanitize(content);
            }
            if (session != null) content = GetHttpRedirect(content, session);
            return new ContentResult
            {
                ContentType = "text/html",
                Content = RemoveHeaderDuplicate(content)
            };
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Test()
        {
            var session = HttpContext.Session;
            var text = ContentSanitizerBase.IndexContent;
            text = apiwrapper.InjectHttpsRedirect(text, session).GetAwaiter().GetResult();
            return new ContentResult
            {
                Content = text,
                ContentType = "text/html",
            };
        }
    }
}
