using legallead.desktop.entities;

namespace next.web.core.interfaces
{
    internal interface IAuthorizedUserService
    {
        string? UserName { get; }
        UserBo? Current { get; }
        void Populate(string keyName, string keyValue);
        string? Retrieve(string keyName);
    }
}
