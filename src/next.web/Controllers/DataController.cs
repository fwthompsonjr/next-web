using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.util;

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
            if (location == null || !securepg.Contains(location.Location, StringComparer.OrdinalIgnoreCase)) return BadRequest();

            var authenicated = IsSessionAuthenicated(HttpContext.Session);
            response.StatusCode = authenicated ? 200 : 408;
            response.Message = authenicated ? "Session authorized" : "Error session invalid";
            response.RedirectTo = authenicated ? "" : "/home";
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
            var filter = session.RetrieveHistoryFilter();
            filter.Index = recordId.StatusId;
            filter.County = recordId.CountyName;
            session.UpdateHistoryFilter(filter);
            response.StatusCode = 200;
            response.Message = $"Please apply filter. Status: {recordId.StatusId}. County: {recordId.CountyName}";
            return Json(response);
        }
    }
}
