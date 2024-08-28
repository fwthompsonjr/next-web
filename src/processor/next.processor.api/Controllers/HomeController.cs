using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
using next.processor.api.services;

namespace next.processor.api.Controllers
{

    public class HomeController(IQueueExecutor queue) : Controller
    {
        private readonly IQueueExecutor queueExecutor = queue;
        public IActionResult Index()
        {
            var available = queueExecutor.IsReadyCount();
            var actual = queueExecutor.InstallerCount();
            var details = queueExecutor.GetDetails();
            var status = 0;
            if (available > 0 && available < actual) status = 1;
            if (available == actual) status = 2;
            var health = status switch
            {
                0 => "Unhealthy",
                1 => "Degraded",
                _ => "Healthy"
            };
            var content = HtmlMapper.Home(HtmlProvider.HomePage, health);
            content = HtmlMapper.Home(content, details);
            return new ContentResult
            {
                Content = content,
                ContentType = "text.html"
            };
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            var content = HtmlMapper.Status(HtmlProvider.StatusPage);
            return new ContentResult
            {
                Content = content,
                ContentType = "text.html"
            };
        }
    }
}