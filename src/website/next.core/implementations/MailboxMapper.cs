using HtmlAgilityPack;
using next.core.interfaces;
using next.core.models;
using System.Text;

namespace next.core.implementations
{
    internal class MailboxMapper : UserProfileMapper, IUserMailboxMapper
    {
        public string Substitute(IMailPersistence? persistence, string source)
        {
            const string findList = "//*[@id=\"dv-mail-item-list\"]";
            const string findFrame = "//*[@id=\"dv-mail-item-preview\"]";
            var document = GetDocument(source);
            if (document == null) return source;
            var data = persistence?.Fetch() ?? string.Empty;
            var list = GetData(persistence, data);
            var recordId = list.Count == 0 ? string.Empty : list[0].Id ?? string.Empty;
            var content = list.Count == 0 ? string.Empty : persistence?.Fetch(recordId) ?? string.Empty;
            var selected = list.Find(x => (x.Id ?? "--").Equals(recordId, StringComparison.OrdinalIgnoreCase));
            AppendCount(document, list, selected);
            AppendList(list, document, findList);
            AppendPreview(content, document, findFrame);

            return AppendJson(document, data, content);
        }
        internal static string RestyleColumns(string content)
        {
            const char semi = ';';
            const string searches = "<td width=\"70%\">;<td width=\"25%\">";
            const string alternate = "<td width=\"65%\">;<td width=\"30%\">";
            var finds = searches.Split(semi).ToList();
            var replacements = alternate.Split(semi);
            foreach (var find in finds)
            {
                var indx = finds.IndexOf(find);
                if (content.Contains(find, StringComparison.OrdinalIgnoreCase))
                {
                    var replacement = replacements[indx];
                    content = content.Replace(find, replacement);
                }
            }
            return content;
        }
        internal static string RestyleBlue(string content)
        {
            const string find = "color: blue";
            const string alternate = "#0d6efd";
            var replacement = $"color: {alternate}";
            if (!content.Contains(find)) return RestyleColumns(content);
            return RestyleColumns(content.Replace(find, replacement));
        }
        private static void AppendCount(HtmlDocument document, List<MailStorageItem> list, MailStorageItem? selected = null)
        {
            const string noItem = "//*[@id=\"dv-mail-item-no-mail\"]";
            const string subHeader = "//*[@id=\"mailbox-sub-header\"]";
            var count = list.Count;
            var mx = count == 0 ? 0 : list.Max(x => x.PositionId);
            var indx = selected == null ? 0 : selected.PositionId;
            var heading = count == 0 ? "Correspondence" : $"Correspondence ( {indx} of {mx} )";
            var itemAttributeValue = count == 0 ? "0" : "1";
            var noItemElement = document.DocumentNode.SelectSingleNode(noItem);
            var headingElement = document.DocumentNode.SelectSingleNode(subHeader);

            if (headingElement == null && noItemElement == null) return;
            if (headingElement != null) headingElement.InnerHtml = heading;

            if (noItemElement == null) return;
            var attribute = noItemElement.Attributes.AsEnumerable().FirstOrDefault(x => x.Name.Equals("data-item-count"));
            if (attribute == null) return;

            attribute.Value = itemAttributeValue;
        }

        private static void AppendList(List<MailStorageItem> list, HtmlDocument document, string findList)
        {
            if (list.Count == 0) return;
            var listElement = document.DocumentNode.SelectSingleNode(findList);
            if (listElement == null) return;
            list.ForEach(item =>
            {
                var position = list.IndexOf(item);
                var child = GetListItem(document, item, position);
                if (child != null) { listElement.AppendChild(child); }
            });
        }

        private static void AppendPreview(string? content, HtmlDocument document, string findFrame)
        {
            if (string.IsNullOrEmpty(content)) return;
            var viewFrame = document.DocumentNode.SelectSingleNode(findFrame);
            if (viewFrame == null) return;
            viewFrame.InnerHtml = RestyleBlue(content);
        }

        private static string AppendJson(HtmlDocument document, string data, string content)
        {
            const string tareajson = "<!-- include: js mail collection -->";
            const string encodedtareajson = "&lt;!-- include: html current view --&gt;";
            const string tareacurrentview = "<!-- include: html current view -->";
            const string encodedtareacurrentview = "&lt;!-- include: js mail collection --&gt;";
            var doubleLine = string.Concat(Environment.NewLine, Environment.NewLine);
            var builder = new StringBuilder(document.DocumentNode.OuterHtml);
            var replacements = new[]
            {
                new { find = tareajson, replace = data},
                new { find = encodedtareajson, replace = data},
                new { find = tareacurrentview, replace = content},
                new { find = encodedtareacurrentview, replace = content},
            };
            foreach (var item in replacements)
            {
                var token = string.Concat(doubleLine, item.replace, doubleLine);
                builder.Replace(item.find, token);
            }
            return builder.ToString();
        }

        private static List<MailStorageItem> GetData(IMailPersistence? persistence, string messages)
        {
            if (persistence == null || string.IsNullOrEmpty(messages)) return new();
            return ObjectExtensions.TryGet<List<MailStorageItem>>(messages);
        }

        private static HtmlNode GetListItem(HtmlDocument document, MailStorageItem item, int index)
        {
            var element = document.CreateElement("a");
            var active = index == 0 ? " active " : string.Empty;
            element.Attributes.Append("name", "link-mail-items-template");
            element.Attributes.Append("href", $"javascript:fetch_item({index})");
            element.Attributes.Append("data-item-index", "1");
            element.Attributes.Append("class", $"{active}list-group-item list-group-item-action".Trim());
            var header = GetHeaderNode(document, item);
            var detail = GetDetailNode(document, item);
            element.AppendChild(header);
            element.AppendChild(detail);
            return element;
        }

        private static HtmlNode GetHeaderNode(HtmlDocument document, MailStorageItem item)
        {
            var element = document.CreateElement("div");
            var subject = document.CreateElement("h5");
            var createDate = document.CreateElement("small");
            element.Attributes.Append("name", "item-header");
            element.Attributes.Append("class", "d-flex w-100 justify-content-between");
            subject.Attributes.Append("name", "item-subject");
            subject.Attributes.Append("class", $"mb-1");
            subject.InnerHtml = item.Subject ?? " - ";
            createDate.Attributes.Append("name", "item-create-date");
            createDate.InnerHtml = item.CreateDate ?? " - ";
            element.AppendChild(subject);
            element.AppendChild(createDate);
            return element;

        }

        private static HtmlNode GetDetailNode(HtmlDocument document, MailStorageItem item)
        {
            var element = document.CreateElement("div");
            var toDv = document.CreateElement("div");
            var fromDv = document.CreateElement("div");
            var itemDv = document.CreateElement("span");

            element.Attributes.Append("name", "item-detail");
            element.Attributes.Append("class", "row");

            // to addresss
            toDv.Attributes.Append("name", "item-address-to");
            toDv.Attributes.Append("class", "col-6 text-start");
            toDv.InnerHtml = $"To: {item.ToAddress ?? string.Empty}";
            // from address
            fromDv.Attributes.Append("name", "item-address-to");
            fromDv.Attributes.Append("class", "col-6 text-start");
            fromDv.InnerHtml = $"From: {item.FromAddress ?? string.Empty}";

            itemDv.Attributes.Append("name", "item-index");
            itemDv.Attributes.Append("class", "d-none");
            itemDv.InnerHtml = item.Id ?? " - ";

            element.AppendChild(toDv);
            element.AppendChild(fromDv);
            element.AppendChild(itemDv);

            return element;

        }

    }
}
