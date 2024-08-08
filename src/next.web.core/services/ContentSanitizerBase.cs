using HtmlAgilityPack;
using next.web.core.interfaces;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace next.web.core.services
{
    internal class ContentSanitizerBase : IContentSanitizer
    {
        public virtual string Sanitize(string content)
        {
            return Execute(content);
        }
        public static string IndexContent => indexContent ??= GetIndexContent;

        [ExcludeFromCodeCoverage]
        internal static HtmlDocument? GetDocument(string html)
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

        protected static string DisplayMenuOptions(HtmlDocument doc, string fallback)
        {
            const string dnone = "d-none";
            const string menuSelector = "//div[@id='app-side-menu']";
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var nodeMenu = node.SelectSingleNode(menuSelector);
            if (nodeMenu == null) return fallback;
            var children = nodeMenu.SelectNodes("div");
            if (children == null) return fallback;
            var items = children.ToList();
            items.ForEach(itm =>
            {
                var attr = itm.Attributes.FirstOrDefault(x => x.Name == "class");
                if (attr != null)
                {
                    var cls = attr.Value.Trim().Split(' ').ToList();
                    cls.RemoveAll(x => x == dnone);
                    itm.Attributes["class"].Value = string.Join(" ", cls);
                }
            });
            return node.OuterHtml;
        }

        protected static string RemoveMenuOptions(HtmlDocument doc, string fallback)
        {
            const string menuSelector = "//div[@id='app-side-menu']";
            const string childSelector = "//div[@name='app-sub-menu']";
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var nodeMenu = node.SelectSingleNode(menuSelector);
            if (nodeMenu != null)
            {
                nodeMenu.ParentNode.RemoveChild(nodeMenu);
            }
            var items = node.SelectNodes(childSelector)?.ToList();
            if (items == null) return node.OuterHtml;
            items.ForEach(itm =>
            {
                itm.ParentNode.RemoveChild(itm);
            });
            return node.OuterHtml;
        }

        private static string CommonSubstition(string content)
        {
            var text = new StringBuilder(content);
            common.Keys.ToList().ForEach(key =>
            {
                var replacement = common[key];
                text.Replace(key, replacement);
            });
            return text.ToString();
        }
        private static string AppendMenuHeadCss(HtmlDocument doc, string fallback)
        {
            const string menuHeadCss = "base-menu";
            var node = doc.DocumentNode;
            var head = node.SelectSingleNode(HtmlSelectors.HeadTag);
            if (head == null) return fallback;
            var find = HtmlSelectors.GetNamedStyleTag(menuHeadCss);
            var block = head.SelectSingleNode(find);
            if (block != null) return node.OuterHtml;
            var current = head.InnerHtml;
            var addition = string.Format(linkRelative, menuHeadCss);
            current += (Environment.NewLine + addition);
            head.InnerHtml = current;
            return node.OuterHtml;
        }
        private static string AppendSideMenu(HtmlDocument doc, string fallback)
        {
            const string menuSelector = "//div[@id='app-side-menu']";
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var element = body.SelectSingleNode(menuSelector);
            if (element != null) return fallback;
            var text = string.Concat(MenuContent, Environment.NewLine, body.InnerHtml);
            body.InnerHtml = text;
            return node.OuterHtml;
        }
        private static string AppendHandlerJs(HtmlDocument doc, string fallback)
        {
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var content = body.InnerHtml;
            if (content.Contains(jsHandlerTag)) return node.OuterHtml;
            content += (Environment.NewLine + jsHandlerTag);
            body.InnerHtml = content;
            return node.OuterHtml;
        }
        private static string ReplaceHeadCss(HtmlDocument doc, string fallback)
        {
            var node = doc.DocumentNode;
            var head = node.SelectSingleNode(HtmlSelectors.HeadTag);
            if (head == null) return fallback;
            head_css_names.ForEach(h =>
            {
                var find = HtmlSelectors.GetNamedStyleTag(h);
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
        private static string RemoveVerifyJs(HtmlDocument doc, string fallback)
        {
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var scripts = body.SelectSingleNode(HtmlSelectors.JsVerifyScriptTag);
            if (scripts == null) return fallback;
            body.RemoveChild(scripts);
            return node.OuterHtml;

        }
        private static string RenameHomeCommonJs(HtmlDocument doc, string fallback)
        {
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var scripts = body.SelectNodes(HtmlSelectors.JsWithoutNameScriptTag);
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
        private static string ReplaceBodyJs(HtmlDocument doc, string fallback)
        {
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            body_js_names.ForEach(h =>
            {
                var find = HtmlSelectors.GetNamedSriptTag(h);
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
        private static string RemoveUnusedJs(HtmlDocument doc, string fallback)
        {
            var node = doc.DocumentNode;
            var scripts = node.SelectNodes(HtmlSelectors.JsWithoutNameScriptTag);
            if (scripts == null) return fallback;
            var items = scripts.ToList()
                .FindAll(x => !string.IsNullOrWhiteSpace(x.InnerHtml))
                .FindAll(x => HtmlSelectors.IsJsExclusion(x.InnerHtml));
            if (items.Count == 0) return fallback;
            items.ForEach(obj =>
            {
                var parentNode = obj.ParentNode;
                parentNode.RemoveChild(obj);
            });
            return node.OuterHtml;
        }

        [ExcludeFromCodeCoverage]
        private static string Execute(string content)
        {
            var fallback = new StringBuilder(content).ToString();
            try
            {
                content = CommonSubstition(content);
                var doc = GetDocument(content);
                if (doc == null) return content;
                content = AppendSideMenu(doc, content);
                content = AppendHandlerJs(doc, content);
                content = ReplaceHeadCss(doc, content);
                content = AppendMenuHeadCss(doc, content);
                content = RemoveVerifyJs(doc, content);
                content = RenameHomeCommonJs(doc, content);
                content = ReplaceBodyJs(doc, content);
                content = RemoveUnusedJs(doc, content);
                return content;
            }
            catch
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

        private static string? menuContent;
        private static string GetMenuContent => Properties.Resources.base_menu;
        private static string MenuContent => menuContent ??= GetMenuContent;


        private static string? indexContent;
        private static string GetIndexContent => Properties.Resources.index_page;
        private static readonly string linkRelative = "<link name=\"{0}\" href=\"/css/{0}.css\" rel=\"stylesheet\" />";
        private static readonly string scriptTag = "<script name=\"{0}\" src=\"/js/{0}.js\"></script>";
        private static readonly string jsHandlerTag = "<script name=\"handler_js\" src=\"/js/handler.js\"></script>";

        [ExcludeFromCodeCoverage(Justification = "Behavior tested from public accessor")]
        protected static class HtmlSelectors
        {
            public const string BodyTag = "//body";
            public const string HeadTag = "//head";
            public const string JsVerifyScriptTag = "//script[@name='verify-and-post']";
            public const string JsWithoutNameScriptTag = "//script[not(@name)]";
            public static string GetNamedStyleTag(string name)
            {
                return $"//style[@name='{name}']";
            }
            public static string GetNamedSriptTag(string name)
            {
                return $"//script[@name='{name}']";
            }
            public static bool IsJsExclusion(string? jscript)
            {
                if (string.IsNullOrEmpty(jscript)) return false;
                var isfound = false;
                js_exclusion_keywords.ForEach(keyword =>
                {
                    if (!isfound && jscript.Contains(keyword, StringComparison.OrdinalIgnoreCase)) isfound = true;
                });
                return isfound;
            }
            private static readonly List<string> js_exclusion_keywords = [
                "function commonClientScript()",
                "CefSharp"
            ];
        }
    }
}
