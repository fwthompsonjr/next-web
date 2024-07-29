using legallead.desktop.entities;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.util;
using System.Text;

namespace next.web.core.services
{
    internal class AuthorizedUserService(IHttpContextAccessor? http) : IAuthorizedUserService
    {
        private readonly IHttpContextAccessor? _contextAccessor = http;

        public string? UserName
        {
            get { return _contextAccessor?.HttpContext.User.Identity?.Name; }
        }

        public UserBo? Current
        {
            get
            {
                if (!IsAuthenicated()) return new();
                var data = Retrieve(SessionKeyNames.UserBo);
                if (string.IsNullOrWhiteSpace(data)) return null;
                return data.ToInstance<UserBo>();
            }
        }

        public void Populate(string keyName, string keyValue)
        {
            var session = _contextAccessor?.HttpContext.Session;
            if (session == null) return;
            session.Set(keyName, Encoding.UTF8.GetBytes(keyValue));
        }

        public string? Retrieve(string keyName)
        {
            if (!IsAuthenicated()) return null;
            var session = _contextAccessor?.HttpContext.Session;
            if (session == null) return null;
            if (!session.TryGetValue(keyName, out var value)) return null;
            return Encoding.UTF8.GetString(value);
        }

        private bool IsAuthenicated()
        {
            return _contextAccessor?.HttpContext.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
