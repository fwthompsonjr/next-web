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
            return RemoveHeaderDuplicate(content);
        }
        internal static bool IsSessionAuthenicated(ISession? session)
        {
            var user = GetUserToken(session);
            if (user == null) return false;
            var isValid = !string.IsNullOrEmpty(user.UserName);
            if (isValid) isValid &= user.IsAuthenicated;
            return isValid;
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
            var keyName = SessionKeyNames.UserBo;
            if (session == null) return null;
            if (!session.TryGetValue(keyName, out var bytes)) return null;
            if (bytes.Length == 0) return null;
            var converted = Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(converted)) return null;
            var contextBo = converted.ToInstance<UserContextBo>();
            if (contextBo == null) return null;
            return contextBo.ToUserBo();
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
            while (content.Contains(find))
            {
                content = content.Replace(find, replace);
            }
            return content;
        }
    }
}
