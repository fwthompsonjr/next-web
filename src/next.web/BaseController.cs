using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web
{
    public abstract class BaseController : Controller
    {

        internal static async Task<string> GetAuthenicatedPage(ISession? session, string pageName)
        {
            var isValid = HasUserToken(session);
            var user = GetUserToken(session);
            if (user != null)
            {
                isValid &= !string.IsNullOrEmpty(user.UserName);
                if (isValid) isValid &= user.IsAuthenicated;
            }
            var newName = isValid ? pageName : "home";
            var content = RemoveHeaderDuplicate(GetPageOrDefault(newName));
            content = AlterMenuBorder(content, pageName);
            if (!isValid) return content;
            var provider = AppContainer.ServiceProvider;
            var api = provider?.GetService<IPermissionApi>();
            var profiles = provider?.GetService<IUserProfileMapper>();
            var permissions = provider?.GetService<IUserPermissionsMapper>();
            if (api == null || user == null || profiles == null || permissions == null) return content;
            if (newName.StartsWith("myaccount"))
            {
                content = await user.MapProfileResponse(api, profiles, content);
                content = await user.MapPermissionsResponse(api, permissions, content);
            }
            if (session != null)
            {
                var dc = content.ToHtml();
                await session.AppendStatus(api, dc);
                content = dc.DocumentNode.OuterHtml;
            }
            
            return RemoveHeaderDuplicate(content);
        }

        private static string AlterMenuBorder(string content, string pageName)
        {
            const string clsname = "alternate-border";
            var alterations = new List<string>() { "mailbox", "viewhistory" };
            if (!alterations.Contains(pageName)) return content;
            var doc = content.ToHtml();
            var border = doc.DocumentNode.SelectSingleNode("//*[@id='app-side-menu-border']");
            if (border == null) return content;
            var clsattribute = border.Attributes.ToList().Find(x => x.Name == "class");
            if (clsattribute != null)
            {
                var items = clsattribute.Value.Split(' ').ToList();
                if (!items.Contains(clsname)) items.Add(clsname);
                clsattribute.Value = string.Join(" ", items);
                return doc.DocumentNode.OuterHtml;
            }
            border.Attributes.Add("class", clsname);
            return doc.DocumentNode.OuterHtml;
        }

        internal static bool IsSessionAuthenicated(ISession? session)
        {
            var user = GetUserToken(session);
            if (user == null) return false;
            var isValid = !string.IsNullOrEmpty(user.UserName);
            if (isValid) isValid &= user.IsAuthenicated;
            return isValid;
        }
        internal static string? GetUserId(ISession? session)
        {
            return GetContextUser(session)?.UserId;
        }
        protected static string GetPageOrDefault(string pageName)
        {
            var content = ContentHandler.GetLocalContent(pageName);
            if (content == null || string.IsNullOrEmpty(content.Content)) { return Introduction; }
            return content.Content;
        }

        private static bool HasUserToken(ISession? session)
        {
            var obj = GetUserToken(session);
            return obj != null;
        }
        private static UserBo? GetUserToken(ISession? session)
        {
            var contextBo = GetContextUser(session);
            if (contextBo == null) return null;
            return contextBo.ToUserBo();
        }

        private static UserContextBo? GetContextUser(ISession? session)
        {
            var keyName = SessionKeyNames.UserBo;
            if (session == null) return null;
            if (!session.TryGetValue(keyName, out var bytes)) return null;
            if (bytes.Length == 0) return null;
            var converted = Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(converted)) return null;
            return converted.ToInstance<UserContextBo>();
        }

        protected static string Introduction => _introduction ??= GetIntroduction();
        private static string? _introduction;
        private static string GetIntroduction()
        {
            var content = ContentHandler.GetLocalContent("home");
            if (content == null || string.IsNullOrWhiteSpace(content.Content)) return string.Empty;
            return content.Content;
        }

        protected static string RemoveHeaderDuplicate(string content)
        {
            const string find = "Oxford Oxford";
            const string replace = "Oxford";
            const string mappers = "styles,scripts";
            var maps = mappers.Split(',');
            while (content.Contains(find))
            {
                content = content.Replace(find, replace);
            }
            content = AppendHistoryJs(content);
            

            foreach (var map in maps)
            {
                var mapper = AppContainer.GetReMapper(map);
                if (mapper == null) continue;
                content = mapper.Map(content);
            }
            return content;
        }


        protected static ContentResult GetResult(string content)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                Content = RemoveHeaderDuplicate(content)
            };
        }

        private static string AppendHistoryJs(string content)
        {
            var sqte = (char)39;
            const char dbl = '"';
            var document = content.ToHtml();
            var node = document.DocumentNode;
            var head = node.SelectSingleNode("//head");
            if (head == null) return content;
            var js = head.SelectSingleNode("//script[@name='site']");
            if (js != null) return content;
            var builder = new StringBuilder(head.InnerHtml);
            builder.AppendLine();
            builder.AppendLine(historyjs.Replace(sqte, dbl));
            head.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }
        private const string historyjs = "<script name='site' src='/js/site.js'></script>";
    }
}
