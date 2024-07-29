using legallead.desktop.entities;
using Microsoft.AspNetCore.Http;
using next.web.core.interfaces;

namespace next.web.core.services
{
    internal class AuthorizedUserService(IHttpContextAccessor? http) : IAuthorizedUserService
    {
        private readonly IHttpContextAccessor? _contextAccessor = http;

        public string? SessionId
        {
            get { return _contextAccessor?.HttpContext.Session?.Id; }
        }

        public string? UserName
        {
            get { return _contextAccessor?.HttpContext.User.Identity?.Name; }
        }

        public UserBo? Current => throw new NotImplementedException();

        public void Clear()
        {
            var session = _contextAccessor?.HttpContext.Session;
            session?.Clear();
        }

        public void Populate(string sessionId, string keyName, string keyValue)
        {
            throw new NotImplementedException();
        }
    }
}
