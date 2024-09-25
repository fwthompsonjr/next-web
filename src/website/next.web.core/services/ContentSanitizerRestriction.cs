using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using next.core.entities;
using next.web.core.extensions;

namespace next.web.core.services
{
    internal class ContentSanitizerRestriction : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(ViewContent);
            if (doc == null) return content;
            AppendMenu(doc);
            UnhideMenuOptions(doc);
            return doc.DocumentNode.OuterHtml;
        }

        public static async Task<string> AppendDetail(string content, IApiWrapper wrapper, ISession session)
        {
            const string zero = "~0";
            const string one = "~1";
            const string findrows = "//*[@id='detail-table']/tbody/tr";
            const string nbrs = "N0";
            var doc = GetDocument(content);
            if (doc == null) return content;
            var node = doc.DocumentNode;
            var payload = new { Id = Guid.NewGuid().ToString(), Name = "legallead.permissions.api" };
            var restriction = await wrapper.Post("search-get-restriction", payload, session);
            if (restriction == null || restriction.StatusCode != 200) return content;
            var rows = node.SelectNodes(findrows).ToList();
            if (rows.Count == 0) return content;
            var data = restriction.Message.ToInstance<MySearchRestrictions>() ?? new();
            var isRestriction = data.IsLocked.GetValueOrDefault();
            var statusText = isRestriction ? "LOCKED" : "OK";
            var cls = isRestriction ? "text-danger" : "text-success";
            rows.ForEach(row =>
            {
                var original = row.InnerHtml;
                var index = rows.IndexOf(row);
                switch (index)
                {
                    case 0:
                        row.InnerHtml = original.Replace(zero, statusText);
                        var span = row.SelectNodes("td").LastOrDefault()?.SelectSingleNode("span");
                        AppendClass(span, cls);
                        break;
                    case 1:
                        row.InnerHtml = original.Replace(zero, data.Reason ?? " - ");
                        break;
                    case 3:
                        row.InnerHtml = original
                            .Replace(zero, data.ThisMonth.GetValueOrDefault().ToString(nbrs))
                            .Replace(one, data.MaxPerMonth.GetValueOrDefault().ToString(nbrs));
                        break;
                    case 4:
                        row.InnerHtml = original
                            .Replace(zero, data.ThisYear.GetValueOrDefault().ToString(nbrs))
                            .Replace(one, data.MaxPerYear.GetValueOrDefault().ToString(nbrs));
                        break;
                    default:
                        break;
                }
            });
            if (isRestriction) return node.OuterHtml;
            var link = node.SelectSingleNode("//tfoot//a");
            if (link == null) return node.OuterHtml;
            link.SetAttributeValue("href", "javascript:void()");
            link.SetAttributeValue("class", "link-secondary");
            return node.OuterHtml;
        }

        private static void AppendClass(HtmlNode? span, string cls)
        {
            const string attribute = "class";
            if (span == null) return;
            var current = span.Attributes.FirstOrDefault(a => a.Name == attribute);
            if (current == null)
            {
                span.Attributes.Add(attribute, cls);
                return;
            }
            var items = current.Value.Split(' ').ToList();
            items.Add(cls);
            items = items.Distinct().ToList();
            current.Value = string.Join(" ", items);
        }

        public static async Task UpgradeRequest(IApiWrapper wrapper, ISession session)
        {
            var payload = new { Id = Guid.NewGuid().ToString(), Name = "legallead.permissions.api" };
            var restriction = await wrapper.Post("search-get-restriction", payload, session);
            if (restriction == null || restriction.StatusCode != 200) return;
            var data = restriction.Message.ToInstance<MySearchRestrictions>() ?? new();
            var isRestriction = data.IsLocked.GetValueOrDefault();
            if (!isRestriction) return;
            _ = await wrapper.Post("search-extend-restriction", payload, session);
        }

        private static void UnhideMenuOptions(HtmlDocument doc)
        {
            const char space = ' ';
            const string dnone = "d-none";
            const string cls = "class";
            const string alt = "alternate-border";
            const string findnames = "//div[@name='left-menu-account']";
            const string findsidemenu = "//*[@id='app-side-menu-border']";
            var node = doc.DocumentNode;
            var dvs = node.SelectNodes(findnames).ToList();
            dvs.ForEach(d =>
            {
                var attr = d.Attributes.FirstOrDefault(x => x.Name == cls);
                if (attr != null)
                {
                    var clss = attr.Value.Split(space).ToList();
                    clss.Remove(dnone);
                    attr.Value = string.Join(" ", clss);
                }
            });
            var menu = node.SelectSingleNode(findsidemenu);
            if (menu == null) return;
            var attr = menu.Attributes.FirstOrDefault(x => x.Name == cls);
            if (attr == null)
            {
                menu.Attributes.Add(cls, alt);
                return;
            }
            var clss = attr.Value.Split(space).ToList();
            if (clss.Contains(alt)) return;
            clss.Add(alt);
            attr.Value = string.Join(" ", clss);
        }

        private static void AppendMenu(HtmlDocument doc)
        {
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode("//body");
            if (body == null) return;
            var txt = string.Concat(
                TheMenu,
                Environment.NewLine,
                body.InnerHtml);
            body.InnerHtml = txt;
        }

        private static string? pageContent;
        private static string GetViewContent => Properties.Resources.user_restriction_view;
        private static string ViewContent => pageContent ??= GetViewContent;

        private static string? bseMenu;
        private static string GetTheMenu => Properties.Resources.base_menu;
        private static string TheMenu => bseMenu ??= GetTheMenu;
    }
}