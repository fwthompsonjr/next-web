using Microsoft.AspNetCore.Http;
using next.core.entities;
using next.core.implementations;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.XPath;

namespace next.web.core.services
{
    using UPM = UserPermissionsMapper;
    public class AccountMapService : IAccountMapService
    {
        internal IApiWrapper? Api { get; set; } = null;

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

        public async Task<string> Transform(string html, ISession session)
        {
            if (Api == null) return html;
            var identity = await Api.Post("get-contact-identity", new object(), session);
            var states = await Api.Get("user-us-state-list", session);
            var counties = await Api.Get("user-us-county-list", session);
            var permissions = await Api.Get("user-permissions-list", session);
            var profile = await Api.Post("profile-get-contact-detail", new { RequestType = "" }, session);
            html = MapPermissions(html, permissions, states, counties);
            html = MapProfile(html, profile, identity);
            return html;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member to be tested from public method")]
        private static string MapPermissions(string html, ApiAnswer? permissions, ApiAnswer? states = null, ApiAnswer? counties = null)
        {
            if (permissions == null || permissions.StatusCode != 200) return html;
            if (states == null || states.StatusCode != 200) return html;
            if (counties == null || counties.StatusCode != 200) return html;
            var data = permissions.Message.ToInstance<List<ContactPermissionResponse>>() ?? [];
            var stateList = states.Message.ToInstance<List<ContactUsStateResponse>>() ?? [];
            var countyList = counties.Message.ToInstance<List<ContactUsStateCountyResponse>>() ?? [];
            var doc = html.ToHtml();
            var node = doc.DocumentNode;
            UPM.DeselectCheckBoxes(doc);
            UPM.DeselectRadioButtons(doc);
            var userlevel = UPM.GetUserLevel(data);
            UPM.SelectRadioButton(doc, userlevel);
            var isStateActive = UPM.GetPermissionStatus("Setting.State.Subscriptions.Active", data);
            var isCountyActive = UPM.GetPermissionStatus("Setting.State.County.Subscriptions.Active", data);
            if (isStateActive) UPM.MapStateSelections(data, stateList, doc);
            if (isCountyActive) UPM.MapCountySelections(data, countyList, doc);
            return node.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member to be tested from public method")]
        private static string MapProfile(string html, ApiAnswer? profile, ApiAnswer? identity = null)
        {
            if (profile == null || profile.StatusCode != 200) return html;
            if (identity == null || identity.StatusCode != 200) return html;
            var idx = identity.Message.ToInstance<ContactIdentity>() ?? new();
            var data = profile.Message.ToInstance<List<ContactProfileResponse>>();
            if (data == null || data.Count == 0) return html;
            var list = new List<ContactProfileItem>();
            var address = data.Find(a => a.ResponseType.Equals("Address"))?.Data.ToInstance<List<ContactAddress>>();
            var names = data.Find(a => a.ResponseType.Equals("Name"))?.Data.ToInstance<List<ContactName>>();
            var emails = data.Find(a => a.ResponseType.Equals("Email"))?.Data.ToInstance<List<ContactEmail>>();
            var phones = data.ToList().Find(a => a.ResponseType.Equals("Phone"))?.Data.ToInstance<List<ContactPhone>>();
            address?.ForEach(a => list.Add(a.ToItem()));
            names?.ForEach(a => list.Add(a.ToItem()));
            emails?.ForEach(a => list.Add(a.ToItem()));
            phones?.ForEach(a => list.Add(a.ToItem()));
            var replacements = new[]
            {
                new { node = "", find = "//*[@id=\"tbx-profile-first-name\"]", replace = GetProfileItem(list, "Name", "First")},
                new { node = "", find = "//*[@id=\"tbx-profile-last-name\"]", replace = GetProfileItem(list, "Name", "Last")},
                new { node = "", find = "//*[@id=\"tbx-profile-company\"]", replace = GetProfileItem(list, "Name", "Company")},
                new { node = "textarea", find = "//*[@id=\"tbx-profile-mailing-address\"]", replace = GetProfileItem(list, "Address", "Mailing")},
                new { node = "textarea", find = "//*[@id=\"tbx-profile-billing-address\"]", replace = GetProfileItem(list, "Address", "Billing")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-01\"]", replace = GetProfileItem(list, "Email", "Personal")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-02\"]", replace = GetProfileItem(list, "Email", "Business")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-03\"]", replace = GetProfileItem(list, "Email", "Other")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-01\"]", replace = GetProfileItem(list, "Phone", "Personal")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-02\"]", replace = GetProfileItem(list, "Phone", "Business")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-03\"]", replace = GetProfileItem(list, "Phone", "Other")},
                new { node = "div", find = "//*[@id=\"account-user-name\"]", replace = idx.UserName},
                new { node = "", find = "//*[@id=\"account-password-username\"]", replace = idx.UserName},
                new { node = "div", find = "//*[@id=\"account-user-email\"]", replace = idx.Email},
                new { node = "div", find = "//*[@id=\"account-create-date\"]", replace = idx.Created},
                new { node = "div", find = "//*[@id=\"account-role\"]", replace = idx.Role},
                new { node = "div", find = "//*[@id=\"account-description\"]", replace = idx.RoleDescription},
                new { node = "span", find = "//*[@id=\"account-text-item-user-name\"]", replace = idx.UserName},
                new { node = "span", find = "//*[@id=\"account-text-item-user-level\"]", replace = idx.Role},
            };
            var doc = html.ToHtml();
            var node = doc.DocumentNode;
            foreach (var item in replacements)
            {
                if (!IsValidXPath(item.find)) continue;
                var element = node.SelectSingleNode(item.find);
                if (element == null) continue;
                if (string.IsNullOrEmpty(item.node))
                {
                    element.SetAttributeValue("value", item.replace);
                }
                else
                {
                    element.InnerHtml = item.replace;
                }
            }
            return node.OuterHtml;
        }



        [ExcludeFromCodeCoverage(Justification = "Private member to be tested from public method")]
        private static string GetProfileItem(List<ContactProfileItem> profile, string category, string code)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            var item = profile.Find(x => x.Category.Equals(category, comparison) &&
                x.Code.Equals(code, comparison));
            if (item == null) return string.Empty;
            return item.Data.Trim();
        }


        [ExcludeFromCodeCoverage(Justification = "Private member to be tested from public method")]
        private static bool IsValidXPath(string xpath)
        {
            if (string.IsNullOrEmpty(xpath)) return false;
            try
            {
                XPathExpression.Compile(xpath);
                return true;
            }
            catch
            {
                return false;
            }
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
