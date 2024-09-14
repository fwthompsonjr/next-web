using next.core.entities;
using next.core.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using next.web.Services;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.services
{
    internal class JsAccountHandler(IPermissionApi api) : JsAuthenicateHandler(api)
    {
        public override string Name => "permissions-subscription-group";


        public override async Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session, IApiWrapper? wrapper = null)
        {
            var response = FormResponses.GetDefault(model.FormName) ?? new();
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                if (wrapper == null && Wrapper != null) wrapper = Wrapper;
                var user = session.GetUser();
                if (user == null || user.Applications == null || user.Applications.Length == 0) return response;
                var appsubmission = await Submit(model, user, failureMessage, session, wrapper);
                response.MapResponse(appsubmission);
                var formName = model.FormName ?? string.Empty;
                if (!IsFormNameValid(formName)) return response; // redirect to login
                return MapApiResponse(session, response, appsubmission, formName);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        [ExcludeFromCodeCoverage]
        private static FormSubmissionResponse MapApiResponse(ISession session, FormSubmissionResponse response, ApiResponse appsubmission, string formName)
        {
            if (appsubmission.StatusCode != 200) response.RedirectTo = ""; // stay on same page
            if (formName == "permissions-subscription-group" && appsubmission.StatusCode == 200)
            {
                // add details to session object
                // instruct script to redirect to invoice controller
                var obj = appsubmission.Message.ToInstance<PermissionChangedResponse>() ?? new();
                session.Save(obj);
                response.StatusCode = 200;
                response.Message = "permession request received. generating invoice.";
                response.RedirectTo = "/invoice/permissions";
                return response;
            }
            _ = AppContainer.AddressMap.TryGetValue(formName, out var address);
            address ??= string.Empty;
            // trigger reload page on 200 event
            var prefix = address.Split('-')[0];
            response.RedirectTo = prefix switch
            {
                "profile" => "/my-account/profile",
                "permissions" => "/my-account/permissions",
                _ => "/my-account/home"
            };
            return response;
        }

        [ExcludeFromCodeCoverage]
        private async Task<ApiResponse> Submit(
            FormSubmissionModel model,
            UserBo user,
            string failureMessage,
            ISession session,
            IApiWrapper? wrapper)
        {
            var formName = model.FormName ?? string.Empty;
            var payload = model.Payload ?? string.Empty;
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
            if (!IsFormNameValid(formName) || string.IsNullOrEmpty(payload)) return failed;
            var isPermissions = AppContainer.PermissionForms.Contains(formName, StringComparer.OrdinalIgnoreCase);
            IJsAccountHandler handler = isPermissions switch
            {
                true => new JsPermissionChange(_api, user, session, wrapper),
                _ => new JsProfileChange(_api, user, formName, session, wrapper),
            };
            var resp = await handler.Submit(payload, failureMessage);
            return resp;
        }

        [ExcludeFromCodeCoverage]
        private bool IsFormNameValid(string formName)
        {
            if (Name.Equals(formName, StringComparison.OrdinalIgnoreCase)) return true;
            if (AppContainer.AddressMap.ContainsKey(formName)) return true;
            return false;
        }

        private interface IJsAccountHandler
        {
            Task<ApiResponse> Submit(string payload, string failureMessage);
        }

        [ExcludeFromCodeCoverage]
        private sealed class JsPermissionChange(
            IPermissionApi permissionApi,
            UserBo user,
            ISession session,
            IApiWrapper? wrapper = null) : IJsAccountHandler
        {
            private readonly IPermissionApi _permissionApi = permissionApi;
            private readonly UserBo _userBo = user;
            private readonly ISession _session = session;
            private readonly IApiWrapper? _wrapper = wrapper;

            public async Task<ApiResponse> Submit(string payload, string failureMessage)
            {
                var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
                var submission = payload.ToInstance<UserPermissionChangeRequest>();
                if (submission == null || !submission.CanSubmit) return failed;
                if (!AppContainer.AddressMap.TryGetValue(submission.SubmissionName, out var landing)) return failed;
                var js = MapPayload(submission);
                if (string.IsNullOrEmpty(landing)) return failed;
                var response =
                    _wrapper == null ?
                    await _permissionApi.Post(landing, js, _userBo) :
                    ApiWrapper.MapTo(await _wrapper.Post(landing, js, _session));
                return response;
            }
            private static object MapPayload(UserPermissionChangeRequest request)
            {
                try
                {
                    var changeType = request.SubmissionName;
                    switch (changeType)
                    {
                        case "Subscription":
                            return request.Subscription.ToInstance<ContactLevel>() ?? new();
                        case "Discounts":
                            var selections = request.Discounts.ToInstance<DiscountChoice[]>();
                            if (selections == null) return new();
                            var discountRequest = new { Choices = selections };
                            return discountRequest;
                        case "Changes":
                            return request.Changes.ToInstance<ContactChangePassword>() ?? new();
                        default:
                            return new();
                    }
                }
                catch (Exception)
                {
                    return new();
                }
            }

        }
        [ExcludeFromCodeCoverage]
        private sealed class JsProfileChange(
            IPermissionApi permissionApi,
            UserBo user,
            string formName,
            ISession session,
            IApiWrapper? wrapper = null) : IJsAccountHandler
        {
            private readonly IPermissionApi _permissionApi = permissionApi;
            private readonly UserBo _userBo = user;
            private readonly string _formName = formName;
            private readonly ISession _session = session;
            private readonly IApiWrapper? _wrapper = wrapper;
            public async Task<ApiResponse> Submit(string payload, string failureMessage)
            {
                var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
                if (!PayloadMap.TryGetValue(_formName, out var changeType)) return failed;
                var submission = payload.ToInstance(changeType);
                if (submission == null) return failed;
                if (!AppContainer.AddressMap.TryGetValue(_formName, out var landing)) return failed;
                if (string.IsNullOrEmpty(landing)) return failed;
                var response = _wrapper == null ?
                    await _permissionApi.Post(landing, submission, _userBo) :
                    ApiWrapper.MapTo(await _wrapper.Post(landing, submission, _session));
                return response;
            }
            private static readonly Dictionary<string, Type> PayloadMap = new()
            {
                { "frm-profile-personal", typeof(ContactName[]) },
                { "frm-profile-address", typeof(ContactAddress[]) },
                { "frm-profile-phone", typeof(ContactPhone[]) },
                { "frm-profile-email", typeof(ContactEmail[]) }
            };
        }
    }
}