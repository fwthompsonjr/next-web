﻿using Microsoft.AspNetCore.Mvc;
using next.core.interfaces;
using next.web.core.extensions;
using next.web.core.util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace next.web.Controllers
{
    public partial class HomeController
    {

        [HttpGet("/subscription-result")]
        [SuppressMessage(
            "Major Code Smell",
            "S6967:ModelState.IsValid should be called in controller actions",
            Justification = "Model is not needed for standard http-get pages")]
        public async Task<IActionResult> UserLevelLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            const string landing = "subscription-result";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var page = await GetLanding(session, landing, sts, id);
            return page;
        }

        protected async Task<IActionResult> GetLanding(
            ISession session, string landing, string? sts, string? id)
        {
            var sanitizer = AppContainer.GetSanitizer("payment-confirmation");
            var content = await GetAuthenicatedPage(session, "blank");
            content = sanitizer.Sanitize(content);
            var remote = await GetRemoteContent(landing, sts, id);
            if (!string.IsNullOrEmpty(remote))
            {
                content = Inject("//div[@name='main-content']", remote, content);
                content = await AppendStatus(content);
                await UpdateUserSession(session, apiwrapper);
            }
            return GetResult(content);
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static async Task UpdateUserSession(ISession session, IApiWrapper? wrapper = null)
        {
            try
            {
                var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
                if (api == null) return;
                // reset session variables
                var usr = session.GetContextUser();
                if (usr == null) return;
                await usr.Save(session, api, wrapper);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string Inject(string xpath, string remote, string content)
        {
            const string findtarget = "//*[@id='payment-card-content']";
            var remoteNode = remote.ToHtml().DocumentNode;
            var node = content.ToHtml().DocumentNode;
            var source = remoteNode.SelectSingleNode(xpath);
            if (source == null) return node.OuterHtml;
            var target = node.SelectSingleNode(findtarget);
            if (target == null) return node.OuterHtml;
            target.InnerHtml = source.OuterHtml;
            return node.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static async Task<string> GetRemoteContent(string landing, string? sts, string? id)
        {
            var target = GetRemoteUri(landing, sts, id);
            if (!Uri.IsWellFormedUriString(target, UriKind.Absolute)) { return string.Empty; }
            try
            {
                using var client = new HttpClient();
                var html = await client.GetStringAsync(target);
                return html;
            }
            catch
            {
                return string.Empty;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string GetRemoteUri(string landing, string? sts, string? id)
        {
            const string remoteServer = "http://api.legallead.co";

            var baseAddress = $"{remoteServer}/{landing}";
            if (!string.IsNullOrEmpty(sts))
            {
                baseAddress = $"{baseAddress}?sts={sts}";
            }
            if (!string.IsNullOrEmpty(id) && baseAddress.Contains('?'))
            {
                baseAddress = $"{baseAddress}&id={id}";
            }
            if (!string.IsNullOrEmpty(id) && !baseAddress.Contains('?'))
            {
                baseAddress = $"{baseAddress}?id={id}";
            }
            return baseAddress;
        }
    }
}
