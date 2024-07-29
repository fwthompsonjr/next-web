using Microsoft.AspNetCore.Mvc;
using next.web.core.util;
using next.web.Models;
using System.Diagnostics;

namespace next.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var content = Introduction;
            if (string.IsNullOrWhiteSpace(content)) return View();
            return new ContentResult
            {
                ContentType = "text/html",
                Content = content
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string? _introduction;
        private static string Introduction => _introduction ??= GetIntroduction();
        private static string GetIntroduction()
        {
            var content = ContentHandler.GetLocalContent("home");
            if (content == null || string.IsNullOrWhiteSpace(content.Content)) return string.Empty;
            return content.Content;
        }
    }
}
