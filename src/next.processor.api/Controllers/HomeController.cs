using Microsoft.AspNetCore.Mvc;
using next.processor.api.services;

namespace next.processor.api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new ContentResult { 
                Content = HtmlProvider.HomePage, 
                ContentType = "text.html" 
            };
        }
    }
}