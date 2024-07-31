using HtmlAgilityPack;

namespace next.web.core.services
{
    internal class ContentSanitizerMyAccount : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var html = base.Sanitize(content);
            var doc = GetDocument(html);
            if (doc == null) return html;
            html = RenameCommonJs(doc, html);
            html = RenameValidationJs(doc, html);
            return html;
        }

        private static string RenameCommonJs(HtmlDocument doc, string fallback)
        {
            const string nodeName = "home_common";
            const string commonName = "my-account-common";
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return fallback;
            var find = HtmlSelectors.GetNamedSriptTag(nodeName);
            var homenode = node.SelectSingleNode(find);
            if (homenode == null) return fallback;
            homenode.Attributes["name"].Value = commonName;
            homenode.Attributes["src"].Value = $"/js/{commonName}.js";
            return node.OuterHtml;
        }

        private static string RenameValidationJs(HtmlDocument doc, string fallback)
        {
            const string validationName = "my-account-validate";
            const string commentLine = "<!-- script: my-account-form-validation -->";
            var node = doc.DocumentNode;
            var scripts = node.SelectNodes(HtmlSelectors.JsWithoutNameScriptTag);
            if (scripts == null) return fallback;
            var items = scripts.ToList()
                .FindAll(x => !string.IsNullOrWhiteSpace(x.InnerHtml))
                .FindAll(x => IsJsValidationExclusion(x.InnerHtml));
            if (items.Count == 0) return fallback;
            items.ForEach(obj =>
            {
                var parentNode = obj.ParentNode;
                parentNode.RemoveChild(obj);
            });
            var find = HtmlSelectors.GetNamedSriptTag(validationName);
            var acctnode = node.SelectSingleNode(find);
            if (acctnode != null) return node.OuterHtml;
            var body = node.SelectSingleNode(HtmlSelectors.BodyTag);
            if (body == null) return node.OuterHtml;
            var newScript = string.Format("<script name=\"{0}\" src=\"/js/{0}.js\"></script>", validationName);
            var hbody = body.InnerHtml;
            hbody = hbody.Replace(commentLine, "");
            hbody += (Environment.NewLine + newScript);
            body.InnerHtml = hbody;
            return node.OuterHtml;
        }
        private static bool IsJsValidationExclusion(string? jscript)
        {
            if (string.IsNullOrEmpty(jscript)) return false;
            var isfound = false;
            validation_common_keywords.ForEach(keyword =>
            {
                if (!isfound && jscript.Contains(keyword, StringComparison.OrdinalIgnoreCase)) isfound = true;
            });
            return isfound;
        }

        private static readonly List<string> validation_common_keywords = [
            "let permissions_general = {",
            "let permissions_data = { ",
            "function permisionChangeRequested()",
            "function changePasswordSubmitButtonClicked()",
            "function changePermissionsButtonClicked()"
        ];
    }
}
