using legallead.desktop.entities;
using legallead.desktop.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.extensions
{
    internal static class MapOperations
    {
        [ExcludeFromCodeCoverage(Justification = "Wrapper function tested elsewhere.")]
        public static async Task<string> MapProfileResponse(this UserBo user, IPermissionApi api, IUserProfileMapper service, string response)
        {
            try
            {
                var resp = await service.Map(api, user, response);
                return resp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return response;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Wrapper function tested elsewhere.")]
        public static async Task<string> MapPermissionsResponse(this UserBo user, IPermissionApi api, IUserPermissionsMapper service, string response)
        {
            try
            {
                var resp = await service.Map(api, user, response);
                return resp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return response;
            }
        }

    }
}
