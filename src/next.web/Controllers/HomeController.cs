using Microsoft.AspNetCore.Mvc;
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
            ISessionStringWrapper? wrapper = null,
            IFetchIntentService? intentSvc = null
            ) : base(apiWrapper)
        {
            _logger = logger;
            if (wrapper != null) _sessionStringWrapper = wrapper;
            if (intentSvc == null) _intentSvc = new FetchIntentService();
            else _intentSvc = intentSvc;
        }
        [HttpGet("home")]
        public async Task<IActionResult> Index()
        {
            var helper = AppContainer.GetSanitizer("post-login");
            var content = Introduction;
            if (string.IsNullOrWhiteSpace(content)) return View();
            if (IsSessionAuthenicated(HttpContext.Session) && helper is ContentSanitizerHome home)
            {
                content = home.Sanitize(content);
                content = await AppendStatus(content);
            }
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
            if (HttpContext != null && HttpContext.Session != null) HttpContext.Session.Clear();
            if (helper is ContentSanitizerLogout home)
            {
                content = home.Sanitize(content);
            }
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
            var text = ContentSanitizerBase.IndexContent;
            return new ContentResult
            {
                Content = text,
                ContentType = "text/html",
            };
        }
    }
}
