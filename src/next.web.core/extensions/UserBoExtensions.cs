using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web.core.extensions
{
    internal static class UserBoExtensions
    {
        public static void Save(this UserContextBo user, ISession session)
        {
            var json = JsonConvert.SerializeObject(user);
            var exists = session.Keys.ToList().Exists(x => x == SessionKeyNames.UserBo);
            if (exists) { session.Remove(SessionKeyNames.UserBo); }
            session.Set(SessionKeyNames.UserBo, Encoding.UTF8.GetBytes(json));
        }
        public static async Task SaveMail(this UserContextBo userbo, ISession session, IPermissionApi api)
        {
            var user = userbo.ToUserBo();
            var mailjs = await GetMail(api, user);
            List<MailItem> collection = (string.IsNullOrWhiteSpace(mailjs) ? new() :
                mailjs.ToInstance<List<MailItem>>()) ?? new();
            var exists = session.Keys.ToList().Exists(x => x == SessionKeyNames.UserMailbox);
            if (exists) { session.Remove(SessionKeyNames.UserMailbox); }
            var timed = new UserTimedCollection<List<MailItem>>(
                collection, TimeSpan.FromMinutes(5));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(SessionKeyNames.UserMailbox, Encoding.UTF8.GetBytes(json));
        }

        public static async Task<MailItemBody?> GetMailBody(this UserBo user, IPermissionApi api, string messageId)
        {
            if (!Guid.TryParse(messageId, out var _)) return null;
            var payload = new
            {
                MessageId = messageId,
                RequestType = "body"
            };
            var response = await api.Post("message-body", payload, user);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message.ToInstance<MailItemBody>();
        }

        public static UserBo? GetUser(this ISession session)
        {
            var exists = session.TryGetValue(SessionKeyNames.UserBo, out var user);
            if (!exists) { return null; }
            var data = Encoding.UTF8.GetString(user);
            var bo = data.ToInstance<UserContextBo>();
            return bo?.ToUserBo();
        }

        public static async Task<string> GetUserId(this UserBo user, IPermissionApi api)
        {
            const string landing = "get-contact-index";
            var payload = new { RequestType = "UserId" };
            var response = await api.Post(landing, payload, user);
            if (response.StatusCode != 200) return string.Empty;
            var data = response.Message.ToInstance<ContactProfileResponse>();
            if (data == null ||
                string.IsNullOrEmpty(data.Message) ||
                !Guid.TryParse(data.Message, out var _)) return string.Empty;
            return data.Message;
        }
        public static UserBo ToUserBo(this UserContextBo user)
        {
            var app = new List<ApiContext> { new() { Id = user.AppId, Name = user.AppName } }.ToArray();
            var token = new AccessTokenBo
            {
                AccessToken = user.Token ?? string.Empty,
                Expires = user.Expires ?? DateTime.MinValue,
                RefreshToken = user.RefreshToken ?? string.Empty
            };
            var response = new UserBo
            {
                Applications = app,
                Token = token,
                UserName = user.UserName,
            };
            return response;
        }
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





        private static async Task<string?> GetMail(IPermissionApi api, UserBo user)
        {
            if (!user.IsAuthenicated) return null;
            var payload = new { RequestType = "messages" };
            var response = await api.Post("message-list", payload, user);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }

    }
}
