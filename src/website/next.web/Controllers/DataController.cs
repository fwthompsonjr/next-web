﻿using Microsoft.AspNetCore.Mvc;
using next.core.entities;
using next.core.interfaces;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using next.web.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace next.web.Controllers
{
    [Route("/data")]
    public class DataController(IApiWrapper wrapper, IViolationService violations) : BaseController(wrapper, violations)
    {
        private readonly IServiceProvider? provider = AppContainer.ServiceProvider;

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
            const string loginPage = "form-login";
            var session = HttpContext.Session;
            var response = FormResponses.GetDefault(null);
            if (!ModelState.IsValid || !model.Validate(Request))
            {
                return BadRequest();
            }
            var handler = provider?.GetKeyedService<IJsHandler>(model.FormName);
            if (handler == null) return Json(response);
            response = await handler.Submit(model, session, apiwrapper);
            if (model.FormName == loginPage && response.StatusCode != 200)
            {
                AppendViolation(model, response);
                if (IsViolation(HttpContext))
                {
                    response.StatusCode = 408;
                    response.RedirectTo = "/home";
                }
            }
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
            var message = await session.FetchMailBody(apiwrapper, recordId);
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
            var thing = (await session.RetrievePurchases(api, apiwrapper)).Find(x => (x.ReferenceId ?? "").Equals(location.Id));
            if (thing != null && !string.IsNullOrEmpty(thing.ExternalId)) location.Id = thing.ExternalId;
            var isDownloadComplete = false;
            try
            {
                var verification = await DownloadVerification(session, response, location);
                isDownloadComplete = verification.StatusCode == 200;
                return Json(verification);
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
                _ = await session.RetrieveMail(api, apiwrapper);
            }
            if (location.Name == ResetCacheNames[1])
            {
                session.Remove(SessionKeyNames.UserSearchPurchases);
                session.Remove(SessionKeyNames.UserSearchHistory);
                _ = await session.RetrievePurchases(api, apiwrapper);
                _ = await session.RetrieveHistory(api, apiwrapper);
            }
            if (usrbo != null && location.Name == ResetCacheNames[2])
            {
                session.Remove(SessionKeyNames.UserIdentity);
                await usrbo.SaveUserIdentity(session, api, apiwrapper);
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

        private void AppendViolation(FormSubmissionModel request, FormSubmissionResponse response)
        {
            var rejectCodes = new[] { 400, 401 };
            var payload = request.Payload ?? string.Empty;
            var data = payload.ToInstance<FormLoginModel>();
            if (data == null || !rejectCodes.Contains(response.StatusCode)) return;
            var email = data.UserName;
            var context = HttpContext;
            base.AppendViolation(context, email);
        }

        [ExcludeFromCodeCoverage]
        private async Task<FormSubmissionResponse> DownloadVerification(ISession session, FormSubmissionResponse response, FetchIntentRequest location)
        {
            var remote = await apiwrapper.Post("make-search-purchase", location, session);
            if (remote == null) return response;
            if (remote.StatusCode != 200)
            {
                response.StatusCode = remote.StatusCode;
                response.Message = remote.Message;
                return response;
            }
            session.SetString(SessionKeyNames.UserDownloadResponse, remote.Message);
            response.StatusCode = remote.StatusCode;
            response.Message = "Download authorized";
            response.RedirectTo = "/download";
            return response;
        }

        private static readonly List<string> ResetCacheNames = ["correspondence", "history", "identity"];
    }
}
