using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using next.web.Services;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.services
{
    [ExcludeFromCodeCoverage(Justification = "Integration only. Might cover at later date.")]
    internal class JsAuthenicateHandler : BaseJsHandler
    {
        protected readonly IPermissionApi _api;

        [SuppressMessage("Style",
            "IDE0290:Use primary constructor",
            Justification = "Primary constructor violates rule CS9136")]
        public JsAuthenicateHandler(IPermissionApi api)
        {
            _api = api;
        }
        public override string Name => "form-login";
        public async override Task<FormSubmissionResponse> Submit(FormSubmissionModel model)
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

        public async override Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session, IApiWrapper? wrapper = null)
        {
            var response = FormResponses.GetDefault(model.FormName) ?? new();
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                if (wrapper == null && Wrapper != null) wrapper = Wrapper;
                var user = GetUser();
                var data = (model.Payload ?? string.Empty).ToInstance<FormLoginModel>();
                if (data == null || user.Applications == null || user.Applications.Length == 0) return response;
                var appsubmission = await Submit(model, failureMessage, session, wrapper);
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
                var userId = wrapper == null ?
                    await user.GetUserId(_api) :
                    await session.GetUserId(wrapper);
                userbo.UserId = userId;
                userbo.Save(session);
                await userbo.SaveMail(session, _api, wrapper);
                await userbo.SaveHistory(session, _api, wrapper);
                await userbo.SaveRestriction(session, _api, wrapper);
                await userbo.SaveSearchPurchases(session, _api, wrapper);
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        private async Task<ApiResponse> Submit(FormSubmissionModel model, string failureMessage, ISession? session = null, IApiWrapper? wrapper = null)
        {
            const string address = "login";
            var user = GetUser();
            var formName = model.FormName ?? string.Empty;
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
            var matchedName = HomeFormNames.Find(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(matchedName)) return failed;
            var data = (model.Payload ?? string.Empty).ToInstance<FormLoginModel>();
            if (data == null) return failed;
            var obj = new { data.UserName, data.Password };
            var loginResponse =
                wrapper == null || session == null ?
                await _api.Post(address, obj, user) :
                ApiWrapper.MapTo(await wrapper.Post(address, obj, session, user.ToJsonString()));
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
