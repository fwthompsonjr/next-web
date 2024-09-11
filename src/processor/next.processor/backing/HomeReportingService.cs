using HtmlAgilityPack;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.services;
using next.processor.api.utility;

namespace next.processor.api.backing
{
    public class HomeReportingService(
        IQueueExecutor queue,
        IApiWrapper api) : BaseTimedSvc<HomeReportingService>(GetSettings())
    {
        private readonly IQueueExecutor queueExecutor = queue;
        private readonly IApiWrapper apiSvc = api;

        protected override void DoWork(object? state)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage",
            "VSTHRD002:Avoid problematic synchronous waits",
            Justification = "<Pending>")]
        protected override string GetHealth()
        {
            var health = GetHealthText();
            if (health.Equals("Healthy")) return $"{DateTime.UtcNow:s} - Reporting services are healthy";
            var html = Task.Run(async () =>
            {
                var txt = await GetIndexAsync();
                return txt;
            }).GetAwaiter().GetResult();
            var tables = new string[]
            {
                GetHeading(html),
                GetInstallationReview(html),
                GetProcessSummary(html)
            }.ToList();
            tables.RemoveAll(t => string.IsNullOrWhiteSpace(t));
            return string.Join("\n", tables);
        }

        protected override string GetStatus()
        {
            return string.Empty;
        }

        private string GetHealthText()
        {
            var available = queueExecutor.IsReadyCount();
            var actual = queueExecutor.InstallerCount();
            var status = 0;
            if (available > 0 && available < actual) status = 1;
            if (available == actual || available > actual) status = 2;
            return status switch
            {
                0 => "Unhealthy",
                1 => "Degraded",
                _ => "Healthy"
            };
        }

        private async Task<string> GetIndexAsync()
        {
            var summary = await apiSvc.FetchSummaryAsync();
            var details = queueExecutor.GetDetails();
            var health = GetHealthText().ToUpper();
            var content = HtmlMapper.Home(HtmlProvider.HomePage, health);
            content = HtmlMapper.Home(content, details);
            content = HtmlMapper.Summary(content, summary);
            return content;
        }

        private static string GetHeading(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var node = document.DocumentNode;
            List<string> parents = [
                "//*[@id='detail-01']",
                "//*[@id='detail-02']",
                "//*[@id='detail-03']"
                ];
            var line = string.Concat(Enumerable.Repeat("- ", 40));
            List<string> list = [
                "",
                line,
                $"Status: {DateTime.Now:s}"
            ];
            parents.ForEach(p =>
            {
                var div = node.SelectSingleNode(p);
                if (div != null)
                {
                    var children = div.SelectNodes("div").Select(p => p.InnerText.Trim());
                    list.Add(string.Join("\t", children));
                }
            });
            return string.Join("\n", list);
        }

        private static string GetInstallationReview(string html)
        {
            const string findbody = "//table[@name='tb-detail']/tbody";
            if (string.IsNullOrEmpty(html)) return string.Empty;
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(findbody);
            if (tbody == null) return string.Empty;
            var line = string.Concat(Enumerable.Repeat("- ", 40));
            List<string> list = [
                "",
                line,
                $"Installation: {DateTime.Now:s}"
            ];
            var rows = tbody.SelectNodes("tr").ToList();
            rows.ForEach(p =>
            {
                var children = p.SelectNodes("td").Select(p => p.InnerText.Trim().PadRight(25));
                list.Add(string.Join("\t", children));
            });

            return string.Join("\n", list);
        }

        private static string GetProcessSummary(string html)
        {
            const string findbody = "//table[@name='tb-queue-summary']/tbody";
            if (string.IsNullOrEmpty(html)) return string.Empty;
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(findbody);
            if (tbody == null) return string.Empty;
            var line = string.Concat(Enumerable.Repeat("- ", 40));
            List<string> list = [
                "",
                line,
                "Processes"
            ];
            var rows = tbody.SelectNodes("tr").ToList();
            rows.ForEach(p =>
            {
                var children = p.SelectNodes("td").Select(p => p.InnerText.Trim().PadRight(25));
                list.Add(string.Join("\t", children));
            });

            return string.Join("\n", list);
        }


        private static ServiceSettings GetSettings()
        {
            return TheSettingsProvider.GetSettingOrDefault("initialization.service").Setting;
        }
    }
}