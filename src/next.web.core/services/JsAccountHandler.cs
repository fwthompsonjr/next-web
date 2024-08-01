using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;

namespace next.web.core.services
{
    internal class JsAccountHandler(IPermissionApi api) : JsAuthenicateHandler(api)
    {
        public override string Name => "permissions-subscription-group";
        

        public override async Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session)
        {
            var response = FormResponses.GetDefault(model.FormName) ?? new();
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                var user = session.GetUser();
                if (user == null || user.Applications == null || user.Applications.Length == 0) return response;
                var appsubmission = await Submit(model, user, failureMessage);
                response.MapResponse(appsubmission);
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        private async Task<ApiResponse> Submit(FormSubmissionModel model, UserBo user, string failureMessage)
        {
            var formName = model.FormName ?? string.Empty;
            var payload = model.Payload ?? string.Empty;
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
            if (!Name.Equals(formName, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(payload)) return failed;
            var data = JsonConvert.DeserializeObject(payload);
            if (data == null) return failed;
            var resp = await _api.Post("login", data, user) ?? failed;
            return resp;
        }




        protected static readonly List<string> ProfileForms =
        [
            "frm-profile-personal",
            "frm-profile-address",
            "frm-profile-phone",
            "frm-profile-email"
        ];
        protected static readonly List<string> PermissionForms = new()
        {
            "permissions-subscription-group",
            "permissions-discounts",
            "form-change-password"
        };

    }
}