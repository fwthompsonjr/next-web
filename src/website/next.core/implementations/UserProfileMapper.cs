using HtmlAgilityPack;
using next.core.entities;
using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Xml.XPath;

namespace next.core.implementations
{
    internal class UserProfileMapper : IUserProfileMapper
    {
        public virtual async Task<string> Map(IPermissionApi api, UserBo user, string source)
        {
            var profile = await GetProfile(api, user);
            if (profile == null) return source;
            var identity = await GetIdentity(api, user);

            var document = GetDocument(source);
            if (document == null) return source;
            var replacements = new[]
            {
                new { node = "", find = "//*[@id=\"tbx-profile-first-name\"]", replace = GetProfileItem(profile, "Name", "First")},
                new { node = "", find = "//*[@id=\"tbx-profile-last-name\"]", replace = GetProfileItem(profile, "Name", "Last")},
                new { node = "", find = "//*[@id=\"tbx-profile-company\"]", replace = GetProfileItem(profile, "Name", "Company")},
                new { node = "textarea", find = "//*[@id=\"tbx-profile-mailing-address\"]", replace = GetProfileItem(profile, "Address", "Mailing")},
                new { node = "textarea", find = "//*[@id=\"tbx-profile-billing-address\"]", replace = GetProfileItem(profile, "Address", "Billing")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-01\"]", replace = GetProfileItem(profile, "Email", "Personal")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-02\"]", replace = GetProfileItem(profile, "Email", "Business")},
                new { node = "", find = "//*[@id=\"tbx-profile-email-03\"]", replace = GetProfileItem(profile, "Email", "Other")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-01\"]", replace = GetProfileItem(profile, "Phone", "Personal")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-02\"]", replace = GetProfileItem(profile, "Phone", "Business")},
                new { node = "", find = "//*[@id=\"tbx-profile-phone-03\"]", replace = GetProfileItem(profile, "Phone", "Other")},
                new { node = "div", find = "//*[@id=\"account-user-name\"]", replace = identity.UserName},
                new { node = "", find = "//*[@id=\"account-password-username\"]", replace = identity.UserName},
                new { node = "div", find = "//*[@id=\"account-user-email\"]", replace = identity.Email},
                new { node = "div", find = "//*[@id=\"account-create-date\"]", replace = identity.Created},
                new { node = "div", find = "//*[@id=\"account-role\"]", replace = identity.Role},
                new { node = "div", find = "//*[@id=\"account-description\"]", replace = identity.RoleDescription},
                new { node = "span", find = "//*[@id=\"account-text-item-user-name\"]", replace = identity.UserName},
                new { node = "span", find = "//*[@id=\"account-text-item-user-level\"]", replace = identity.Role},
            };
            foreach (var item in replacements)
            {
                if (!IsValidXPath(item.find)) continue;
                var element = document.DocumentNode.SelectSingleNode(item.find);
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
            return document.DocumentNode.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static async Task<ContactIdentity> GetIdentity(IPermissionApi api, UserBo user)
        {
            const string landing = "get-contact-identity";
            try
            {
                var payload = new object();
                var response = await api.Post(landing, payload, user);
                if (response.StatusCode != 200) return new();
                var data = ObjectExtensions.TryGet<ContactIdentity>(response.Message);
                if (data == null) return new();
                return data;
            }
            catch
            {
                return new();
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static async Task<List<ContactProfileItem>?> GetProfile(IPermissionApi api, UserBo user)
        {
            const string landing = "get-contact-detail";

            var payload = new { RequestType = string.Empty };
            var response = await api.Post(landing, payload, user);
            if (response.StatusCode != 200) return null;
            var data = ObjectExtensions.TryGet<List<ContactProfileResponse>>(response.Message);
            if (data == null || !data.Any()) return null;
            var list = new List<ContactProfileItem>();
            var address = data.ToList().Find(a => a.ResponseType.Equals("Address"))?.Data;
            var names = data.ToList().Find(a => a.ResponseType.Equals("Name"))?.Data;
            var emails = data.ToList().Find(a => a.ResponseType.Equals("Email"))?.Data;
            var phones = data.ToList().Find(a => a.ResponseType.Equals("Phone"))?.Data;
            if (!string.IsNullOrEmpty(address))
            {
                var t1 = ObjectExtensions.TryGet<List<ContactAddress>>(address);
                t1?.ForEach(t => list.Add(t.ToItem()));
            }
            if (!string.IsNullOrEmpty(names))
            {
                var t2 = ObjectExtensions.TryGet<List<ContactName>>(names);
                t2?.ForEach(t => list.Add(t.ToItem()));
            }
            if (!string.IsNullOrEmpty(emails))
            {
                var t3 = ObjectExtensions.TryGet<List<ContactEmail>>(emails);
                t3?.ForEach(t => list.Add(t.ToItem()));
            }
            if (!string.IsNullOrEmpty(phones))
            {
                var t4 = ObjectExtensions.TryGet<List<ContactPhone>>(phones);
                t4?.ForEach(t => list.Add(t.ToItem()));
            }
            return list;
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
        protected static bool IsValidXPath(string xpath)
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

        [ExcludeFromCodeCoverage(Justification = "Private member to be tested from public method")]
        protected static HtmlDocument? GetDocument(string source)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(source);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}