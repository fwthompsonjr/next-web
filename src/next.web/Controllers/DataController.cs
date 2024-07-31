using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/data")]
    public class DataController : Controller
    {
        private readonly IServiceProvider? provider;

        public DataController()
        {
            provider = AppContainer.ServiceProvider;
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
    }
}
