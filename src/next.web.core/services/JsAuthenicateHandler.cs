using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;

namespace next.web.core.services
{
    internal class JsAuthenicateHandler : IJsHandler
    {
        public JsAuthenicateHandler(
            IHttpContextAccessor http,
            IAuthorizedUserService service,
            IPermissionApi api)
        {

        }
        public string Name => "form-login";
        public virtual FormSubmissionResponse Submit(FormSubmissionModel model)
        {
            var response = FormResponses.GetDefault(model.FormName);
            try
            {
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
