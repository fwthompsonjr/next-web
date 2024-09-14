using next.core.entities;

namespace next.core.interfaces
{
    internal interface IUserPermissionsMapper
    {
        Task<string> Map(IPermissionApi api, UserBo user, string source);
    }
}