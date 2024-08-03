using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using legallead.desktop.interfaces;
using next.web.core.models;
using System.Text;
using Newtonsoft.Json;
using next.web.core.util;

namespace next.web.core.extensions
{
    internal static class MailboxExtensions
    {
        public static async Task<string> GetMailBox(
            this ISession session,
            IPermissionApi api,
            string source
        )
        {

            const string findList = "//*[@id=\"dv-mail-item-list\"]";
            const string findFrame = "//*[@id=\"dv-mail-item-preview\"]";
            var document = GetDocument(source);
            if (document == null) return source;
            var list = await session.RetrieveMail(api);
            var data = JsonConvert.SerializeObject(list);
            var recordId = list.Count == 0 ? string.Empty : list[0].Id ?? string.Empty;
            var content = list.Count == 0 ?
                string.Empty : (await session.FetchMailBody(api, recordId));
            var selected = list.Find(x => (x.Id ?? "--").Equals(recordId, StringComparison.OrdinalIgnoreCase));
            AppendCount(document, list, selected);
            AppendList(list, document, findList);
            AppendPreview(content, document, findFrame);

            return AppendJson(document, data, content);
        }

        public static async Task<string> FetchMailBody(this ISession session, IPermissionApi api, string recordId)
        {
            var keyname = string.Format(SessionKeyNames.UserMailboxItemFormat, recordId);
            if (!session.IsItemExpired<MailItemBody>(keyname))
            {
                var item = session.GetTimedItem<MailItemBody>(keyname) ?? new();
                return item.Body ?? string.Empty;
            }
            var user = session.GetUser();
            if (user == null) { return string.Empty; }
            var body = await user.GetMailBody(api, recordId);
            if (body == null) return string.Empty;
            // add item to session
            var timed = new UserTimedCollection<MailItemBody>(body, TimeSpan.FromDays(2));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(keyname, Encoding.UTF8.GetBytes(json));

            return body.Body ?? string.Empty;
        }


        internal static string RestyleBlue(string content)
        {
            const string find = "color: blue";
            const string alternate = "#0d6efd";
            var replacement = $"color: {alternate}";
            if (!content.Contains(find)) return RestyleColumns(content);
            return RestyleColumns(content.Replace(find, replacement));
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
        private static void AppendCount(HtmlDocument document, List<MailItem> list, MailItem? selected = null)
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

        private static void AppendList(List<MailItem> list, HtmlDocument document, string findList)
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


        private static HtmlNode GetListItem(HtmlDocument document, MailItem item, int index)
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


        private static HtmlNode GetHeaderNode(HtmlDocument document, MailItem item)
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
            createDate.InnerHtml = item.CreateDt;
            element.AppendChild(subject);
            element.AppendChild(createDate);
            return element;
        }


        private static HtmlNode GetDetailNode(HtmlDocument document, MailItem item)
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


        private static void AppendPreview(string? content, HtmlDocument document, string findFrame)
        {
            if (string.IsNullOrEmpty(content)) return;
            var viewFrame = document.DocumentNode.SelectSingleNode(findFrame);
            if (viewFrame == null) return;
            viewFrame.InnerHtml = RestyleBlue(content);
        }


        private static HtmlDocument? GetDocument(string source)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(source);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
