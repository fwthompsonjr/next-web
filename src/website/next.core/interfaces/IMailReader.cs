using next.core.entities;

namespace next.core.interfaces
{
    internal interface IMailReader
    {
        string? GetCount(IPermissionApi? api, UserBo? user);
        string? GetBody(IPermissionApi? api, UserBo? user, string messageId);
        string? GetMessages(IPermissionApi? api, UserBo? user);
    }
}
