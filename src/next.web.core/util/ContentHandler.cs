using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace next.web.core.util
{
    internal static class ContentHandler
    {
        internal static ContentHtml? GetLocalContent(string name)
        {
            if (AppContainer.ServiceProvider == null) AppContainer.Build();
            var provider = AppContainer.ServiceProvider;
            var contentProvider = ContentProvider.LocalContentProvider;
            if (contentProvider.SearchUi == null)
            {
                var searchUI = provider?.GetService<ISearchBuilder>();
                if (searchUI != null)
                {
                    contentProvider.SearchUi = searchUI;
                }
            }
            var raw = contentProvider.GetContent(name);
            if (raw == null) return null;
            var beutifier = provider?.GetRequiredService<IContentParser>();
            if (beutifier == null) return raw;
            var text = new StringBuilder(raw.Content);
            common.Keys.ToList().ForEach(key =>
            {
                var replacement = common[key];
                text.Replace(key, replacement);
            });
            var html = text.ToString();
            var doc = GetDocument(html);
            if (doc == null)
            {
                raw.Content = beutifier.BeautfyHTML(html);
                return raw;
            }
            html = ReplaceHeadCss(doc, html);
            html = RemoveVerifyJs(doc, html);
            html = RenameHomeCommonJs(doc, html);
            html = ReplaceBodyJs(doc, html);
            html = RemoveUnusedJs(doc, html);
            raw.Content = beutifier.BeautfyHTML(html);
            return raw;
        }

        private static HtmlDocument? GetDocument(string html)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static string ReplaceHeadCss(HtmlDocument doc, string fallback)
        {
            try
            {
                var node = doc.DocumentNode;
                var head = node.SelectSingleNode("//head");
                if (head == null) return fallback;
                head_css_names.ForEach(h =>
                {
                    var find = $"//style[@name='{h}']";
                    var block = head.SelectSingleNode(find);
                    if (block != null)
                    {
                        head.RemoveChild(block);
                        var current = head.InnerHtml;
                        var addition = string.Format(linkRelative, h);
                        current += (Environment.NewLine + addition);
                        head.InnerHtml = current;
                    }
                });
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return fallback;
            }
        }
        private static string RemoveVerifyJs(HtmlDocument doc, string fallback)
        {
            try
            {
                var node = doc.DocumentNode;
                var body = node.SelectSingleNode("//body");
                if (body == null) return fallback;
                var scripts = body.SelectSingleNode("//script[@name='verify-and-post']");
                if (scripts == null) return fallback;
                body.RemoveChild(scripts);
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return fallback;
            }
        }
        private static string RenameHomeCommonJs(HtmlDocument doc, string fallback)
        {
            try
            {
                var node = doc.DocumentNode;
                var body = node.SelectSingleNode("//body");
                if (body == null) return fallback;
                var scripts = body.SelectNodes("//script[not(@name)]");
                if (scripts == null) return fallback;
                var items = scripts.ToList()
                    .FindAll(x => !x.HasAttributes)
                    .FindAll(x => !string.IsNullOrWhiteSpace(x.InnerHtml));
                if (items.Count == 0) return fallback;
                items.ForEach(h =>
                {
                    var current = h.InnerHtml ?? string.Empty;
                    var keywords = 0;
                    home_common_keywords.ForEach(k =>
                    {
                        if (current.Contains(k, StringComparison.CurrentCulture)) { keywords++; }
                    });
                    if (keywords == home_common_keywords.Count)
                    {
                        h.Attributes.Add("name", "home_common");
                    }
                });
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return fallback;
            }
        }
        private static string ReplaceBodyJs(HtmlDocument doc, string fallback)
        {
            try
            {
                var node = doc.DocumentNode;
                var body = node.SelectSingleNode("//body");
                if (body == null) return fallback;
                body_js_names.ForEach(h =>
                {
                    var find = $"//script[@name='{h}']";
                    var block = body.SelectSingleNode(find);
                    if (block != null)
                    {
                        body.RemoveChild(block);
                        var current = body.InnerHtml;
                        var addition = string.Format(scriptTag, h);
                        current += (Environment.NewLine + addition);
                        body.InnerHtml = current;
                    }
                });
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return fallback;
            }
        }
        private static string RemoveUnusedJs(HtmlDocument doc, string fallback)
        {
            try
            {
                var node = doc.DocumentNode;
                var scripts = node.SelectNodes("//script[not(@name)]");
                if (scripts == null) return fallback;
                var items = scripts.ToList()
                    .FindAll(x => !string.IsNullOrWhiteSpace(x.InnerHtml))
                    .FindAll(x =>
                    {
                        var txt = x.InnerHtml ?? string.Empty;
                        return txt.Contains("function commonClientScript()") || txt.Contains("CefSharp");
                    });
                if (items.Count == 0) return fallback;
                items.ForEach(obj =>
                {
                    var parentNode = obj.ParentNode;
                    parentNode.RemoveChild(obj);
                });
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return fallback;
            }
        }
        private static readonly string appName = CoreMetaData.GetKeyOrDefault("app.meta:name", "Oxford Legal Lead");
        private static readonly string appCode = CoreMetaData.GetKeyOrDefault("app.meta:prefix", "oxford.leads.web");
        private static readonly Dictionary<string, string> common = new() {
            { "Legal Lead", appName },
            { $"{appName}s UI", appName },
            { "legallead.ui", appCode },
            { "(c)", "&copy;" }
        };
        private static readonly List<string> head_css_names = [
            "base-css",
            "subcontent-css",
            "mysearch-purchase-css"
        ];
        private static readonly List<string> body_js_names = [
            "home_common",
            "home-form-validation",
            "re-authenticate-js"
        ];
        private static readonly List<string> home_common_keywords = [
            "function showLogout() {",
            "function setDisplay( name )",
            "function setActiveDiv( name )",
            "function getPageName()",
            "function getDisplay( )",
            "function reloadContent()"
        ];
        private static readonly string linkRelative = "<link name=\"{0}\" href=\"/css/{0}.css\" rel=\"stylesheet\" />";
        private static readonly string scriptTag = "<script name=\"{0}\" src=\"/js/{0}.js\"></script>";
    }
}
