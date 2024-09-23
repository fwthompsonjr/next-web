using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using next.web.core.extensions;
using next.web.core.util;
using System.Text;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController(IApiWrapper wrapper) : BaseController(wrapper)
    {
        [HttpGet]
        [Route("home")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Index()
        {
            return await GetPage("account-home");
        }

        [HttpGet]
        [Route("profile")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Profile()
        {
            return await GetPage("account-profile");
        }

        [HttpGet]
        [Route("permissions")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Permissions()
        {
            return await GetPage("account-permissions");
        }

        [HttpGet]
        [Route("cache-manager")]
        public async Task<IActionResult> CacheManagement()
        {
            const string name = "cache-manager";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) Redirect("/home");
            var sanitizer = AppContainer.GetSanitizer(name);
            var content = sanitizer.Sanitize(string.Empty);
            content = await AppendStatus(content, true);
            var doc = content.ToHtml();
            session.InjectSessionKeys(doc);
            content = doc.DocumentNode.OuterHtml;
            return GetResult(content);
        }

        private async Task<IActionResult> GetPage(string viewName)
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "myaccount");
            var views = "account-home,account-profile,account-permissions".Split(',').ToList();
            views.Remove(viewName);
            content = AppendHeadings(content, viewName);
            content = AppendModals(content);
            content = AppendShell(content);
            content = PostPendScripts(content);
            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null)
            {
                return GetResult(content);
            }

            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = viewer.SetTab(content);
            content = await AppendStatus(content);
            content = GetHttpRedirect(content, session);
            foreach (var view in views)
            {
                var original = await GetLandingText(view);
                content = RestoreDiv(content, original);
            }
            return GetResult(content);
        }
        private async Task<string> GetLandingText(string viewName)
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "myaccount");
            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null)
            {
                return content;
            }
            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = viewer.SetTab(content);
            content = await AppendStatus(content);
            content = GetHttpRedirect(content, session);
            return content;
        }


        private static string AppendHeadings(string content, string viewName)
        {
            const string title = "oxford.leads.web: {0}";
            var landing = viewName.Split('-')[^1];
            var landingName = landing switch
            {
                "home" => "account",
                _ => landing,
            };
            var pageTitle = string.Format(title, landingName);
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var head = node.SelectSingleNode("//head");
            if (head == null) return node.OuterHtml;
            var headingBlock = core.Properties.Resources.base_account_heading;
            head.InnerHtml = headingBlock;
            var nodeTitle = node.SelectSingleNode("//head/title");
            if (nodeTitle != null) nodeTitle.InnerHtml = pageTitle;
            return node.OuterHtml;
        }

        private static string AppendModals(string content)
        {
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode("//body");
            if (body == null) return node.OuterHtml;
            var modalBlock = core.Properties.Resources.base_account_modals;
            var bodyText = string.Concat(modalBlock, Environment.NewLine, body.InnerHtml);
            body.InnerHtml = bodyText;
            return node.OuterHtml;

        }
        private static string AppendShell(string content)
        {
            var find = "//*[@id='dv-subcontent-home']";
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode("//body");
            if (body == null) return node.OuterHtml;
            var shellBlock = core.Properties.Resources.base_account_shell;
            var originalNode = node.SelectSingleNode(find);
            if (originalNode == null) return node.OuterHtml;
            var inner = originalNode.InnerHtml;
            var substitution = originalNode.OuterHtml;
            body.InnerHtml = body.InnerHtml.Replace(substitution, shellBlock);
            originalNode = node.SelectSingleNode(find);
            if (originalNode == null) return node.OuterHtml;
            originalNode.InnerHtml = inner;
            return node.OuterHtml;
        }
        private static string PostPendScripts(string content)
        {
            const string fmt = "<script name='{0}' src='{1}'></script>";
            var names = new Dictionary<string, string> () {
                { "handler_js", "/js/handler.js" },
                { "my-account-common", "/js/my-account-common.js" },
                { "re-authenticate-js", "/js/re-authenticate-js.js" },
                { "my-account-validate", "/js/my-account-validate.js" }
            };
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode("//body");
            if (body == null) return node.OuterHtml;
            var inner = body.InnerHtml + Environment.NewLine;
            var builder = new StringBuilder(inner);
            foreach (var item in names)
            {
                var line = string.Format(fmt, item.Key, item.Value);
                builder.AppendLine(line);
            }
            body.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }

        private static string RestoreDiv(string modified, string content)
        {
            var doc1 = modified.ToHtml();
            var doc2 = content.ToHtml();
            var dest = doc1.DocumentNode;
            var src = doc2.DocumentNode;
            var targets = new List<string>() {
                "//*[@id='dv-subcontent-profile']",
                "//*[@id='dv-subcontent-permissions']",
                "//*[@id='dv-subcontent-password']",
            };
            targets.ForEach(qry =>
            {
                var snode = src.SelectSingleNode(qry);
                var dnode = dest.SelectSingleNode(qry);
                if (snode != null && dnode != null)
                {
                    dnode.InnerHtml = snode.InnerHtml;
                }
            });
            return dest.OuterHtml;
        }
    }
}
