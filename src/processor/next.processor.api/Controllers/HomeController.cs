using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
using next.processor.api.services;
using next.processor.api.utility;

namespace next.processor.api.Controllers
{

    public class HomeController(IQueueExecutor queue, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration config = configuration;
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


        [HttpGet("clear")]
        public IActionResult Clear([FromQuery] string? name)
        {
            if (!ModelState.IsValid) RedirectToAction("Index", "Home");
            if (name != null && name.Equals("stop"))
            {
                config[Constants.KeyServiceInstallation] = "false";
                config[Constants.KeyQueueProcessEnabled] = "false";
                return RedirectToAction("Status");
            }
            if (name != null && name.Equals("errors"))
            {
                var selection = TrackEventService.Models.Find(x => x.Name == Constants.ErrorLogName);
                if (selection != null) TrackEventService.Models.Remove(selection);
            }
            return RedirectToAction("Status");
        }
    }
}