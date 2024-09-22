using HtmlAgilityPack;
using Newtonsoft.Json;
using next.core.entities;
using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.core.implementations
{
    internal class UserSearchMapper : IUserSearchMapper
    {

        public void SetCounty(IHistoryPersistence? persistence, HtmlNode? combo)
        {
            const string sel = "selected";
            if (persistence == null || combo == null) return;
            var json = persistence.Filter();
            if (string.IsNullOrWhiteSpace(json)) return;
            var filter = ObjectExtensions.TryGet<UserSearchFilterBo>(json) ?? new();
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

        public void SetFilter(IHistoryPersistence? persistence, HtmlNode? combo)
        {
            const string sel = "selected";
            if (persistence == null || combo == null) return;
            var json = persistence.Filter();
            if (string.IsNullOrWhiteSpace(json)) return;
            var filter = ObjectExtensions.TryGet<UserSearchFilterBo>(json) ?? new();
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
        public string Map(IHistoryPersistence? persistence, string? history, out int rows)
        {
            const char space = ' ';
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            rows = 0;
            if (persistence == null) { return Map(history, out rows); }
            if (string.IsNullOrWhiteSpace(history)) { return Map(history, out rows); }
            var json = persistence.Filter();
            if (string.IsNullOrWhiteSpace(json)) { return Map(history, out rows); }
            var filter = ObjectExtensions.TryGet<UserSearchFilterBo>(json) ?? new();
            if (!filter.HasFilter) return Map(history, out rows);
            var items = ObjectExtensions.TryGet<List<UserSearchQueryBo>>(history);
            var statusName = filter.Index switch
            {
                10 => "Error",
                1 => "Submitted",
                2 => "Processing",
                3 => "Completed",
                4 => "Purchased",
                5 => "Downloaded",
                _ => string.Empty
            };
            items = items.FindAll(x =>
            {
                if (string.IsNullOrEmpty(statusName)) { return true; }
                var progress = x.SearchProgress ?? "";
                if (!string.IsNullOrWhiteSpace(progress) && progress.Contains(space))
                {
                    var arr = progress.Split(space);
                    progress = arr[^1].Trim();
                }
                return progress.Equals(statusName, comparison);
            });
            if (!string.IsNullOrEmpty(filter.County))
            {
                items = items.FindAll(x =>
                {
                    var county = (x.CountyName ?? "");
                    if (string.IsNullOrEmpty(county)) { return false; }
                    return county.Equals(filter.County, comparison);
                });
            }
            var collection = JsonConvert.SerializeObject(items);
            return Map(collection, out rows);
        }
        public string Map(string? history, out int rows)
        {
            var html = historyhtml;
            rows = 0;
            if (string.IsNullOrEmpty(history)) { return html; }
            var items = ObjectExtensions.TryGet<List<UserSearchQueryBo>>(history);
            rows = items.Count;
            var template = Substitutions["history"];
            var document = ToDocument(html);
            var transform = TransformRows(document, items.Cast<ISearchIndexable>().ToList(), template);
            var styled = ApplyHistoryStatus(ToDocument(transform), template);
            return styled;
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
        private readonly Dictionary<string, MySearchSubstitutions> Substitutions =
            new()
            {
                { "history", JsonConvert.DeserializeObject<MySearchSubstitutions>(substitutions_history) ?? new() }
            };
        private static readonly string substitutions_history = "{ " + Environment.NewLine +
            " \"table\": \"//table[@name='search-dt-table']\", " + Environment.NewLine +
            " \"template\": \"//tr[@id='tr-subcontent-history-data-template']\", " + Environment.NewLine +
            " \"nodatatemplate\": \"//tr[@id='tr-subcontent-history-no-data']\", " + Environment.NewLine +
             " \"targets\": 6 " + Environment.NewLine + " }";

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static HtmlDocument ToDocument(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
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

        private const int tableRowCount = 15;


        private readonly static string historyhtml = Properties.Resources.searchhistory_table_html;
    }
}
