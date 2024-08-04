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
            var filter = new UserSearchFilterBo();
            json = JsonConvert.SerializeObject(filter);
            exists = session.Keys.ToList().Exists(x => x == SessionKeyNames.UserSearchHistoryFilter);
            if (exists) { session.Remove(SessionKeyNames.UserSearchHistoryFilter); }
            session.Set(SessionKeyNames.UserSearchHistoryFilter, Encoding.UTF8.GetBytes(json));
        }

        public static async Task SaveMail(this UserContextBo userbo, ISession session, IPermissionApi api)
        {
            const char gt = '<'; // greater than
            const char gtp = '('; // greater than replacement
            const char lt = '>'; // less than
            const char ltp = ')'; // less than replacement
            var user = userbo.ToUserBo();
            var mailjs = await GetMail(api, user);
            List<MailItem> collection = (string.IsNullOrWhiteSpace(mailjs) ? new() :
                mailjs.ToInstance<List<MailItem>>()) ?? new();
            var exists = session.Keys.ToList().Exists(x => x == SessionKeyNames.UserMailbox);
            if (exists) { session.Remove(SessionKeyNames.UserMailbox); }
            collection.ForEach(c =>
            {
                var createDate = c.CreateDate.HasValue ?
                    c.CreateDate.Value.ToString("f").ToDdd() 
                    : " - ";
                var toAddress = string.IsNullOrEmpty(c.ToAddress) ? string.Empty : c.ToAddress.Replace(gt, gtp).Replace(lt, ltp);
                var fromAddress = string.IsNullOrEmpty(c.FromAddress) ? string.Empty : c.FromAddress.Replace(gt, gtp).Replace(lt, ltp);
                c.PositionId = collection.IndexOf(c) + 1;
                c.ToAddress = toAddress;
                c.FromAddress = fromAddress;
                c.CreateDt = createDate;
            });
            var timed = new UserTimedCollection<List<MailItem>>(
                collection, TimeSpan.FromMinutes(5));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(SessionKeyNames.UserMailbox, Encoding.UTF8.GetBytes(json));
        }

        public static async Task SaveRestriction(this UserContextBo userbo, ISession session, IPermissionApi api)
        {
            var user = userbo.ToUserBo();
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response = await api.Post("search-get-restriction", payload, user);
            var restriction = new MySearchRestrictions
            {
                IsLocked = true,
                MaxPerMonth = 5,
                MaxPerYear = 5,
                Reason = "Unable to contact remote service",
                ThisMonth = 5,
                ThisYear = 5,
            };
            if (response != null && response.StatusCode == 200) 
            { 
                var temp = response.Message.ToInstance<MySearchRestrictions>();
                if (temp != null) { restriction = temp; }
            }
            var timed = new UserTimedCollection<MySearchRestrictions>(
                restriction, TimeSpan.FromSeconds(15));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(SessionKeyNames.UserRestriction, Encoding.UTF8.GetBytes(json));
        }

        public static async Task SaveHistory(this UserContextBo userbo, ISession session, IPermissionApi api)
        {
            var user = userbo.ToUserBo();
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response = await api.Post("search-get-history", payload, user);
            var searches = new List<UserSearchQueryBo>();
            if (response != null && response.StatusCode == 200)
            {
                var temp = response.Message.ToInstance<List<UserSearchQueryBo>>();
                if (temp != null) { searches = temp; }
            }
            var timed = new UserTimedCollection<List<UserSearchQueryBo>>(
                searches, TimeSpan.FromSeconds(60));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(SessionKeyNames.UserSearchHistory, Encoding.UTF8.GetBytes(json));
        }
        public static async Task<List<MailItem>> RetrieveMail(this ISession session, IPermissionApi api)
        {
            var key = SessionKeyNames.UserMailbox;
            var userbo = session.GetContextUser();
            if (userbo == null) { return []; }
            var exists = session.IsItemExpired<List<MailItem>>(key);
            if (!exists) { await userbo.SaveMail(session, api); }
            var data = session.GetTimedItem<List<MailItem>>(key);
            return data ?? [];
        }

        public static async Task<MySearchRestrictions> RetrieveRestriction(this ISession session, IPermissionApi api)
        {
            var key = SessionKeyNames.UserRestriction;
            var userbo = session.GetContextUser();
            if (userbo == null) { return new(); }
            var exists = session.IsItemExpired<MySearchRestrictions>(key);
            if (!exists) { await userbo.SaveRestriction(session, api); }
            var data = session.GetTimedItem<MySearchRestrictions>(key);
            return data ?? new();
        }

        public static async Task<List<UserSearchQueryBo>> RetrieveHistory(this ISession session, IPermissionApi api)
        {
            var key = SessionKeyNames.UserSearchHistory;
            var userbo = session.GetContextUser();
            if (userbo == null) { return []; }
            var exists = session.IsItemExpired<List<UserSearchQueryBo>>(key);
            if (!exists) { await userbo.SaveHistory(session, api); }
            var data = session.GetTimedItem<List<UserSearchQueryBo>>(key);
            return data ?? [];
        }

        public static UserSearchFilterBo RetrieveHistoryFilter(this ISession session)
        {
            var key = SessionKeyNames.UserSearchHistoryFilter;
            var userbo = session.GetContextUser();
            if (userbo == null) { return new(); }
            var exists = session.TryGetValue(key, out var filter);
            if (!exists) 
            { 
                var tmp = new UserSearchFilterBo();
                session.Set(key, Encoding.UTF8.GetBytes(tmp.ToJsonString()));
                return tmp;
            }
            var data = Encoding.UTF8.GetString(filter);
            return data.ToInstance<UserSearchFilterBo>() ?? new();
        }

        public static void UpdateHistoryFilter(this ISession session, UserSearchFilterBo filter)
        {
            var key = SessionKeyNames.UserSearchHistoryFilter;
            var exists = session.TryGetValue(key, out var _);
            if (exists) session.Remove(key);
            session.Set(key, Encoding.UTF8.GetBytes(filter.ToJsonString()));
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
            return GetContextUser(session)?.ToUserBo();
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



        private static UserContextBo? GetContextUser(this ISession session)
        {
            var exists = session.TryGetValue(SessionKeyNames.UserBo, out var user);
            if (!exists) { return null; }
            var data = Encoding.UTF8.GetString(user);
            return data.ToInstance<UserContextBo>();
        }

        public static bool IsItemExpired<T>(this ISession session, string keyName)
        {
            var exists = session.TryGetValue(keyName, out var bytes);
            if (!exists) return true;
            var data = Encoding.UTF8.GetString(bytes);
            var obj = data.ToInstance<UserTimedCollection<T>>();
            if (obj == null) return true;
            return obj.IsExpired();
        }

        public static T? GetTimedItem<T>(this ISession session, string keyName)
        {
            var exists = session.TryGetValue(keyName, out var bytes);
            if (!exists) return default;
            var data = Encoding.UTF8.GetString(bytes);
            var obj = data.ToInstance<UserTimedCollection<T>>();
            if (obj != null) return obj.GetValue();
            return default;
        }

        private static string ToDdd(this string date)
        {
            const char comma = ',';
            if (string.IsNullOrEmpty(date) || !date.Contains(comma)) { return date; }
            var components = date.Split(comma);
            components[0] = components[0].Substring(0, 3);
            return string.Join(comma, components);
        }

    }
}
