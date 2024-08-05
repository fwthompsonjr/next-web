using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace next.web.core.extensions
{
    internal static class HistoryExtensions
    {
        public static async Task<string> GetHistory(
            this ISession session,
            IPermissionApi api,
            string source,
            SearchFilterNames searchFilter = SearchFilterNames.History
        )
        {
            var document = source.ToHtml();
            if (document == null) return source;
            return await GetSearchHtml(session, api, document, searchFilter);
        }

        private static async Task<string> GetSearchHtml(ISession session, IPermissionApi api, HtmlDocument document, SearchFilterNames searchFilter = SearchFilterNames.History)
        {
            var list = await session.RetrieveHistory(api);
            var purchases = await session.RetrievePurchases(api);
            list = MapDownloaded(list, purchases);
            var filter = session.RetrieveFilter(searchFilter);
            list = list.FindAll(x =>
            {
                if (searchFilter == SearchFilterNames.History) return true;
                if (searchFilter == SearchFilterNames.Purchases) return IsPurchase(x.SearchProgress);
                if (searchFilter == SearchFilterNames.Active) return IsActive(x.SearchProgress);
                return false;
            });
            var restriction = await session.RetrieveRestriction(api);
            if (filter.HasFilter)
            {
                list = list.FindAll(x =>
                {
                    var showStatus = MatchProgress(x.SearchProgress, filter.Index);
                    var showCounty = MatchCounty(x.CountyName, filter.County);
                    return showStatus && showCounty;
                });
            }
            var data = list.ToJsonString();
            var table = Map(data, out var rows);
            AppendTable(document, itemlist, table);
            AppendRestriction(document, restriction, restrictionstatus);
            ApplyStatusFilter(document, filter);
            ApplyCountyFilter(document, filter);
            ApplyFilterCaption(document, filter);
            ToggleVisibility(nohistory, itemlist, itemview, countattribute, rows, document);
            var title = searchFilter switch
            {
                SearchFilterNames.Active => "My Active Searches",
                SearchFilterNames.Purchases => "My Search Purchases",
                _ => "Search History"
            };
            var node = document.DocumentNode;
            var subheader = node.SelectSingleNode("//*[@id='search-history-sub-header']");
            if (subheader != null) subheader.InnerHtml = title;
            return node.OuterHtml;
        }

        private static bool IsActive(string? searchProgress)
        {
            var find = new List<string> { "submitted", "processing" };
            if (string.IsNullOrEmpty(searchProgress)) return false;
            var exists = find.Exists(x => searchProgress.Contains(x, StringComparison.OrdinalIgnoreCase));
            return exists;
        }

        private static bool IsPurchase(string? searchProgress)
        {
            var find = new List<string> { "purchase", "download" };
            if (string.IsNullOrEmpty(searchProgress)) return false;
            var exists = find.Exists(x => searchProgress.Contains(x, StringComparison.OrdinalIgnoreCase));
            return exists;
        }

        private static List<UserSearchQueryBo> MapDownloaded(List<UserSearchQueryBo> list, List<MyPurchaseBo> purchases)
        {
            const string txt = "5 - Downloaded";
            var downloads = purchases.FindAll(x =>
                (x.StatusText ?? string.Empty).Contains("download", StringComparison.OrdinalIgnoreCase));
            if (downloads.Count == 0) { return list; }
            var indexed = downloads
                .Select(x => x.ReferenceId ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().ToList();
            var found = list.FindAll(x => indexed.Contains(x.Id ?? string.Empty));
            found.ForEach(a => a.SearchProgress = txt);
            return list;
        }

        public static string Map(string? history, out int rows)
        {
            var html = historyhtml;
            rows = 0;
            if (string.IsNullOrEmpty(history)) { return html; }
            var items = history.ToInstance<List<UserSearchQueryBo>>() ?? [];
            rows = items.Count;
            var template = Substitutions["history"];
            var document = html.ToHtml();
            var transform = TransformRows(document, items.Cast<ISearchIndexable>().ToList(), template);
            var styled = ApplyHistoryStatus(transform.ToHtml(), template);
            return styled;
        }

        private static void AppendTable(HtmlDocument doc, string itemlist, string table)
        {
            var element = doc.DocumentNode.SelectSingleNode(itemlist);
            if (element != null) { element.InnerHtml = table; }
        }

        private static void AppendRestriction(HtmlDocument doc, MySearchRestrictions restriction, string restrictionstatus)
        {
            var isrestricted = restriction.IsLocked.GetValueOrDefault(true) ? "true" : "false";
            var node = doc.DocumentNode.SelectSingleNode(restrictionstatus);
            if (node != null) { node.Attributes["value"].Value = isrestricted; }
        }
        public static void ApplyStatusFilter(HtmlDocument document, UserSearchFilterBo filter)
        {
            const string sel = "selected";
            HtmlNode? combo = document.DocumentNode.SelectSingleNode(filterstatus);
            if (!filter.HasFilter || combo == null) return;
            var options = combo.SelectNodes("option").ToList();
            options.ForEach(opt =>
            {
                var attribute = opt.Attributes.ToList().Find(x => x.Name.Equals(sel));
                var ovalue = opt.Attributes["value"].Value;
                if (int.TryParse(ovalue, out var id) && id == filter.Index && attribute == null)
                {
                    opt.Attributes.Add(sel, sel);
                }
                else
                {
                    if (attribute != null) { opt.Attributes.Remove(attribute); }
                }
            });
        }
        public static void ApplyCountyFilter(HtmlDocument document, UserSearchFilterBo filter)
        {
            const string sel = "selected";
            HtmlNode? combo = document.DocumentNode.SelectSingleNode(filtercounty);
            if (!filter.HasFilter || combo == null) return;
            var options = combo.SelectNodes("option").ToList();
            var search = string.IsNullOrEmpty(filter.County) ? "None" : filter.County;
            options.ForEach(opt =>
            {
                var attribute = opt.Attributes.ToList().Find(x => x.Name.Equals(sel));
                var ovalue = opt.Attributes["name"].Value;
                if (search.Equals(ovalue, StringComparison.OrdinalIgnoreCase) && attribute == null)
                {
                    opt.Attributes.Add(sel, sel);
                }
                else
                {
                    if (attribute != null) { opt.Attributes.Remove(attribute); }
                }
            });
        }

        public static void ApplyFilterCaption(HtmlDocument document, UserSearchFilterBo filter)
        {
            HtmlNode? combo = document.DocumentNode.SelectSingleNode(filtercaption);
            if (!filter.HasFilter || combo == null) return;
            combo.InnerHtml = filter.GetCaption();
        }
        private static void ToggleVisibility(
            string nohistory,
            string itemlist,
            string itemview,
            string countattribute,
            int rows,
            HtmlDocument doc)
        {
            HtmlNode? element;

            const string previewtable = "//*[@automationid='search-preview-table']";
            var tablestyle = rows == 0 ? "display: none" : "width: 95%";
            var dvs = new List<string> { itemlist, itemview, nohistory };
            var display = rows == 0 ? "0" : "1";
            dvs.ForEach(d =>
            {
                element = doc.DocumentNode.SelectSingleNode(d);
                if (element != null) element.Attributes[countattribute].Value = display;
            });
            element = doc.DocumentNode.SelectSingleNode(previewtable);
            if (element == null) return;
            element.Attributes["style"].Value = tablestyle;
        }
        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string TransformRows(HtmlDocument document, List<ISearchIndexable> data, MySearchSubstitutions substitutions)
        {
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(substitutions.Table);
            var template = node.SelectSingleNode(substitutions.Template);
            var nodata = node.SelectSingleNode(substitutions.NoDataTemplate);
            if (table == null || template == null) return node.OuterHtml;
            var tbody = table.SelectSingleNode("tbody");
            var style = document.CreateAttribute("style", "display: none");
            nodata.Attributes.Add(style);
            foreach (var item in data)
            {
                var r = data.IndexOf(item);
                var pg = r / tableRowCount;
                var position = r % 2 == 0 ? "even" : "odd";
                var rowdata = document.CreateElement("tr");
                var attr = document.CreateAttribute("search-uuid", item[0]);
                var rwnumber = document.CreateAttribute("data-row-number", r.ToString());
                var pgnumber = document.CreateAttribute("data-page-number", pg.ToString());
                var attrwpos = document.CreateAttribute("data-position", position);
                var rwstyle = document.CreateAttribute("style", "display: none");
                rowdata.Attributes.Add(attr);
                rowdata.Attributes.Add(rwnumber);
                rowdata.Attributes.Add(pgnumber);
                rowdata.Attributes.Add(attrwpos);
                if (pg > 0) rowdata.Attributes.Add(rwstyle);
                var row = template.InnerHtml.Replace("~0", r.ToString()).Replace("~7", r.ToString()); ;
                for (var i = 1; i < substitutions.Targets + 1; i++)
                {
                    var search = $"~{i}";
                    row = row.Replace(search, item[i]);
                }
                rowdata.InnerHtml = row;
                tbody.AppendChild(rowdata);
            }
            TransformPaging(table, data);
            return node.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static void TransformPaging(HtmlNode table, List<ISearchIndexable> data)
        {
            var tfoot = table.SelectSingleNode("tfoot");
            var trow = tfoot.SelectSingleNode("tr");
            var cells = trow.SelectNodes("td").ToArray();
            var cbo = cells[0].SelectSingleNode("select");
            var td = cells[1];
            td.InnerHtml = $"Records: {data.Count}";
            cbo.ChildNodes.Clear();
            var doc = table.OwnerDocument;
            if (data.Count > 0 && tfoot.Attributes["class"] != null)
            {
                var attr = tfoot.Attributes["class"];
                attr.Value = attr.Value.Replace("d-none", "").Trim();
            }
            for (var i = 0; i < data.Count; i += tableRowCount)
            {
                var pg = i / tableRowCount;
                var mx = Math.Min(i + tableRowCount, data.Count);
                var lbl = $"Records: {i + 1} to {mx}";
                var optn = doc.CreateElement("option");
                var att1 = doc.CreateAttribute("value", pg.ToString());
                optn.InnerHtml = lbl;
                optn.Attributes.Append(att1);
                if (i == 0)
                {
                    var att2 = doc.CreateAttribute("selected", "selected");
                    optn.Attributes.Add(att2);
                }
                cbo.AppendChild(optn);
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string ApplyHistoryStatus(HtmlDocument document, MySearchSubstitutions substitutions)
        {
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(substitutions.Table);
            if (table == null) return node.OuterHtml;
            var tbody = table.SelectSingleNode("tbody");
            if (tbody == null) return node.OuterHtml;
            var rows = tbody.SelectNodes("//tr[@data-position]")?.ToList();
            if (rows == null || rows.Count == 0) return node.OuterHtml;
            rows.ForEach(row =>
            {
                var status = row.SelectNodes("td")?.ToList()[^1].SelectSingleNode("span");
                var text = status?.InnerText.Trim();
                if (status != null && !string.IsNullOrEmpty(text))
                {
                    var stsCss = GetStatusCss(text);
                    if (stsCss != null)
                    {
                        var attr = document.CreateAttribute("class", stsCss);
                        status.Attributes.Add(attr);
                    }
                }
            });
            return node.OuterHtml;
        }

        private static bool MatchProgress(string? progressText, int statusCode)
        {
            if (statusCode == 0) return true;
            if (string.IsNullOrEmpty(progressText)) return false;
            var find = statusCode switch
            {
                10 => "Error",
                1 => "Submitted",
                2 => "Processing",
                3 => "Completed",
                4 => "Purchased",
                5 => "Downloaded",
                _ => string.Empty
            };
            if (string.IsNullOrWhiteSpace(find)) return false;
            return progressText.Contains(find, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchCounty(string? countyText, string? countyCode)
        {
            if (string.IsNullOrWhiteSpace(countyCode)) return true;
            if (string.IsNullOrEmpty(countyText)) return false;
            return countyText.Equals(countyCode, StringComparison.OrdinalIgnoreCase);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string? GetStatusCss(string searchStatus)
        {
            return searchStatus switch
            {
                "Completed" => "text-success",
                "Processing" => "text-warning-emphasis",
                "Error" => "text-danger",
                "Purchased" => "text-info",
                "Downloaded" => "text-primary",
                _ => null,
            };
        }
        private const int tableRowCount = 15;
        private readonly static string historyhtml = Properties.Resources.search_table_layout;
        private static Dictionary<string, MySearchSubstitutions> Substitutions => _substitions ??= GetSubstitutions();

        private static readonly string substitutions_history = "{ " + Environment.NewLine +
            " \"table\": \"//table[@name='search-dt-table']\", " + Environment.NewLine +
            " \"template\": \"//tr[@id='tr-subcontent-history-data-template']\", " + Environment.NewLine +
            " \"nodatatemplate\": \"//tr[@id='tr-subcontent-history-no-data']\", " + Environment.NewLine +
             " \"targets\": 6 " + Environment.NewLine + " }";

        private const string nohistory = "//*[@id='dv-history-item-no-history']";
        private const string itemlist = "//*[@id='dv-history-item-list']";
        private const string itemview = "//*[@id='dv-history-item-preview']";
        private const string filterstatus = "//*[@id='cbo-search-history-filter']";
        private const string filtercounty = "//*[@id='cbo-search-history-county']";
        private const string filtercaption = "//*[@id='search-history-heading-caption']";
        private const string restrictionstatus = "//*[@id='user-restriction-status']";
        private const string countattribute = "data-item-count";
        private static Dictionary<string, MySearchSubstitutions>? _substitions;
        private static Dictionary<string, MySearchSubstitutions> GetSubstitutions()
        {
            var history = substitutions_history.ToInstance<MySearchSubstitutions>() ?? new();
            return new Dictionary<string, MySearchSubstitutions>
            {
                { "history", history }
            };

        }


    }
}
/*

*/