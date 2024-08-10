using HtmlAgilityPack;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace next.web.core.extensions
{
    internal static class StatusExtensions
    {
        public static async Task AppendStatus(this ISession session, IPermissionApi api, HtmlDocument document, bool isAlternateLayout = false)
        {
            const string dash = " - ";
            var bo = session.GetContextUser();
            var id = await session.RetrieveIdentity(api);
            var restricted = await session.RetrieveRestriction(api);
            if (bo == null) { return; }
            var node = document.DocumentNode;
            var body = node.SelectSingleNode("//body");
            var table = node.SelectSingleNode("//table[@automationid='user-status-table']");
            if (body == null || table != null) { return; }
            var builder = new StringBuilder(body.InnerHtml);
            builder.AppendLine();
            builder.AppendLine(StatusTemplate);
            body.InnerHtml = builder.ToString();
            table = node.SelectSingleNode("//table[@automationid='user-status-table']");
            if (table == null) return;
            AppendTableCss(isAlternateLayout, table);
            var islocked = restricted?.IsLocked ?? true;
            var sts = islocked ? "ERROR" : "OK";
            var userName = string.IsNullOrWhiteSpace(id.UserName) ? dash : id.UserName;
            var fullName = string.IsNullOrWhiteSpace(id.FullName) ? dash : id.FullName;
            var commonName = fullName.Equals(dash) ? userName : fullName;
            Dictionary<string, string> headers = new() {
                { "current-dt", DateTime.UtcNow.ToString("MM-dd-yyyy")},
                { "user-level", string.IsNullOrWhiteSpace(id.Role) ? dash : id.Role },
                { "user-name", commonName },
                { "user-status", sts }
            };
            headers.Keys.ToList().ForEach(keyname =>
            {
                var find = $"//span[@id='spn-status-{keyname}']";
                var span = node.SelectSingleNode(find);
                if (span != null) span.InnerHtml = headers[keyname];
            });

        }

        private static void AppendTableCss(bool isAlternateLayout, HtmlNode? table)
        {
            if (table == null) return;
            var tablecss = isAlternateLayout ? "alternate-view" : "default-view";
            var attr = table.Attributes.FirstOrDefault(x => x.Name == "class");
            if (attr != null)
            {
                var clss = attr.Value.Split(' ').ToList();
                if (!clss.Contains(tablecss)) clss.Add(tablecss);
                attr.Value = string.Join(" ", clss);
            }
        }

        private static string StatusTemplate => _statusTemplate ??= GetTemplate();
        private static string? _statusTemplate;
        private static string GetTemplate()
        {
            return Properties.Resources.status_menu;
        }
    }
}
