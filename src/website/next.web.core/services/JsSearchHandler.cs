using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.core.entities;
using next.core.interfaces;
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

        public static string AppendHarrisJpOptions(string content)
        {
            const string indx = "32190";
            const string find_option = "//option[@dat-county-index='{0}'][@value='{1}']";
            const string find = "//*[@id='cbo-search-dynamic-0']";
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var cbo = node.SelectSingleNode(find);
            if (cbo == null) return node.OuterHtml;
            foreach (var item in HarrisJpMap)
            {
                var query = string.Format(find_option, indx, item.Key);
                var nde = cbo.SelectSingleNode(query);
                if (nde != null) continue;
                var append = doc.CreateElement("option");
                append.Attributes.Add("value", item.Key);
                append.Attributes.Add("dat-row-index", "0");
                append.Attributes.Add("dat-row-name", "Search Type");
                append.Attributes.Add("dat-county-index", indx);
                append.Attributes.Add("style", "display: none");
                append.InnerHtml = item.Value;
                cbo.AppendChild(append);
            }
            return node.OuterHtml;
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
                var detail = GetHarrisJpParameter(search.Details);
                search.Details.Clear();
                search.Details.Add(detail);
            }
        }

        private static BeginSearchDetail GetHarrisJpParameter(List<BeginSearchDetail> selection)
        {
            var searchValue = "0";
            var searchText = HarrisJpMap[searchValue];
            const string name = "Court Selection";
            var item = selection.FirstOrDefault();
            if (item != null && HarrisJpMap.TryGetValue(item.Value, out var userSelected))
            {
                searchValue = item.Value;
                searchText = userSelected;
            }
            return new() { Name = name, Text = searchText, Value = searchValue };
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

        private static readonly Dictionary<string, string> HarrisJpMap = new()
        {
            { "0", "All JP Courts" },
            { "1", "All JP Civil Courts" },
            { "2", "All JP Criminal Courts" }
        };
    }
}