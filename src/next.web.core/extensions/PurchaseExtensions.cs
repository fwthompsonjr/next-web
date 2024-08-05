using HtmlAgilityPack;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace next.web.core.extensions
{
    using HistoryHelper = legallead.desktop.utilities.SearchHistoryContentHelper;
    internal static class PurchaseExtensions
    {
        public static async Task<string> GetPurchases(
            this ISession session,
            IPermissionApi api,
            string source
            )
        {
            const string purchaseTable = "//table[@automationid='search-purchases-table']";
            const string noDataRow = "//*[@id='tr-subcontent-purchases-no-data']";
            const string pagerRow = "//*[@id='tr-subcontent-purchases-pager']";
            const string cboPager = "//*[@id='cbo-subcontent-purchases-pager']";
            const string findRecordCount = "//*[@id='td-subcontent-purchases-record-count']";

            const int pageSize = 10;
            var dPage = Convert.ToDecimal( pageSize );
            var user = session.GetUser();
            if (user == null) return source;
            source = await HistoryHelper.InjectHistory(api, user, source);
            var document = source.ToHtml();
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(purchaseTable);
            if (table == null) return source;
            var tbody = table.SelectSingleNode("tbody");
            if (tbody == null) return source;
            var rows = tbody.SelectNodes("tr");
            if (rows == null) return source;
            if (rows.Count < 3) return source;
            // hide the no records found indicator
            var noData = node.SelectSingleNode(noDataRow);
            if (noData != null) AppendStyle(noData, "display: none");
            // setup pagination at row level
            var indx = 0;
            var collection = rows
                .Select(w => {
                    var id = (indx++) - 2;
                    var page = Convert.ToInt32(Math.Floor(id / dPage));
                    var position = id % 2 == 0 ? "even" : "odd";
                    return new { id, page, position, row = w };
                    })
                .Where(x => x.id >= 0)
                .ToList();
            collection.ForEach(c => {
                var element = c.row;
                element.Attributes.Add("name", "injected-content");
                element.Attributes.Add("data-row-number", c.id.ToString());
                element.Attributes.Add("data-page-number", c.page.ToString());
                element.Attributes.Add("data-position", c.position);
            });
            // populate pager
            var trPager = node.SelectSingleNode(pagerRow);
            RemoveStyleAndClass(trPager);
            var rowcount = collection.Count;
            var cbo = node.SelectSingleNode(cboPager);
            ApppendRowCount(cbo, rowcount, pageSize);

            var nCount = node.SelectSingleNode(findRecordCount);
            if (nCount != null) nCount.InnerHtml = $"Records: {rowcount}";

            return node.OuterHtml;
        }

        private static void AppendStyle(HtmlNode noData, string attributeValue)
        {
            const string attrName = "style";
            var attr = noData.Attributes.ToList();
            var style = attr.Find(n => n.Name == attrName);
            if (style != null) { 
                style.Value = attributeValue;
                return;
            }
            noData.Attributes.Append(attrName, attributeValue);
        }

        private static void RemoveStyleAndClass(HtmlNode? node)
        {
            if (node == null) return;
            List<string> cls = ["stlye", "class"];
            var attrs = node.Attributes.Select(s => s.Name);
            cls.ForEach(s => {
                if (attrs.Contains(s)) { node.Attributes.Remove(s); }
            });
        }

        private static void ApppendRowCount(HtmlNode? node, int rowcount, int interval)
        {
            if (node == null) return;
            var builder = new StringBuilder();
            builder.AppendLine();
            var rw = 0;
            for (int i = 0; i < rowcount; i += interval) 
            {
                var mn = i + 1;
                var mx = (rw+1) * interval;
                var text = $"<option value=\"{rw}\">Records: {mn} to {mx}</option>";
                builder.AppendLine(text);
                rw++;
            }
            node.InnerHtml = builder.ToString();
        }
    }
}
