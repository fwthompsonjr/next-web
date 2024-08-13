using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.util;
using next.web.Models;
using System.Diagnostics;

namespace next.web.Controllers
{
    [Route("/data")]
    public class DataController : BaseController
    {
        private readonly IServiceProvider? provider;

        public DataController()
        {
            provider = AppContainer.ServiceProvider;
        }

        [HttpPost("session-check")]
        public IActionResult Check(FormSubmissionModel model)
        {
            string[] securepg = ["my-account", "search", "correspondence"];
            var response = FormResponses.GetDefault(null);
            if (!ModelState.IsValid || !model.Validate(Request)) return BadRequest();
            if (string.IsNullOrEmpty(model.Payload)) return BadRequest();
            var location = model.Payload.ToInstance<FormLocationModel>();
            if (location == null) return BadRequest();
            var authenicated = IsSessionAuthenicated(HttpContext.Session);
            if (!securepg.Contains(location.Location, StringComparer.OrdinalIgnoreCase))
            {
                response.StatusCode = authenicated ? 200 : 204;
                response.RedirectTo = string.Empty;
                response.Message = authenicated ? "Session authorized" : "Error session invalid";
            }
            else
            {
                response.StatusCode = authenicated ? 200 : 408;
                response.RedirectTo = authenicated ? "" : "/home";
            }
            return Json(response);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit(FormSubmissionModel model)
        {
            var response = FormResponses.GetDefault(null);
            if (!ModelState.IsValid || !model.Validate(Request))
            {
                return BadRequest();
            }
            var handler = provider?.GetKeyedService<IJsHandler>(model.FormName);
            if (handler == null) return Json(response);
            response = await handler.Submit(model, this.HttpContext.Session);
            return Json(response);
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch(FormSubmissionModel model)
        {
            var session = HttpContext.Session;
            var response = FormResponses.GetDefault(null);
            var authenicated = IsSessionAuthenicated(session);
            var user = session.GetUser();
            var api = provider?.GetService<IPermissionApi>();
            var errResponse = Json(response);
            if (user == null ||
                api == null ||
                !authenicated ||
                !ModelState.IsValid ||
                !model.Validate(Request) ||
                string.IsNullOrWhiteSpace(model.Payload))
            {
                return errResponse;
            }
            var recordId = model.Payload ?? string.Empty;
            var message = await session.FetchMailBody(api, recordId);
            response.StatusCode = 200;
            response.Message = message;
            return Json(response);
        }

        [HttpPost("filter-status")]
        public IActionResult Filter(FormSubmissionModel model)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var session = HttpContext.Session;
            var response = FormResponses.GetDefault(null);
            var authenicated = IsSessionAuthenicated(session);
            var user = session.GetUser();
            var api = provider?.GetService<IPermissionApi>();
            var errResponse = Json(response);
            if (user == null ||
                api == null ||
                !authenicated ||
                !ModelState.IsValid ||
                !model.Validate(Request) ||
                string.IsNullOrWhiteSpace(model.Payload))
            {
                return errResponse;
            }
            var recordId = model.Payload.ToInstance<FormStatusFilter>();
            if (recordId == null) { return errResponse; }
            var filterType = SearchFilterNames.History;
            if (recordId.Heading.Contains("active", oic)) filterType = SearchFilterNames.Active;
            if (recordId.Heading.Contains("purchase", oic)) filterType = SearchFilterNames.Purchases;
            var filterName = Enum.GetName(filterType) ?? "history";
            var filter = session.RetrieveFilter(filterType);
            filter.Index = recordId.StatusId;
            filter.County = recordId.CountyName;
            session.UpdateFilter(filter, filterType);
            response.StatusCode = 200;
            response.Message = $"Please apply filter: {filterName}. Status: {recordId.StatusId}. County: {recordId.CountyName}";
            return Json(response);
        }

        [HttpPost("download-verify")]
        public async Task<IActionResult> VerifyDownload(FormSubmissionModel model)
        {
            var session = HttpContext.Session;
            var response = FormResponses.GetDefault(null);
            if (!ModelState.IsValid || !model.Validate(Request)) return BadRequest();
            if (string.IsNullOrEmpty(model.Payload)) return BadRequest();
            var location = model.Payload.ToInstance<FetchIntentRequest>();
            if (location == null) return BadRequest();
            var authenicated = IsSessionAuthenicated(session);
            response.StatusCode = authenicated ? 200 : 408;
            response.RedirectTo = authenicated ? "" : "/home";
            if (!authenicated) return Json(response);
            // contact api to get download response
            var user = session.GetUser();
            var api = provider?.GetService<IPermissionApi>();
            if (api == null || user == null) return StatusCode(503);
            var thing = (await session.RetrievePurchases(api)).Find(x => (x.ReferenceId ?? "").Equals(location.Id));
            if (thing != null && !string.IsNullOrEmpty(thing.ExternalId)) location.Id = thing.ExternalId;
            var isDownloadComplete = false;
            try
            {
                var remote = await api.Post("make-search-purchase", location, user);
                if (remote == null) return Json(response);
                if (remote.StatusCode != 200)
                {
                    response.StatusCode = remote.StatusCode;
                    response.Message = remote.Message;
                    return Json(response);
                }
                session.SetString(SessionKeyNames.UserDownloadResponse, remote.Message);
                response.StatusCode = remote.StatusCode;
                response.Message = "Download authorized";
                response.RedirectTo = "/download";
                isDownloadComplete = true;
                return Json(response);
            }
            catch (Exception ex)
            {
                isDownloadComplete = false;
                Debug.WriteLine("Download retrieval: {0}, id: {1}", isDownloadComplete, location.Id);
                Debug.WriteLine("Download error: {0}}", ex.Message);
                return Json(response);
            }
            finally
            {
                Debug.WriteLine("Download retrieval: {0}, id: {1}", isDownloadComplete, location.Id);
                await RevertDownload(api, location, user);
            }
        }

        [HttpPost("download-file-status")]
        public IActionResult DownloadCompleted()
        {
            var session = HttpContext.Session;
            var response = FormResponses.GetDefault(null);
            response.StatusCode = 404;
            response.Message = "No content found";
            response.RedirectTo = "";
            if (!IsSessionAuthenicated(session)) return Json(response);
            var keyvalue = session.GetString(SessionKeyNames.UserDownloadResponse);
            if (!string.IsNullOrEmpty(keyvalue))
            {
                response.StatusCode = 200;
                response.Message = "Item download is in progress";
                response.RedirectTo = "/search/history";
            }
            return Json(response);
        }

        [HttpPost("reset-cache")]
        public async Task<IActionResult> ResetCache(FormSubmissionModel model)
        {
            var session = HttpContext.Session;
            var response = FormResponses.GetDefault(null);
            var api = provider?.GetService<IPermissionApi>();
            if (!ModelState.IsValid || !model.Validate(Request)) return BadRequest();
            if (api == null || string.IsNullOrEmpty(model.Payload)) return BadRequest();
            var location = model.Payload.ToInstance<CacheUpdateRequest>();
            if (location == null || !ResetCacheNames.Contains(location.Name)) return BadRequest();
            var authenicated = IsSessionAuthenicated(session);
            response.StatusCode = authenicated ? 200 : 408;
            response.RedirectTo = authenicated ? "" : "/home";
            if (!authenicated) return Json(response);
            var usrbo = session.GetContextUser();
            if (location.Name == ResetCacheNames[0])
            {
                session.Remove(SessionKeyNames.UserMailbox);
                _ = await session.RetrieveMail(api);
            }
            if (location.Name == ResetCacheNames[1])
            {
                session.Remove(SessionKeyNames.UserSearchPurchases);
                session.Remove(SessionKeyNames.UserSearchHistory);
                _ = await session.RetrievePurchases(api);
                _ = await session.RetrieveHistory(api);
            }
            if (usrbo != null && location.Name == ResetCacheNames[2])
            {
                session.Remove(SessionKeyNames.UserIdentity);
                await usrbo.SaveUserIdentity(session, api);
            }
            return Json(response);

        }

        private static async Task RevertDownload(IPermissionApi api, FetchIntentRequest request, UserBo user)
        {
            try
            {
                await api.Post("reset-download", request, user);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        private static readonly List<string> ResetCacheNames = ["correspondence", "history", "identity"];
    }
}
