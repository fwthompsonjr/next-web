using next.web.core.extensions;
using next.web.core.interfaces;
using System.Text;

namespace next.web.core.services
{
    public class AccountMapService : IAccountMapService
    {

        public string GetHtml(string content, string viewName)
        {
            var html = Headings(content, viewName);
            html = Modals(html);
            html = Shell(html);
            html = Scripts(html);
            return html;
        }

        public string Headings(string content, string viewName)
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
            var head = node.SelectSingleNode(map["head"]);
            if (head == null) return node.OuterHtml;
            var headingBlock = heading;
            head.InnerHtml = headingBlock;
            var nodeTitle = node.SelectSingleNode(map["title"]);
            if (nodeTitle != null) nodeTitle.InnerHtml = pageTitle;
            return node.OuterHtml;
        }

        public string Modals(string content)
        {
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(map["body"]);
            if (body == null) return node.OuterHtml;
            var modalBlock = modals;
            var menu = node.SelectSingleNode(map["menu_container"]);
            if (menu == null)
            {
                var mnutxt = menus;
                modalBlock = modalBlock.Replace(map["menu"], mnutxt);
            }
            var bodyText = string.Concat(modalBlock, Environment.NewLine, body.InnerHtml);
            body.InnerHtml = bodyText;

            return node.OuterHtml;

        }

        public string Scripts(string content)
        {
            const string fmt = "<script name='{0}' src='{1}'></script>";
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(map["body"]);
            if (body == null) return node.OuterHtml;
            var inner = body.InnerHtml + Environment.NewLine;
            var builder = new StringBuilder(inner);
            foreach (var item in namesMap)
            {
                var line = string.Format(fmt, item.Key, item.Value);
                builder.AppendLine(line);
            }
            body.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }


        public string Shell(string content)
        {
            const string find = "//*[@id='dv-subcontent-home']";
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(map["body"]);
            if (body == null) return node.OuterHtml;
            var shellBlock = shells;
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

        private static readonly string heading = Properties.Resources.base_account_heading;
        private static readonly string menus = Properties.Resources.base_account_menus;
        private static readonly string modals = Properties.Resources.base_account_modals;
        private static readonly string shells = Properties.Resources.base_account_shell;

        private static readonly Dictionary<string, string> map = new()
        {
            { "body", "//body" },
            { "head", "//head" },
            { "menu", "<!-- Menu -->" },
            { "menu_container", "//*[@id='menu-container']" },
            { "title", "//head/title" }
        };

        private static readonly Dictionary<string, string> namesMap = new()
        {
            { "handler_js", "/js/handler.js" },
            { "my-account-common", "/js/my-account-common.js" },
            { "re-authenticate-js", "/js/re-authenticate-js.js" },
            { "my-account-validate", "/js/my-account-validate.js" }
        };
    }
}
