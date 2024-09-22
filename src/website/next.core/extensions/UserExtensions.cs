using Microsoft.Extensions.DependencyInjection;
using next.core.entities;
using next.core.interfaces;
using next.core.utilities;
using System.Diagnostics.CodeAnalysis;

namespace next.core.extensions
{
    internal static class UserExtensions
    {
        public static void AppendAuthorization(this HttpClient client, UserBo user)
        {
            if (user.Token == null || string.IsNullOrEmpty(user.Token.AccessToken) || !user.IsAuthenicated) return;
            var token = user.Token.AccessToken;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public static async Task ExtendToken(this UserBo user, IPermissionApi? api)
        {
            if (api == null) return;
            if (user.Token == null || string.IsNullOrEmpty(user.Token.AccessToken)) return;
            var expiration = DateTime.UtcNow.Subtract(user.Token.Expires.GetValueOrDefault());
            if (expiration.TotalSeconds < -45) return;
            var token = user.Token;
            var payload = new { refreshToken = token.RefreshToken, accessToken = token.AccessToken };
            var response = await api.Post("refresh", payload, user);
            if (response == null || response.StatusCode != 200)
            {
                user.Token = null;
                return;
            }
            var newtoken = ObjectExtensions.TryGet<AccessTokenBo>(response.Message);
            if (!newtoken.Expires.HasValue)
            {
                user.Token = null;
                return;
            }
            user.Token = newtoken;
        }

        public static async Task<string> GetUserId(this UserBo user, IPermissionApi? api = null)
        {
            const string landing = "get-contact-index";
            api ??= GetApi(api);
            if (api == null) return string.Empty;
            var payload = new { RequestType = "UserId" };
            var response = await api.Post(landing, payload, user);
            if (response.StatusCode != 200) return string.Empty;
            var data = ObjectExtensions.TryGet<ContactProfileResponse>(response.Message);
            if (data == null ||
                string.IsNullOrEmpty(data.Message) ||
                !Guid.TryParse(data.Message, out var _)) return string.Empty;
            return data.Message;
        }
        public static async Task SetUserId(this UserBo user, IPermissionApi? api = null, IQueueFilter? queue = null)
        {
            var userId = await GetUserId(user, api);
            if (string.IsNullOrEmpty(userId)) return;
            var provider = DesktopCoreServiceProvider.Provider;
            queue ??= provider?.GetService<IQueueFilter>();
            if (queue == null) return;
            queue.Append(userId);
        }
        [ExcludeFromCodeCoverage(Justification = "Private member access only from public member.")]
        private static IPermissionApi? GetApi(IPermissionApi? api = null)
        {
            if (api != null) return api;
            var provider = DesktopCoreServiceProvider.Provider;
            return provider?.GetService<IPermissionApi>();
        }
    }
}