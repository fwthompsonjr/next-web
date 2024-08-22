using Microsoft.AspNetCore.Mvc;

namespace next.maintenance.web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new ContentResult { Content = HomePage, ContentType = "text.html" };
        }
        private static string HomePage => homePage ??= GetHomeContent();
        private static string? homePage;
        private static string GetHomeContent()
        {
            return Properties.Resources.home_page;
        }
    }
}
