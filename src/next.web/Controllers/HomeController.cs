using Microsoft.AspNetCore.Mvc;
using next.web.core.services;
using next.web.core.util;
using next.web.Models;
using System.Diagnostics;

namespace next.web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var helper = AppContainer.GetSanitizer("post-login");
            var content = Introduction;
            if (string.IsNullOrWhiteSpace(content)) return View();
            if (IsSessionAuthenicated(HttpContext.Session) && helper is ContentSanitizerHome home)
            {
                content = home.Sanitize(content);
            }
            return GetResult(content);
        }

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
            return GetResult(content);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
