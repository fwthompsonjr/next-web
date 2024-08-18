using Microsoft.AspNetCore.Mvc;
using next.processor.api.services;

namespace next.processor.api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var content = HtmlMapper.Home(HtmlProvider.HomePage);
            return new ContentResult { 
                Content = content, 
                ContentType = "text.html" 
            };
        }
    }
}