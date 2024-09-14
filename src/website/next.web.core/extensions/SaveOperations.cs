using next.core.entities;
using next.core.interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.models;
using next.web.core.util;
using next.web.Services;
using System.Text;

namespace next.web.core.extensions
{
    internal static class SaveOperations
    {
        public static async Task SaveMail(
            this UserContextBo userbo,
            ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            const char gt = '<'; // greater than
            const char gtp = '('; // greater than replacement
            const char lt = '>'; // less than
            const char ltp = ')'; // less than replacement
            var user = userbo.ToUserBo();
            var mailjs =
                wrapper == null ?
                await GetMail(api, user) :
                await GetMail(wrapper, session);
            List<MailItem> collection = (string.IsNullOrWhiteSpace(mailjs) ? [] :
                mailjs.ToInstance<List<MailItem>>()) ?? [];
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

        public static async Task SaveRestriction(
            this UserContextBo userbo,
            ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            const string methodName = "search-get-restriction";
            var user = userbo.ToUserBo();
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response =
                wrapper == null ?
                await api.Post(methodName, payload, user) :
                ApiWrapper.MapTo(await wrapper.Post(methodName, payload, session));
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

        public static async Task SaveHistory(
            this UserContextBo userbo,
            ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            const string methodName = "search-get-history";
            var user = userbo.ToUserBo();
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response =
                wrapper == null ?
                await api.Post(methodName, payload, user) :
                ApiWrapper.MapTo(await wrapper.Post(methodName, payload, session));
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

        public static async Task SaveSearchPurchases(
            this UserContextBo userbo,
            ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            const string methodName = "search-get-purchases";
            var user = userbo.ToUserBo();
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response =
                wrapper == null ?
                await api.Post(methodName, payload, user) :
                ApiWrapper.MapTo(await wrapper.Post(methodName, payload, session));
            var searches = new List<MyPurchaseBo>();
            if (response != null && response.StatusCode == 200)
            {
                var temp = response.Message.ToInstance<List<MyPurchaseBo>>();
                if (temp != null) { searches = temp; }
            }
            var timed = new UserTimedCollection<List<MyPurchaseBo>>(
                searches, TimeSpan.FromSeconds(60));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(SessionKeyNames.UserSearchPurchases, Encoding.UTF8.GetBytes(json));
        }

        public static async Task SaveUserIdentity(
            this UserContextBo userbo,
            ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            var user = userbo.ToUserBo();
            var identity = wrapper == null ?
                await user.GetUserIdentity(api) :
                await session.GetUserIdentity(wrapper);
            var timed = new UserTimedCollection<UserIdentityBo>(identity, TimeSpan.FromMinutes(10));
            var json = JsonConvert.SerializeObject(timed);
            session.Set(SessionKeyNames.UserIdentity, Encoding.UTF8.GetBytes(json));
        }

        public static async Task Save(
            this UserContextBo userbo,
            ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            // reset all user cache objects
            await userbo.SaveUserIdentity(session, api, wrapper);
            await userbo.SaveMail(session, api, wrapper);
            await userbo.SaveHistory(session, api, wrapper);
            await userbo.SaveRestriction(session, api, wrapper);
            await userbo.SaveSearchPurchases(session, api, wrapper);
        }


        internal static async Task<string?> GetMail(IApiWrapper api, ISession session)
        {
            var payload = new { RequestType = "messages" };
            var response = await api.Post("message-list", payload, session);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }
        internal static async Task<string?> GetMail(IPermissionApi api, UserBo user)
        {
            if (!user.IsAuthenicated) return null;
            var payload = new { RequestType = "messages" };
            var response = await api.Post("message-list", payload, user);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }
        internal static string ToDdd(this string date)
        {
            const char comma = ',';
            if (string.IsNullOrEmpty(date) || !date.Contains(comma)) { return date; }
            var components = date.Split(comma);
            components[0] = components[0][..3];
            return string.Join(comma, components);
        }
    }
}
