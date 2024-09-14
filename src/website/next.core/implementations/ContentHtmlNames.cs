using next.core.entities;
using next.core.interfaces;
using next.core.utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace next.core.implementations
{
    internal class ContentHtmlNames : IContentHtmlNames
    {
        private readonly ICopyrightBuilder? _copyrightBuilder;
        private ISearchBuilder? uiBuilder;

        public ContentHtmlNames()
        {
            _copyrightBuilder = DesktopCoreServiceProvider.Provider.GetService<ICopyrightBuilder>();
        }

        public List<ContentHtml> ContentNames => _contents;

        public List<string> Names => _names ??= GetNames();

        public ISearchBuilder? SearchUi
        {
            get { return uiBuilder; }
            set
            {
                uiBuilder = value;
                if (uiBuilder != null)
                {
                    var content = uiBuilder.GetHtml();
                    if (Replacements.ContainsKey(HtmMySearchInclude))
                    {
                        Replacements[HtmMySearchInclude] = content;
                    }
                }
            }
        }

        public static List<ContentReplacementItem> ContentReplacements => contentReplacementItems ??= GetContentReplacements();

        public bool IsValid(string name)
        {
            return Names.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        public ContentHtml? GetContent(string name)
        {
            const char minus = '-';
            if (!IsValid(name)) return null;
            var item = ContentNames
                    .Where(w => w.Name.Contains(minus))
                    .FirstOrDefault(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return null;
            if (string.IsNullOrEmpty(item.Content))
            {
                MapResourceContent(item);
            }
            item = TransformFooterCopyRight(item);
            return item;
        }

        private ContentHtml TransformFooterCopyRight(ContentHtml item)
        {
            if (_copyrightBuilder == null) { return item; }
            var content = item.Content;
            var hasFooter = content.Contains(HtmCommonFooterCopyRight);
            if (!hasFooter) { return item; }
            var copy = _copyrightBuilder.GetCopyright();
            var text = string.Format(HtmCommonFooterCopyRight, copy);
            content = content.Replace(HtmCommonFooterCopyRight, text);
            item.Content = content;
            return item;
        }

        public Stream GetContentStream(string name)
        {
            var item = GetContent(name);
            if (item == null || string.IsNullOrEmpty(item.Content))
                return new System.IO.MemoryStream();
            return GenerateStreamFromString(item.Content);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private List<string>? _names;

        private static List<string> GetNames()
        {
            const char minus = '-';
            var names = _contents.Select(x => x.Name.Split(minus)[0].ToLower()).ToList();
            return names;
        }

        internal static string CommonDialogueJs => Properties.Resources.commondialogscript_js;

        internal static string CommonReplacement(string? source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            var keys = Replacements.Keys;
            foreach (var key in from key in keys
                                where source.Contains(key)
                                select key)
            {
                source = source.Replace(key, Replacements[key]);
            }
            return source;
        }

        [ExcludeFromCodeCoverage]
        private static void MapResourceContent(ContentHtml? item)
        {
            if (item == null) return;
            if (!string.IsNullOrEmpty(item.Content)) return;
            if (_mappedcontents.Exists(x => x.Name == item.Name))
            {
                var mapped = _mappedcontents.Find(x => x.Name == item.Name);
                item.Content = CommonReplacement(mapped?.Content ?? string.Empty);
                return;
            }
            var manager = Properties.Resources.ResourceManager;
            var resourceText = CommonReplacement(manager.GetString(item.Name));
            if (string.IsNullOrEmpty(resourceText))
            {
                resourceText = CommonReplacement(manager.GetString(item.Name.Replace("-", "_")));
            }
            item.Content = resourceText;
        }

        private static string GetBaseCssScript()
        {
            var basecsstext = Properties.Resources.base_css;
            var builder = new StringBuilder("<style name=\"base-css\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</style>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static string GetLoginInclude()
        {
            var text = Properties.Resources.homelogin_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetRegistrationInclude()
        {
            var text = Properties.Resources.homeregister_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetWelcomeInclude()
        {
            var text = Properties.Resources.homewelcome_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetCommonCefInclude()
        {
            var text = Properties.Resources.commoncefhandler_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetCommonFooterInclude()
        {
            var text = Properties.Resources.commonfooter_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetCommonHeaderInclude()
        {
            var text = Properties.Resources.commonheadings_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetBootstrapCssScript()
        {
            var basecsstext = Properties.Resources.homelogin_html;
            var builder = new StringBuilder("<style name=\"base-css\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</style>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static string GetHomeValidationScript()
        {
            var basecsstext = Properties.Resources.homevalidation_js;
            var builder = new StringBuilder("<script name=\"home-form-validation\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</script>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static readonly List<ContentHtml> _contents = new()
        {
            new() { Index = -1, Name = "test"},
            new() { Index = 0, Name = "blank-html"},
            new() { Index = 10, Name = "base-css"},
            new() { Index = 20, Name = "commondialogue-html"},
            new() { Index = 30, Name = "commondialoguescript-js"},
            new() { Index = 100, Name = "introduction-html"},
            new() { Index = 110, Name = "home-html"},
            new() { Index = 110, Name = "homelogin-html"},
            new() { Index = 200, Name = "errorbox-css"},
            new() { Index = 300, Name = "myaccount-html"},
            new() { Index = 310, Name = "myaccounthome-html"},
            new() { Index = 315, Name = "myaccountpermissions-html"},
            new() { Index = 320, Name = "myaccountprofile-html"},
            new() { Index = 400, Name = "mysearch-html"},
            new() { Index = 405, Name = "mysearchtemplate-html"},
            new() { Index = 410, Name = "mysearchactive-html"},
            new() { Index = 500, Name = "invoice-html"},
            new() { Index = 600, Name = "mailbox-base-html"},
            new() { Index = 700, Name = "viewhistory-base-html"},
            new() { Index = 800, Name = "failed-purchase-html"},
        };

        private static readonly List<ContentHtml> _mappedcontents = new()
        {
            new() { Index = 20, Name = "commondialogue-html", Content = Properties.Resources.common_dialogue },
            new() { Index = 30, Name = "commondialoguescript-js", Content = CommonDialogueJs },
            new() { Index = 405, Name = "mysearchtemplate-html", Content = Properties.Resources.mysearchtemplate_html },
            new() { Index = 600, Name = "mailbox-base-html", Content = Properties.Resources.mailbox_base_html },
            new() { Index = 700, Name = "viewhistory-base-html", Content = Properties.Resources.viewhistory_base_html },
            new() { Index = 800, Name = "failed-purchase-html", Content = Properties.Resources.invoice_failure_html },
        };
        private const string CssBaseLink = "<link rel=\"stylesheet\" name=\"base\" href=\"css/base.css\" />";
        private const string CssBootStrapLink = "<link rel=\"stylesheet\" href=\"bootstrap.min.css\" />";
        private const string CssErrorBox = "<link rel=\"stylesheet\" name=\"errorbox\" href=\"css/error.css\">";
        private const string CssMyAccountInclude = "<!-- style: my-account-custom-css -->";
        private const string CssMyAccountSubContent = "<!-- style: my-account-subcontent-css -->";
        private const string CssMyActiveSearchContent = "<!-- style: my-active-searches-css -->";
        private const string JsCommonCefHandler = "<!-- script: common-cef-handler -->";
        private const string HtmAccountHomeInclude = "<p>My Account</p>";
        private const string HtmAccountPasswordInclude = "<p>My Password</p>";
        private const string HtmAccountProfileInclude = "<p>My Profile</p>";
        private const string HtmAccountPermissionsInclude = "<p>My Permissions</p>";
        private const string HtmAccountLogoutInclude = "<!-- component: my-account-logout -->";
        private const string HtmAccountReAuthenticate = "<!-- component: account-re-authenticate -->";
        private const string HtmCommonFooter = "<!-- block: common-footer -->";
        private const string HtmCommonFooterCopyRight = "<span id=\"footer-copy-span\">{0}</span>";
        private const string HtmCommonHeading = "<!-- block: common-headings -->";
        private const string HtmCommonMastHead = "<!-- block: common-mast-heading -->";
        private const string HtmLoginInclude = "<p>Login form</p>";
        private const string HtmWelcomeInclude = "<p>Welcome form</p>";
        private const string HtmRegistrationInclude = "<p>Registration form</p>";
        private const string HtmMySearchInclude = "<p>My Search Base</p>";
        private const string HtmMySearchHistory = "<p>My Search History</p>";
        private const string HtmMySearchPurchase = "<p>My Purchase History</p>";
        private const string HtmMySearchPreview = "<!-- component: my-search-preview -->";
        private const string HtmInvoiceInclude = "<p>Invoice form</p>";
        private const string JsCommonReload = "/* js-include-common-reload */";
        private const string JsCommonClientInclude = "<!-- script: common-client-include -->";
        private const string JsHomeValidation = "<!-- script: home-form-validation -->";
        private const string JsMyAccountNavigation = "<!-- script: my-account-navigation -->";
        private const string JsMyAccountProfile = "<!-- script: my-account-profile-valid -->";
        private const string JsMyAccountPermissions = "/* inject: permissions-validation script */";
        private const string JsMySearchBehavior = "<!-- script: my-search-searching-behaviors -->";
        private const string JsVerifyAndPost = "<!-- script: verify-and-post -->";
        private const string JsCloseInvoice = "<!-- script: invoice-submission-include -->";
        private const string JsMyActiveSearchScript = "<!-- script: my-active-searches-js -->";
        private const string JsReAuthenticateScript = "<!-- script: account-re-authenticate -->";
        private const string MailboxScriptBlock = "<!-- script: mailbox-behavior -->";
        private const string MailboxStyleSheet = "<!-- style: mailbox-css -->";
        private const string ViewHistoryScriptBlock = "<!-- script: search-history-behavior -->";
        private const string ViewHistoryStyleSheet = "<!-- style: search-history-css -->";
        private const string ViewHistoryItemTemplate = "<!-- include: search item template -->";

        private static readonly Dictionary<string, string> Replacements = new() {
            { CssBaseLink, GetBaseCssScript() },
            { CssBootStrapLink, GetBootstrapCssScript() },
            { CssErrorBox, Properties.Resources.errorbox_css },
            { CssMyAccountInclude, Properties.Resources.myaccount_css },
            { CssMyAccountSubContent, Properties.Resources.subcontent_css },
            { CssMyActiveSearchContent, Properties.Resources.mysearchactive_css },
            { HtmAccountHomeInclude, Properties.Resources.myaccount_home_html },
            { HtmAccountPasswordInclude, Properties.Resources.myaccount_password_html },
            { HtmAccountProfileInclude, Properties.Resources.myaccount_profile_html },
            { HtmAccountPermissionsInclude, Properties.Resources.myaccount_permissions_html },
            { HtmAccountLogoutInclude, Properties.Resources.myaccount_logout },
            { HtmAccountReAuthenticate, Properties.Resources.common_authenticate },
            { HtmMySearchInclude, Properties.Resources.mysearch_search_html },
            { HtmMySearchHistory, Properties.Resources.mysearch_history_html },
            { HtmMySearchPurchase, Properties.Resources.mysearch_purchases_html },
            { HtmMySearchPreview, Properties.Resources.mysearch_preview_html },
            { HtmLoginInclude, GetLoginInclude() },
            { HtmRegistrationInclude, GetRegistrationInclude() },
            { HtmInvoiceInclude, Properties.Resources.invoice_content_html },
            { JsHomeValidation, GetHomeValidationScript() },
            { JsCommonCefHandler, GetCommonCefInclude() },
            { JsMyAccountNavigation, Properties.Resources.myaccount_script_js },
            { JsMyAccountProfile, Properties.Resources.myaccount_profile_validation_js },
            { HtmCommonFooter, GetCommonFooterInclude() },
            { HtmCommonHeading, GetCommonHeaderInclude() },
            { HtmWelcomeInclude, GetWelcomeInclude() },
            { HtmCommonMastHead, Properties.Resources.common_mast_head_html },
            { JsCommonReload, Properties.Resources.commonreload_js },
            { JsCommonClientInclude, Properties.Resources.commonclientinjection_js },
            { JsMyAccountPermissions, Properties.Resources.myaccount_permissions_validation_js },
            { JsMySearchBehavior, Properties.Resources.mysearch_script_js },
            { JsVerifyAndPost, Properties.Resources.verifyandpost_js },
            { JsCloseInvoice, Properties.Resources.invoice_submission_js },
            { JsMyActiveSearchScript, Properties.Resources.mysearchactive_js },
            { JsReAuthenticateScript, Properties.Resources.common_authenticate_js },
            { MailboxScriptBlock, Properties.Resources.mailbox_base_js },
            { MailboxStyleSheet, Properties.Resources.mailbox_style_css },
            { ViewHistoryScriptBlock, Properties.Resources.viewhistory_base_js },
            { ViewHistoryStyleSheet, Properties.Resources.viewhistory_style_css },
            { ViewHistoryItemTemplate, Properties.Resources.viewhistory_item_template_html },
        };

        private static List<ContentReplacementItem>? contentReplacementItems = null;

        private static List<ContentReplacementItem> GetContentReplacements()
        {
            var list = new List<ContentReplacementItem>();
            var keys = Replacements.Keys.ToList();
            keys.ForEach(k =>
            {
                list.Add(new ContentReplacementItem { Key = k, Value = Replacements[k] });
            });
            return list;
        }
    }
}