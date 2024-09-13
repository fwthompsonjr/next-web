using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using next.web.Services;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.services
{
    [ExcludeFromCodeCoverage(Justification = "Integration only. Might cover at later date.")]
    internal class JsSearchHandler(IPermissionApi permissionApi) : BaseJsHandler
    {
        private readonly IPermissionApi api = permissionApi;

        public override string Name => "frm-search";

        public override Task<FormSubmissionResponse> Submit(FormSubmissionModel model)
        {
            var formName = model.FormName ?? string.Empty;
            var response = FormResponses.GetDefault(formName) ?? new();
            return Task.FromResult(response);
        }

        public async override Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session, IApiWrapper? wrapper = null)
        {
            var formName = model.FormName ?? string.Empty;
            var json = model.Payload ?? string.Empty;
            var response = FormResponses.GetDefault(formName) ?? new();
            var issearch = SearchForms.Contains(formName);
            try
            {
                const string failureMessage = "Unable to parse form submission data.";
                if (wrapper == null && Wrapper != null) wrapper = Wrapper;
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

                var appsubmission =
                    wrapper == null ?
                    await api.Post(address, js, user) :
                    ApiWrapper.MapTo(await wrapper.Post(address, js, session));

                response.MapResponse(appsubmission);
                if (appsubmission.StatusCode != 200) response.RedirectTo = ""; // stay on same page
                // trigger reload page on 200 event
                var prefix = formName.Split('-')[^1];
                if (prefix.Equals("search") && appsubmission.StatusCode == 200)
                {
                    // reset all user cache objects
                    await userbo.SaveMail(session, api, wrapper);
                    await userbo.SaveHistory(session, api, wrapper);
                    await userbo.SaveRestriction(session, api, wrapper);
                    await userbo.SaveSearchPurchases(session, api, wrapper);
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
            if (mapped is BeginSearchModel search)
            {
                MapCountyPayload(search);
                return search;
            }
            return mapped ?? new();
        }
        private static void MapCountyPayload(BeginSearchModel search)
        {
            var oic = StringComparison.OrdinalIgnoreCase;
            if (search.County.Name.Equals("harris-jp", oic))
            {
                search.Details.Clear();
                search.Details.Add(new() { Name = "Court Selection", Text = "All JP Courts", Value = "0" });
            }
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