using next.core.entities;

namespace next.core.interfaces
{
    internal interface IHistoryReader
    {
        Task<string?> GetHistory(IPermissionApi? api, UserBo? user);
    }
}
