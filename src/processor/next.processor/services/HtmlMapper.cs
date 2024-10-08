﻿using HtmlAgilityPack;
using legallead.jdbc.entities;
using next.processor.api.extensions;
using next.processor.api.models;
using next.processor.api.utility;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace next.processor.api.services
{
    public static class HtmlMapper
    {
        public static string Home(string content, string health = "Healthy")
        {
            var document = content.ToDocument();
            var node = document.DocumentNode;
            if (string.IsNullOrWhiteSpace(health)) health = "Healthy";
            var substitutions = new Dictionary<string, string>
            {
                { "//span[@name='detail-01-caption']", Environment.MachineName.ToUpper() },
                { "//span[@name='detail-02-caption']", DateTime.UtcNow.ToString("s").Replace("T", " - ") },
                { "//span[@name='detail-03-caption']", health }
            };
            var keys = substitutions.Keys.ToList();
            keys.ForEach(key =>
            {
                var indx = keys.IndexOf(key);
                var find = node.SelectSingleNode(key);
                if (find != null)
                {
                    find.InnerHtml = substitutions[key];
                    if (indx == 2) AlterNodeClass(find, health);
                }
            });
            SetCssIndex(document);
            return node.OuterHtml;
        }

        public static string Home(string content, Dictionary<string, object> substitutions)
        {
            const string find = "//table[@name='tb-detail']/tbody";
            var document = content.ToDocument();
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return node.OuterHtml;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return node.OuterHtml;
            var template = tr.OuterHtml.Replace("template-row", "detail-item");
            var builder = new StringBuilder();
            substitutions.Keys.ToList().ForEach(k =>
            {
                var data = Convert.ToString(substitutions[k]);
                var content = template.Replace("~0", k).Replace("~1", data);
                builder.AppendLine(content);
            });
            tbody.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }

        public static string Summary(string content, List<StatusSummaryBo>? substitutions)
        {
            const string find = "//table[@name='tb-queue-summary']/tbody";
            if (substitutions == null || substitutions.Count == 0) return content;
            var document = content.ToDocument();
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return node.OuterHtml;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return node.OuterHtml;
            var template = tr.OuterHtml.Replace("template-row", "detail-item");
            var builder = new StringBuilder();
            substitutions.ForEach(k =>
            {
                var txt = template
                    .Replace("~0", k.SearchProgress ?? " - ")
                    .Replace("~1", k.Total.GetValueOrDefault(0).ToString());
                builder.AppendLine(txt);
            });
            tbody.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }
        public static string Life(string content)
        {
            // remove div elements named detail- 0 - 4
            int indx = 0;
            var document = content.ToDocument();
            var node = document.DocumentNode;
            while (indx < 5)
            {
                var find = $"//*[@id='detail-{indx:D2}']";
                var div = node.SelectSingleNode(find);
                var parent = div?.ParentNode;
                if (div != null && parent != null) parent.RemoveChild(div);
                indx++;
            }
            SetCssIndex(document);
            return node.OuterHtml;
        }
        public static string Status(string content, List<KeyValuePair<string, string>>? statuses = null)
        {
            var document = content.ToDocument();
            var node = document.DocumentNode;
            SetCssIndex(document);
            var errors = TrackEventService.Get(Constants.ErrorLogName);
            AppendErrorDetail(document, errors);
            AppendStatusDetail(document, statuses);
            return node.OuterHtml;
        }

        public static string Status(string content, Dictionary<string, string> substitutions)
        {
            const string find = "//table[@name='tb-directory']/tbody";
            var document = content.ToDocument();
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return node.OuterHtml;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return node.OuterHtml;
            var template = tr.OuterHtml.Replace("template-row", "detail-item");
            var builder = new StringBuilder();
            substitutions.Keys.ToList().ForEach(k =>
            {
                var data = string.IsNullOrEmpty(substitutions[k]) ? " - " : substitutions[k];
                var content = template.Replace("~0", k).Replace("~1", data);
                builder.AppendLine(content);
            });
            tbody.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }

        public static string StatusDetail(string content, List<StatusSummaryByCountyBo>? substitutions)
        {
            const string dash = " - ";
            const string find = "//table[@name='tb-drill-down']/tbody";
            if (substitutions == null || substitutions.Count == 0) return content;
            var document = content.ToDocument();
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return node.OuterHtml;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return node.OuterHtml;
            var template = tr.OuterHtml.Replace("template-row", "detail-item");
            var builder = new StringBuilder();
            substitutions.ForEach(k =>
            {
                var txt = template
                    .Replace("~0", k.Region ?? dash)
                    .Replace("~1", k.Count.GetValueOrDefault(0).ToString())
                    .Replace("~2", k.Newest.HasValue ? k.Newest.Value.ToString("s") : dash)
                    .Replace("~3", k.Oldest.HasValue ? k.Oldest.Value.ToString("s") : dash);
                builder.AppendLine(txt);
            });
            tbody.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }
        private static void AlterNodeClass(HtmlNode node, string health)
        {
            const string cls = "class";
            const string secondary = "text-secondary";
            var clsname = health.ToLower() switch
            {
                "healthy" => "text-success",
                "degraded" => "text-warning",
                "unhealthy" => "text-danger",
                _ => secondary
            };
            AddOrUpdateAttribute(node, cls, secondary, clsname);
        }


        private static HtmlDocument ToDocument(this string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }


        private static void AppendErrorDetail(HtmlDocument document, string? errors)
        {
            const string find = "//table[@name='tb-errors']/tbody";
            if (string.IsNullOrWhiteSpace(errors)) return;
            var models = errors.ToInstance<List<TrackErrorModel>>();
            if (models == null) return;
            var items = models.Select(x => x.Data).ToList();
            var details = items.Select(x => new { code = x.CreateDate.ToString("s"), message = x.Message }).ToList();
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return;
            var template = tr.OuterHtml.Replace("template-row", "detail-row");
            var builder = new StringBuilder();
            details.ForEach(detail =>
            {
                var content = template.Replace("~0", detail.code).Replace("~1", detail.message);
                builder.AppendLine(content);
            });
            tbody.InnerHtml = builder.ToString();
        }

        private static void AppendStatusDetail(HtmlDocument document, List<KeyValuePair<string, string>>? statuses = null)
        {
            const string find = "//table[@name='tb-status']/tbody";
            if (statuses == null || statuses.Count == 0) return;
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return;
            var template = tr.OuterHtml.Replace("template-row", "detail-row");
            var builder = new StringBuilder();
            statuses.ForEach(detail =>
            {
                var innerHtml = GetStatusLinkHtml(detail.Key, detail.Value);
                var content = template.Replace("~0", detail.Key).Replace("~1", innerHtml);
                builder.AppendLine(content);
            });
            tbody.InnerHtml = builder.ToString();
            AppendStatusHealthCss(document);
        }

        private static void AppendStatusHealthCss(HtmlDocument document)
        {
            const string find = "//table[@name='tb-status']/tbody/tr[@name='detail-row']";
            var node = document.DocumentNode;
            var tr = node.SelectSingleNode(find);
            if (tr == null) return;
            var tds = tr.SelectNodes("td");
            if (tds == null || tds.Count < 2) return;
            var td = tds[1];
            var text = td.InnerText;
            var clsname = text switch
            {
                "UNHEALTHY" => "text-danger",
                "DEGRADED" => "text-warning",
                "HEALTHY" => "text-success",
                _ => "text-info"
            };
            AddOrUpdateAttribute(td, "class", "text-info", clsname);
        }

        private static string GetStatusLinkHtml(string key, string name)
        {
            const char space = ' ';
            const char dqoute = '"';
            const char sqoute = (char)39;
            const string layout = "<a class='link-light' href='/clear?name=toggle-{0}'>{1}</a>";
            var oic = StringComparison.OrdinalIgnoreCase;
            List<string> transforms = ["installation", "queue"];
            var keyname = key.Split(space)[0];
            var transformKey = transforms.Find(x => x.Equals(keyname, oic));
            var transformName = name.ToUpper();
            if (transformKey == null) return transformName;
            var html = string.Format(layout.Replace(sqoute, dqoute), transformKey, transformName);
            return html;
        }

        [ExcludeFromCodeCoverage]
        private static void AddOrUpdateAttribute(HtmlNode node, string cls, string secondary, string clsname)
        {
            var clss = node.Attributes.FirstOrDefault(x => x.Name == cls);
            if (clss == null)
            {
                node.Attributes.Add(cls, clsname);
                return;
            }
            var items = clss.Value.Split(' ').ToList();
            items.Remove(secondary);
            items.Add(clsname);
            clss.Value = string.Join(" ", items);
        }

        private static void SetCssIndex(HtmlDocument document)
        {
            List<string> finds = [
                "//link[@name='base-css']",
                "//link[@name='reader-css']"
            ];
            var node = document.DocumentNode;
            finds.ForEach(query =>
            {
                var link = node.SelectSingleNode(query);
                if (link != null)
                {
                    var attr = link.Attributes.FirstOrDefault(a => a.Name == "href");
                    if (attr != null)
                    {
                        var current = attr.Value.Split('?')[0];
                        var transformed = $"{current}?id={EnvironmentTicks}";
                        attr.Value = transformed;
                    }
                }
            });
        }

        private static string? enviromentTicks;
        private static string EnvironmentTicks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(enviromentTicks))
                {
                    enviromentTicks = Environment.TickCount.ToString();
                }
                return enviromentTicks;
            }
        }
    }
}
