using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.services;
using next.web.core.util;
using next.web.Models;
using System.Diagnostics;

namespace next.web.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Code Analysis",
        "S6967:ModelState.IsValid should be called in controller actions",
        Justification = "Model is not needed for standard http-get pages")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Code Analysis",
        "S6934:A Route attribute should be added to the controller when a route template is specified at the action level",
        Justification = "Controller route is defined in definition of partial class")]
    public partial class HomeController
    {
        [HttpGet("/payment-result")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Code Analysis",
            "S6967:ModelState.IsValid should be called in controller actions",
            Justification = "Model validations are performed against payload.")]
        public async Task<IActionResult> PaymentLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            const string landing = "payment-result";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var page = await GetLanding(session, landing, sts, id);
            return page;
        }

        [HttpPost("/payment-fetch-intent")]
        public async Task<IActionResult> FetchIntent([FromBody] FetchIntentRequest request)
        {
            const string landing = "payment-fetch-intent";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var url = GetRemoteUri(landing, null, null);
            var response = await GetIntent(_intentSvc, url, request);
            if (response == null) return Redirect("/error");
            return Json(new { clientSecret = response.ClientSecret });
        }

        [HttpGet("/download")]
        public async Task<IActionResult> Download()
        {
            try
            {
                var session = HttpContext.Session;
                if (!IsSessionAuthenicated(session)) return Redirect("/home");
                var wrapper = _sessionStringWrapper ?? new SessionStringWrapper(session);
                var payload = wrapper.GetString(SessionKeyNames.UserDownloadResponse);
                if (string.IsNullOrEmpty(payload)) return Redirect("/error");
                var data = payload.ToInstance<DownloadJsResponse>();
                if (data == null || string.IsNullOrEmpty(data.Content)) return Redirect("/error");
                var keys = new List<KeyValuePair<string, string>>
                {
                    new("//*[@id='spn-download-external-id']", data.ExternalId ?? " - "),
                    new("//*[@id='spn-download-description']", data.Description ?? " - "),
                    new("//*[@id='spn-download-date']", data.CreateDate ?? " - "),
                };
                var content = await GetAuthenicatedPage(session, "blank");
                var sanity = AppContainer.GetSanitizer("download");
                content = sanity.Sanitize(content);
                content = ContentSanitizerDownload.AppendContext(content, keys);
                content = await AppendStatus(content);
                return GetResult(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Redirect("/error");
            }
        }


        [HttpGet("/download-file")]
        public IActionResult DownloadFile()
        {
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var session = HttpContext.Session;
            var wrapper = _sessionStringWrapper ?? new SessionStringWrapper(session);
            var isFileDownload = false;
            try
            {
                if (!IsSessionAuthenicated(session)) return Unauthorized();
                var payload = wrapper.GetString(SessionKeyNames.UserDownloadResponse);
                if (string.IsNullOrEmpty(payload)) return BadRequest();
                var data = payload.ToInstance<DownloadJsResponse>();
                if (data == null || string.IsNullOrEmpty(data.Content)) return BadRequest();
                var bytes = Convert.FromBase64String(data.Content);
                var name = data.FileName();
                isFileDownload = true;
                return new FileContentResult(bytes, contentType) { FileDownloadName = name };
            }
            catch
            {
                isFileDownload = false;
                return BadRequest();
            }
            finally
            {
                if (isFileDownload) wrapper.Remove(SessionKeyNames.UserDownloadResponse);
            }
        }
        private async static Task<FetchIntentResponse?> GetIntent(
            IFetchIntentService service,
            string url,
            FetchIntentRequest request)
        {
            var js = request.ToJsonString();
            var response = await service.GetIntent(url, js);
            if (string.IsNullOrEmpty(response)) return null;
            return response.ToInstance<FetchIntentResponse>();
        }
    }
}