using HtmlAgilityPack;
using next.core.entities;
using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.core.implementations
{
    internal class UserPermissionsMapper : UserProfileMapper, IUserPermissionsMapper
    {
        public override async Task<string> Map(IPermissionApi api, UserBo user, string source)
        {
            var permissions = await GetPermissions(api, user);
            var stateList = await GetStates(api, user);
            var counties = await GetCounties(api, user);
            if (permissions == null || stateList == null || counties == null) { return source; }

            var document = GetDocument(source);
            if (document == null) return source;
            DeselectCheckBoxes(document);
            DeselectRadioButtons(document);
            var userlevel = GetUserLevel(permissions);
            SelectRadioButton(document, userlevel);
            var isStateActive = GetPermissionStatus("Setting.State.Subscriptions.Active", permissions);
            var isCountyActive = GetPermissionStatus("Setting.State.County.Subscriptions.Active", permissions);
            if (isStateActive) MapStateSelections(permissions, stateList, document);
            if (isCountyActive) MapCountySelections(permissions, counties, document);
            return document.DocumentNode.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void MapCountySelections(List<ContactPermissionResponse> permissions, List<ContactUsStateCountyResponse> counties, HtmlDocument document)
        {
            var index = GetPermissionValue("Setting.State.County.Subscriptions", permissions);
            if (!string.IsNullOrEmpty(index))
            {
                var indexes = index.Split(',').Select(s => s.Trim()).ToArray();
                var selected = counties.FindAll(x => x.IsActive && indexes.Contains(x.IndexCode));
                var transforms = selected.Select(x => new UiItemMap
                {
                    Find = string.Format(xfmt, x.UiControlId),
                    ReplaceWith = chkd
                }).ToList();
                transforms.ForEach(c => c.ApplyTransform(document));
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void MapStateSelections(List<ContactPermissionResponse> permissions, List<ContactUsStateResponse> states, HtmlDocument document)
        {
            var index = GetPermissionValue("Setting.State.Subscriptions", permissions);
            var comparison = StringComparer.OrdinalIgnoreCase;
            if (!string.IsNullOrEmpty(index))
            {
                var indexes = index.Split(',').Select(s => s.Trim()).ToArray();
                var selected = states.FindAll(x => x.IsActive && indexes.Contains(x.ShortName.Trim(), comparison));
                var transforms = selected.Select(x => new UiItemMap
                {
                    Find = string.Format(xfmt, x.UiControlId),
                    ReplaceWith = chkd
                }).ToList();
                transforms.ForEach(c => c.ApplyTransform(document));
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void DeselectCheckBoxes(HtmlDocument document)
        {
            const string xpath = "//*[@id=\"dv-subcontent-permissions-row-03\"]//input[@type=\"checkbox\"]";
            var elements = document.DocumentNode.SelectNodes(xpath);
            if (elements == null) return;
            var list = elements.ToList();
            list.ForEach(e =>
            {
                var attributes = e.Attributes.ToList();
                var find = attributes.Find(a => a.Name.Equals(chkd, StringComparison.OrdinalIgnoreCase));
                if (find != null) { e.Attributes.Remove(find); }
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void DeselectRadioButtons(HtmlDocument document)
        {
            const string xpath = "//*[@id=\"permissions-subscription-group\"]//input[@type=\"radio\"]";
            var elements = document.DocumentNode.SelectNodes(xpath);
            if (elements == null) return;
            var list = elements.ToList();
            list.ForEach(e =>
            {
                var attributes = e.Attributes.ToList();
                var find = attributes.Find(a => a.Name.Equals(chkd, StringComparison.OrdinalIgnoreCase));
                if (find != null) { e.Attributes.Remove(find); }
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void SelectRadioButton(HtmlDocument document, string groupName)
        {
            const string fmt = "permissions-subscription-{0}-radio";
            var radioIndex = string.Format(fmt, groupName.ToLower());
            var xpath = string.Format(xfmt, radioIndex);
            var element = document.DocumentNode.SelectSingleNode(xpath);
            if (element == null) return;
            element.SetAttributeValue(chkd, chkd);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static bool GetPermissionStatus(string find, List<ContactPermissionResponse> permissions)
        {
            var value = GetPermissionValue(find, permissions);
            if (string.IsNullOrWhiteSpace(value)) { return false; }
            return value.Equals("True");
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string GetUserLevel(List<ContactPermissionResponse> permissions)
        {
            const string accountLevel = "Account.Permission.Level";
            const string fallback = "Guest";
            var value = GetPermissionValue(accountLevel, permissions);
            if (string.IsNullOrWhiteSpace(value)) { return fallback; }
            return value;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string GetPermissionValue(string find, List<ContactPermissionResponse> permissions)
        {
            var item = permissions.Find(x => x.KeyName.Equals(find));
            if (item == null) { return string.Empty; }
            return item.KeyValue;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static async Task<List<ContactPermissionResponse>?> GetPermissions(IPermissionApi api, UserBo user)
        {
            const string landing = "user-permissions-list";
            var response = await api.Get(landing, user);
            if (response.StatusCode != 200) return null;
            return ObjectExtensions.TryGet<List<ContactPermissionResponse>>(response.Message);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static async Task<List<ContactUsStateCountyResponse>?> GetCounties(IPermissionApi api, UserBo user)
        {
            const string landing = "user-us-county-list";
            var response = await api.Get(landing, user);
            if (response.StatusCode != 200) return null;
            return ObjectExtensions.TryGet<List<ContactUsStateCountyResponse>>(response.Message);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static async Task<List<ContactUsStateResponse>?> GetStates(IPermissionApi api, UserBo user)
        {
            const string landing = "user-us-state-list";

            var response = await api.Get(landing, user);
            if (response.StatusCode != 200) return null;
            return ObjectExtensions.TryGet<List<ContactUsStateResponse>>(response.Message);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private sealed class UiItemMap
        {
            public string NodeType { get; set; } = "input";
            public string Find { get; set; } = string.Empty;
            public string ReplaceWith { get; set; } = string.Empty;

            public void ApplyTransform(HtmlDocument document)
            {
                if (!IsValidXPath(Find)) return;
                if (string.IsNullOrEmpty(NodeType)) return;
                var element = document.DocumentNode.SelectSingleNode(Find);
                if (element == null) return;
                if (!string.IsNullOrEmpty(ReplaceWith))
                {
                    element.SetAttributeValue(chkd, chkd);
                    return;
                }
                var attributes = element.Attributes.ToList();
                var find = attributes.Find(a => a.Name.Equals(chkd, StringComparison.OrdinalIgnoreCase));
                if (find == null) return;
                element.Attributes.Remove(find);
            }
        }

        private const string chkd = "checked";
        private const string xfmt = "//*[@id=\"{0}\"]";
    }
}