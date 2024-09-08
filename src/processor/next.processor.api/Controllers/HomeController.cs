using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
using next.processor.api.services;
using next.processor.api.utility;
using next.processor.models;

namespace next.processor.api.Controllers
{

    public class HomeController(
        IQueueExecutor queue,
        IConfiguration configuration,
        IStatusChanger changer,
        IApiWrapper api,
        DrillDownModel model) : Controller
    {
        private readonly IConfiguration config = configuration;
        private readonly IQueueExecutor queueExecutor = queue;
        private readonly IStatusChanger changingSvc = changer;
        private readonly IApiWrapper apiSvc = api;
        private readonly DrillDownModel drillDownSvc = model;

        public async Task<IActionResult> IndexAsync()
        {
            var summary = await apiSvc.FetchSummaryAsync();
            var details = queueExecutor.GetDetails();
            var health = GetHealth().ToUpper();
            var content = HtmlMapper.Home(HtmlProvider.HomePage, health);
            content = HtmlMapper.Home(content, details);
            content = HtmlMapper.Summary(content, summary);
            return new ContentResult
            {
                Content = content,
                ContentType = "text.html"
            };
        }
        [HttpGet("alive")]
        public IActionResult Alive()
        {
            var content = HtmlMapper.Life(HtmlProvider.HomePage);
            return new ContentResult
            {
                Content = content,
                ContentType = "text.html"
            };
        }

        [HttpGet("status")]
        public async Task<IActionResult> StatusAsync()
        {
            var summary = await apiSvc.FetchSummaryAsync();
            var details = await apiSvc.FetchStatusAsync(drillDownSvc.Id);
            var statuses = new List<KeyValuePair<string, string>>
            {
                new("Health", GetHealth()),
                new("Installation", config[Constants.KeyServiceInstallation] ?? "FALSE"),
                new("Queue Processing", config[Constants.KeyQueueProcessEnabled] ?? "FALSE")
            };
            var content = HtmlMapper.Status(HtmlProvider.StatusPage, statuses);
            content = HtmlMapper.Summary(content, summary);
            content = HtmlMapper.StatusDetail(content, details);
            return new ContentResult
            {
                Content = content,
                ContentType = "text.html"
            };
        }


        [HttpGet("clear")]
        public IActionResult Clear([FromQuery] string? name)
        {
            var health = GetHealth().ToUpper();
            if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
            if (string.IsNullOrEmpty(name)) return RedirectToAction("Index", "Home");
            changingSvc.ChangeStatus(name, health);
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