using Microsoft.AspNetCore.Mvc;
using next.core.interfaces;
using next.web.core.extensions;
using next.web.core.services;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;

namespace next.web.Controllers
{
    [Route("/search")]
    public class SearchController(IApiWrapper wrapper, IViolationService violations) : BaseController(wrapper, violations)
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetPage("mysearch-home");
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> Active()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetHistory(SearchFilterNames.Active);
        }

        [HttpGet]
        [Route("purchases")]
        public async Task<IActionResult> Purchases()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetHistory(SearchFilterNames.Purchases);
        }


        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> History()
        {
            var isViolation = IsViolation(HttpContext);
            if (isViolation) RedirectToAction("Index", "Home");
            return await GetHistory();
        }


        [ExcludeFromCodeCoverage(Justification = "Helper method tested completely through public accessor")]
        private async Task<IActionResult> GetPage(string viewName)
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "mysearch");
            var fallback = GetResult(content);
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            if (api == null) return fallback;
            fallback = GetResult(content);

            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null) return fallback;

            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = await AppendStatus(content);
            content = GetHttpRedirect(content, session);
            content = await AccountMapService.TransformSearch(content, apiwrapper, session);
            return GetResult(content);
        }
        [ExcludeFromCodeCoverage(Justification = "Helper method tested completely through public accessor")]
        private async Task<IActionResult> GetHistory(SearchFilterNames searchFilter = SearchFilterNames.History)
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "viewhistory");
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            if (api != null) content = await session.GetHistory(api, content, searchFilter);
            content = await AppendStatus(content, true);
            content = RemoveOption(content, searchFilter);
            content = SetPageTitle(content, searchFilter);
            content = GetHttpRedirect(content, session);
            content = await AccountMapService.TransformSearch(content, apiwrapper, session);
            return GetResult(content);
        }
        [ExcludeFromCodeCoverage(Justification = "Helper method tested completely through public accessor")]
        protected string RemoveOption(string content, SearchFilterNames searchFilter)
        {
            const string vu = "value";
            const string st = "style";
            const string dn = "display: none";
            if (searchFilter == SearchFilterNames.History) return content;
            const string findcbo = "//*[@id='cbo-search-history-filter']";
            List<string> indexes = searchFilter switch
            {
                SearchFilterNames.Purchases => ["1", "2", "3", "10"],
                SearchFilterNames.Active => ["4", "5", "10"],
                _ => [],
            };
            var document = content.ToHtml();
            var node = document.DocumentNode;
            var cbo = node.SelectSingleNode(findcbo);
            if (cbo == null || indexes.Count == 0) return node.OuterHtml;
            var options = cbo.SelectNodes("option").ToList();
            options.ForEach(option =>
            {
                var attrValue = option.Attributes.FirstOrDefault(a => a.Name == vu)?.Value ?? string.Empty;
                if (indexes.Contains(attrValue))
                {
                    var attrStyle = option.Attributes.FirstOrDefault(a => a.Name == st);
                    if (attrStyle == null) option.Attributes.Add(st, dn);
                    else attrStyle.Value = dn;
                }
            });
            return node.OuterHtml;
        }
        [ExcludeFromCodeCoverage(Justification = "Helper method tested completely through public accessor")]
        protected string SetPageTitle(string content, SearchFilterNames searchFilter)
        {
            const string fmt = "oxford.leads.web: {0}";
            if (searchFilter == SearchFilterNames.History) return content;
            var document = content.ToHtml();
            var node = document.DocumentNode;
            var head = node.SelectSingleNode("//head");
            if (head == null) return node.OuterHtml;
            var title = head.SelectSingleNode("title");
            if (title == null) return node.OuterHtml;
            var newtitle = searchFilter switch
            {
                SearchFilterNames.Active => string.Format(fmt, "active searches"),
                SearchFilterNames.Purchases => string.Format(fmt, "purchase history"),
                _ => "search history"
            };
            title.InnerHtml = newtitle;
            return node.OuterHtml;
        }
    }
}
