using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
using next.processor.api.services;
using next.processor.api.utility;

namespace next.processor.api.Controllers
{

    public class HomeController(
        IQueueExecutor queue,
        IConfiguration configuration,
        IStatusChanger changer) : Controller
    {
        private readonly IConfiguration config = configuration;
        private readonly IQueueExecutor queueExecutor = queue;
        private readonly IStatusChanger changingSvc = changer;
        public IActionResult Index()
        {
            var details = queueExecutor.GetDetails();
            var health = (GetHealth() ?? "Unknown").ToUpper();
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
            var statuses = new List<KeyValuePair<string, string>>
            {
                new("Health", GetHealth()),
                new("Installation", config[Constants.KeyServiceInstallation] ?? "FALSE"),
                new("Queue Processing", config[Constants.KeyQueueProcessEnabled] ?? "FALSE")
            };
            var content = HtmlMapper.Status(HtmlProvider.StatusPage, statuses);

            return new ContentResult
            {
                Content = content,
                ContentType = "text.html"
            };
        }


        [HttpGet("clear")]
        public IActionResult Clear([FromQuery] string? name)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
            if (string.IsNullOrEmpty(name)) return RedirectToAction("Index", "Home");
            changingSvc.ChangeStatus(name);
            return RedirectToAction("Status");
        }

        private string GetHealth()
        {
            var available = queueExecutor.IsReadyCount();
            var actual = queueExecutor.InstallerCount();
            var status = 0;
            if (available > 0 && available < actual) status = 1;
            if (available == actual) status = 2;
            return status switch
            {
                0 => "Unhealthy",
                1 => "Degraded",
                _ => "Healthy"
            };
        }
    }
}