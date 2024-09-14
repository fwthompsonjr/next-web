using next.core.entities;

namespace next.core.interfaces
{
    internal interface IUserRestrictionMapper
    {
        Task<string> Map(IPermissionApi api, UserBo user, string source);
    }
}
