using HtmlAgilityPack;
using next.core.entities;
using next.core.interfaces;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace next.core.implementations
{
    internal class UserRestrictionMapper : IUserRestrictionMapper
    {
        public async Task<string> Map(IPermissionApi api, UserBo user, string source)
        {
            var content = await MapRestriction(api, user, source);
            return content;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static async Task<string> MapRestriction(IPermissionApi api, UserBo user, string source)
        {
            var payload = new { Id = Guid.NewGuid().ToString(), Name = "legallead.permissions.api" };
            var response = await api.Post("search-get-restriction", payload, user);
            if (response.StatusCode != 200) return source;
            var json = response.Message;
            var result = JsonConvert.DeserializeObject<MySearchRestrictions>(json);
            if (result == null || !result.IsLocked.GetValueOrDefault()) return source;

            var document = ToDocument(source);
            return TransformAlert(document, result);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static HtmlDocument ToDocument(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string TransformAlert(HtmlDocument document, MySearchRestrictions restrictions)
        {
            const string dvquery = "//*[@id='dv-restriction-alert']";
            const string spanquery = "//*[@id='span-restriction-alert-message']";
            var node = document.DocumentNode;
            var dv = node.SelectSingleNode(dvquery);
            var span = node.SelectSingleNode(spanquery);
            if (dv == null || span == null) return node.OuterHtml;
            span.InnerHtml = restrictions.Reason ?? "Usage limit exceeded.";
            dv.RemoveClass("d-none");
            return node.OuterHtml;
        }
    }
}