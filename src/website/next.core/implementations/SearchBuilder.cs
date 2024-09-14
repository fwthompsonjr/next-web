using HtmlAgilityPack;
using next.core.entities;
using next.core.interfaces;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace next.core.implementations
{
    internal class SearchBuilder : ISearchBuilder
    {
        private const string PageName = "application-state-configuration";
        private static readonly ContentParser Parser = new();
        private readonly IPermissionApi _api;
        private string? _jsConfiguration;

        public SearchBuilder(IPermissionApi api)
        {
            _api = api;
        }

        public string GetHtml()
        {
            const string parentXpath = "//*[@id='dv-subcontent-search']";
            const string fmt = "<html><body>{0}</body></html>";
            var search = Configuration()?.ToList();

            var doc = new HtmlDocument();
            var frame = string.Format(fmt, Properties.Resources.mysearch_search_frame);
            doc.LoadHtml(frame);
            var table = doc.DocumentNode.SelectSingleNode($"//*[@id='table-search']");
            var tbody = doc.CreateElement("tbody");
            BuildTableBody(doc, tbody, search);
            table.AppendChild(tbody);
            var tempHtml = doc.DocumentNode.OuterHtml;
            var html = Parser.BeautfyHTML(tempHtml);
            doc = new HtmlDocument();
            doc.LoadHtml(html);
            var element = doc.DocumentNode.SelectSingleNode(parentXpath);
            if (element == null) return tempHtml;
            return element.OuterHtml;
        }

        public StateSearchConfiguration[]? GetConfiguration()
        {
            return Configuration();
        }

        protected StateSearchConfiguration[]? Configuration()
        {
            if (string.IsNullOrEmpty(_jsConfiguration))
            {
                var response = TryRequest();
                if (response == null || response.StatusCode != 200) return null;
                if (string.IsNullOrEmpty(response.Message)) return null;
                _jsConfiguration = response.Message;
                var mapped = ObjectExtensions.TryGet<List<StateSearchConfiguration>>(_jsConfiguration);
                if (mapped.Any())
                {
                    _jsConfiguration = JsonConvert.SerializeObject(mapped, Formatting.None);
                }
            }
            return ObjectExtensions.TryGet<List<StateSearchConfiguration>>(_jsConfiguration).ToArray();
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private ApiResponse TryRequest()
        {
            var failed = new ApiResponse { Message = "Unable to connect to remote source", StatusCode = 500 };
            try
            {
                return _api.Get(PageName).Result;
            }
            catch (Exception ex)
            {
                failed.Message = ex.Message;
                return failed;
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void BuildTableBody(HtmlDocument doc, HtmlNode node, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1 };
            var rows = new[] {
                "State",
                "County",
                "Dynamic-0",
                "Dynamic-1",
                "Dynamic-2",
                "Dynamic-3",
                "Dynamic-4",
                "Dynamic-5",
                "Dynamic-6",
                "Start Date",
                "End Date"}.ToList();
            rows.ForEach(r =>
            {
                // add error rows to collection
                var rwname = r.Replace(' ', '-').ToLower();
                var isParameterRow = r.StartsWith("Dynamic-");
                var tr = doc.CreateElement("tr");
                var lbl = doc.CreateElement("label");
                var td1 = doc.CreateElement("td");
                var td2 = doc.CreateElement("td");
                tr.Attributes.Add("id", $"tr-search-{rwname}");
                lbl.Attributes.Add("class", "m-1 form-label text-secondary");
                lbl.InnerHtml = r;
                td1.AppendChild(lbl);
                if (r.EndsWith("Date"))
                {
                    var tbx = doc.CreateElement("input");
                    var tbname = r.Replace(" ", string.Empty).ToLower();
                    tbx.Attributes.Add("id", $"tbx-search-{tbname}");
                    tbx.Attributes.Add("name", "search-field");
                    tbx.Attributes.Add("type", "date");
                    tbx.Attributes.Add("class", "form-control");
                    td2.AppendChild(tbx);
                }
                else
                {
                    var cbo = doc.CreateElement("select");
                    var cboname = r.Replace(" ", string.Empty).ToLower();
                    cbo.Attributes.Add("id", $"cbo-search-{cboname}");
                    cbo.Attributes.Add("name", "search-field");
                    cbo.Attributes.Add("class", "form-control");
                    var indx = rows.IndexOf(r);
                    if (indexes.Contains(indx)) PopulateOptions(indx, cbo, configurations);
                    td2.AppendChild(cbo);
                }
                tr.AppendChild(td1);
                tr.AppendChild(td2);
                if (isParameterRow)
                {
                    tr.Attributes.Add("style", "display: none");
                    tr.Attributes.Add("name", "tr-search-dynamic");
                    var rowId = Convert.ToInt32(r.Split('-')[1]);
                    PopulateParameters(rowId, tr, configurations);
                }
                node.AppendChild(tr);
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void AppendCounties(HtmlNode cbo, List<StateSearchConfiguration>? config)
        {
            if (config == null) return;
            var nde = cbo.OwnerDocument.CreateElement("option");
            nde.Attributes.Add("value", "0");
            nde.Attributes.Add("dat-county-index", "");
            nde.Attributes.Add("dat-state-index", "");
            nde.InnerHtml = "- select -";
            cbo.AppendChild(nde);
            var temp = config.SelectMany(s => s.Counties).ToList();

            temp.Sort((a, b) =>
            {
                var aa = (a.StateCode ?? string.Empty).CompareTo(b.StateCode ?? string.Empty);
                if (aa != 0) return aa;
                return (a.Name ?? string.Empty).CompareTo(b.Name ?? string.Empty);
            });

            temp.ForEach(s =>
            {
                var hasCase = (s.Data?.CaseSearchTypes ?? Array.Empty<CaseSearchModel>()).Length > 0;
                var dropDownCount = 0;
                if (s.Data != null) { dropDownCount = s.Data.DropDowns.Length; }
                var id = s.Index.ToString();
                var abrev = s.Name?.ToLower() ?? id;
                var itm = cbo.OwnerDocument.CreateElement("option");
                itm.Attributes.Add("value", id);
                itm.Attributes.Add("dat-state-index", s.StateCode?.ToLower() ?? id);
                itm.Attributes.Add("dat-county-index", abrev);
                itm.Attributes.Add("dat-has-case-search", hasCase ? "true" : "false");
                itm.Attributes.Add("dat-parameter-count", dropDownCount.ToString());
                itm.Attributes.Add("style", "display: none");
                itm.InnerHtml = s.Name ?? id;
                cbo.AppendChild(itm);
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void AppendStates(HtmlNode cbo, List<StateSearchConfiguration>? config)
        {
            if (config == null) return;
            var nde = cbo.OwnerDocument.CreateElement("option");
            nde.Attributes.Add("value", "0");
            nde.Attributes.Add("dat-state-index", "");
            nde.InnerHtml = "- select -";
            cbo.AppendChild(nde);
            config.ForEach(s =>
            {
                var id = (config.IndexOf(s) + 1).ToString();
                var abrev = s.ShortName?.ToLower() ?? id;
                var itm = cbo.OwnerDocument.CreateElement("option");
                itm.Attributes.Add("value", id);
                itm.Attributes.Add("dat-state-index", abrev);
                itm.InnerHtml = s.Name ?? id;
                cbo.AppendChild(itm);
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void PopulateOptions(int indx, HtmlNode cbo, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1 };
            if (configurations == null) return;
            if (!indexes.Contains(indx)) return;
            if (indx == 0) AppendStates(cbo, configurations);
            if (indx == 1) AppendCounties(cbo, configurations);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void PopulateParameters(int indx, HtmlNode tr, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1, 2, 3, 4, 5, 6 };
            if (configurations == null) return;
            if (!indexes.Contains(indx)) return;
            var cbo = FindComboBox(tr);
            if (cbo == null) return;
            /* find all configurations,
             * county having count of drop-downs
             * greater than zero */
            if (indx != indexes[^1])
            {
                AppendDropDownValues(indx, cbo, configurations);
                return;
            }
            AppendCaseTypeValues(cbo, configurations);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static HtmlNode? FindComboBox(HtmlNode tr)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (tr.ChildNodes.Count != 2) return null;
            if (tr.ChildNodes[1].ChildNodes.Count < 1) return null;
            var element = tr.ChildNodes[1].ChildNodes[0];
            if (!element.Name.Equals("select", oic)) return null;
            return element;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void AppendDropDownValues(int indx, HtmlNode cbo, List<StateSearchConfiguration> configurations)
        {
            var temp = configurations
            .SelectMany(s => s.Counties)
            .Where(w => w.Data != null && w.Data.DropDowns.Length > 0)
            .ToList();
            if (!temp.Any()) return;
            CreateDefaultCountyOption(cbo);
            temp.ForEach(t =>
            {
                if (t.Data != null && t.Data.DropDowns.Any())
                {
                    var dd = t.Data.DropDowns;
                    foreach (var dd2 in dd) { dd2.CountyId = t.Index; }
                }
            });
            var dropdowns = temp.Select(s =>
            {
                if (s.Data == null || !s.Data.DropDowns.Any() || s.Data.DropDowns.Length < indx)
                    return null;
                return s.Data.DropDowns;
            }).Where(w => w != null).ToList();
            dropdowns.ForEach(s =>
            {
                if (s != null)
                {
                    foreach (var item in s)
                    {
                        if (item.Id != indx) continue;
                        var members = item.Members.ToList();
                        members.Sort((a, b) => a.Name.CompareTo(b.Name));
                        var countyIndex = item.CountyId.GetValueOrDefault();
                        var countyNumber = countyIndex.ToString();
                        var rowLabel = item.Name ?? $"Parameter {indx + 1}";
                        foreach (var (child, itm) in from child in members
                                                     where item.IsDisplayed.GetValueOrDefault()
                                                     let itm = cbo.OwnerDocument.CreateElement("option")
                                                     select (child, itm))
                        {
                            itm.Attributes.Add("value", child.Id.ToString());
                            itm.Attributes.Add("dat-row-index", indx.ToString());
                            itm.Attributes.Add("dat-row-name", rowLabel);
                            itm.Attributes.Add("dat-county-index", countyNumber);
                            itm.Attributes.Add("style", "display: none");
                            itm.InnerHtml = textInfo.ToTitleCase(child.Name ?? child.Id.ToString());
                            cbo.AppendChild(itm);
                        }
                    }
                }
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void AppendCaseTypeValues(HtmlNode cbo, List<StateSearchConfiguration> configurations)
        {
            const string caseType = "Case Type";
            var temp = configurations
            .SelectMany(s => s.Counties)
            .Where(w => w.Data != null && w.Data.CaseSearchTypes != null)
            .ToList();
            if (!temp.Any()) return;
            CreateDefaultCountyOption(cbo);
            temp.ForEach(t =>
            {
                if (t.Data?.CaseSearchTypes != null && t.Data.CaseSearchTypes.Any())
                {
                    var dd = t.Data.CaseSearchTypes;
                    foreach (var dd2 in dd) { dd2.CountyId = t.Index; }
                }
            });
            var dropdowns = temp.Select(s =>
            {
                if (s.Data?.CaseSearchTypes == null || !s.Data.CaseSearchTypes.Any())
                    return null;
                return new { searches = s.Data.CaseSearchTypes };
            }).Where(w => w != null).ToList();
            dropdowns.ForEach(s =>
            {
                if (s != null)
                {
                    foreach (var item in s.searches)
                    {
                        var id = item.CountyId.GetValueOrDefault();
                        var itm = cbo.OwnerDocument.CreateElement("option");
                        itm.Attributes.Add("value", item.Id.ToString());
                        itm.Attributes.Add("dat-row-name", caseType);
                        itm.Attributes.Add("dat-county-index", id.ToString());
                        itm.Attributes.Add("style", "display: none");
                        itm.InnerHtml = textInfo.ToTitleCase(item.Name ?? item.Id.ToString());
                        cbo.AppendChild(itm);
                    }
                }
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void CreateDefaultCountyOption(HtmlNode cbo)
        {
            if (cbo.HasChildNodes) return;
            var nde = cbo.OwnerDocument.CreateElement("option");
            nde.Attributes.Add("value", "");
            nde.Attributes.Add("dat-county-index", "");
            nde.Attributes.Add("dat-state-index", "");
            nde.InnerHtml = "- select -";
            cbo.AppendChild(nde);
        }

        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
    }
}