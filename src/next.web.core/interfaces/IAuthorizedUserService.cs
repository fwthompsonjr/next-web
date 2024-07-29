using legallead.desktop.entities;

namespace next.web.core.interfaces
{
    internal interface IAuthorizedUserService
    {
        string? SessionId { get; }
        string? UserName { get; }
        UserBo? Current { get; }
        void Clear();
        void Populate(string sessionId, string keyName, string keyValue);
    }
}
