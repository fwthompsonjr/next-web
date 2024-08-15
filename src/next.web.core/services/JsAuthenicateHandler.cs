using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.services
{
    [ExcludeFromCodeCoverage(Justification = "Integration only. Might cover at later date.")]
    internal class JsAuthenicateHandler : IJsHandler
    {
        protected readonly IPermissionApi _api;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style",
            "IDE0290:Use primary constructor",
            Justification = "Primary constructor violates rule CS9136")]
        public JsAuthenicateHandler(IPermissionApi api)
        {
            _api = api;
        }
        public virtual string Name => "form-login";
        public async virtual Task<FormSubmissionResponse> Submit(FormSubmissionModel model)
        {
            var response = FormResponses.GetDefault(model.FormName) ?? new();
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                var appsubmission = await Submit(model, failureMessage);
                response.MapResponse(appsubmission);
                if (response.StatusCode != 200) return response;
                response.Message = "Login completed";
                response.RedirectTo = AppContainer.PostLoginPage ?? "/my-acount/home";
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        public async virtual Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session, IApiWrapper? wrapper = null)
        {
            var response = FormResponses.GetDefault(model.FormName) ?? new();
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                var user = GetUser();
                var data = (model.Payload ?? string.Empty).ToInstance<FormLoginModel>();
                if (data == null || user.Applications == null || user.Applications.Length == 0) return response;
                var appsubmission = await Submit(model, failureMessage);
                response.MapResponse(appsubmission);
                if (response.StatusCode != 200) return response;
                response.Message = "Login completed";
                response.RedirectTo = AppContainer.PostLoginPage ?? "/my-acount/home";
                var token = appsubmission.Message.ToInstance<AccessTokenBo>();
                var app = user.Applications[0];
                var userbo = new UserContextBo
                {
                    AppId = app.Id,
                    AppName = app.Name,
                    UserName = data.UserName,
                    Token = token?.AccessToken,
                    RefreshToken = token?.RefreshToken,
                    Expires = token?.Expires
                };
                user = userbo.ToUserBo();
                var userId = await user.GetUserId(_api);
                userbo.UserId = userId;
                userbo.Save(session);
                await userbo.SaveMail(session, _api);
                await userbo.SaveHistory(session, _api);
                await userbo.SaveRestriction(session, _api);
                await userbo.SaveSearchPurchases(session, _api);
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        private async Task<ApiResponse> Submit(FormSubmissionModel model, string failureMessage)
        {
            var user = GetUser();
            var formName = model.FormName ?? string.Empty;
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
            var matchedName = HomeFormNames.Find(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(matchedName)) return failed;
            var data = (model.Payload ?? string.Empty).ToInstance<FormLoginModel>();
            if (data == null) return failed;
            var obj = new { data.UserName, data.Password };
            var loginResponse = await _api.Post("login", obj, user) ?? failed;
            return loginResponse;
        }

        internal static readonly List<string> HomeFormNames = ["form-login", "form-register"];
        protected static UserBo GetUser()
        {
            var apps = new List<ApiContext>
                    {
                        new(){ Id = "d6450506-3479-4c02-92c7-de59f6e7091e", Name = "legallead.permissions.api" }
                    }.ToArray();
            return new()
            {
                Applications = apps
            };
        }
    }
}
