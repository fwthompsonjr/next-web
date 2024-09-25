namespace next.web.core.services
{
    internal class ContentSanitizerHome : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var doc = GetDocument(content);
            if (doc == null) return content;
            var html = DisplayMenuOptions(doc, content);
            return html;
        }

        public static string ApplyViolation(string content)
        {
            const char comma = ',';
            const string fmt = "//*[@id='{0}']";
            const string registers = "register-username,register-email,register-password,register-password-confirmation";
            const string logins = "form-login-submit,form-register-submit,username,login-password";
            const string dsb = "disabled";
            List<string> controls = [.. registers.Split(comma), .. logins.Split(comma)];
            var doc = GetDocument(content);
            if (doc == null) return content;
            var node = doc.DocumentNode;
            controls.ForEach(qry =>
            {
                var find = string.Format(fmt, qry);
                var element = node.SelectSingleNode(find);
                element?.SetAttributeValue(dsb, dsb);
            });
            return node.OuterHtml;
        }
    }
}
