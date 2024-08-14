using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.services
{
    [ExcludeFromCodeCoverage(Justification = "Integration only. Might cover at later date.")]
    internal class JsSearchHandler : IJsHandler
    {
        private readonly IPermissionApi api;
        public JsSearchHandler(IPermissionApi permissionApi)
        {
            api = permissionApi;
        }
        public string Name => "frm-search";

        public Task<FormSubmissionResponse> Submit(FormSubmissionModel model)
        {
            var formName = model.FormName ?? string.Empty;
            var response = FormResponses.GetDefault(formName) ?? new();
            return Task.FromResult(response);
        }

        public async Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session)
        {
            var formName = model.FormName ?? string.Empty;
            var json = model.Payload ?? string.Empty;
            var response = FormResponses.GetDefault(formName) ?? new();
            var issearch = SearchForms.Contains(formName);
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                response.Message = failureMessage;
                var user = session.GetUser();
                var userbo = session.GetContextUser();
                if (userbo == null ||
                    user == null ||
                    user.Applications == null ||
                    user.Applications.Length == 0 ||
                    string.IsNullOrEmpty(formName) ||
                    string.IsNullOrEmpty(json) ||
                    !issearch
                    ) return response;
                var js = JsSearchSubmissionHelper.Refine(MapPayload(formName, json));
                if (!AddressMap.TryGetValue(formName, out var address) || string.IsNullOrEmpty(address)) return response; // redirect to login
                var appsubmission = await api.Post(address, js, user);
                response.MapResponse(appsubmission);
                if (appsubmission.StatusCode != 200) response.RedirectTo = ""; // stay on same page
                // trigger reload page on 200 event
                var prefix = formName.Split('-')[^1];
                if (prefix.Equals("search") && appsubmission.StatusCode == 200)
                {
                    // reset all user cache objects
                    await userbo.SaveMail(session, api);
                    await userbo.SaveHistory(session, api);
                    await userbo.SaveRestriction(session, api);
                    await userbo.SaveSearchPurchases(session, api);
                }
                response.RedirectTo = prefix switch
                {
                    "history" => "/search/history",
                    "purchases" => "/search/purchases",
                    "search" => "/search/active",
                    _ => ""
                };
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
        private static readonly List<string> SearchForms =
        [
            "frm-search",
            "frm-search-history",
            "frm-search-purchases",
            "frm-search-preview",
            "frm-search-invoice"
        ];

        private static object MapPayload(string formName, object payload)
        {
            var js = Convert.ToString(payload);
            if (js == null) return new();
            var typeMap = PayloadMap[formName];
            var mapped = JsonConvert.DeserializeObject(js, typeMap);
            return mapped ?? new();
        }

        private static readonly Dictionary<string, Type> PayloadMap = new()
        {
            { "frm-search", typeof(BeginSearchModel) },
            { "frm-search-history", typeof(ContactAddress[]) },
            { "frm-search-purchases", typeof(ContactPhone[]) },
            { "frm-search-preview", typeof(SearchPreviewModel) },
            { "frm-search-invoice", typeof(GenerateInvoiceModel) }
        };

        private static readonly Dictionary<string, string> AddressMap = new()
        {
            { "frm-search", "search-begin" },
            { "frm-search-history", "profile-edit-contact-address" },
            { "frm-search-purchases", "profile-edit-contact-phone" },
            { "frm-search-preview", "search-get-preview" },
            { "frm-search-invoice", "search-get-invoice" }
        };
    }
}