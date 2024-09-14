using next.core.entities;
using next.core.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web.core.extensions
{
    internal static class RetrieveOperations
    {

        public static async Task<List<MailItem>> RetrieveMail(
            this ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            var key = SessionKeyNames.UserMailbox;
            var userbo = session.GetContextUser();
            if (userbo == null) { return []; }
            var expired = session.IsItemExpired<List<MailItem>>(key);
            if (expired)
            {
                await userbo.SaveMail(session, api, wrapper);
            }
            var data = session.GetTimedItem<List<MailItem>>(key);
            return data ?? [];
        }

        public static async Task<MySearchRestrictions> RetrieveRestriction(
            this ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            var key = SessionKeyNames.UserRestriction;
            var userbo = session.GetContextUser();
            if (userbo == null) { return new(); }
            var expired = session.IsItemExpired<MySearchRestrictions>(key);
            if (expired) { await userbo.SaveRestriction(session, api, wrapper); }
            var data = session.GetTimedItem<MySearchRestrictions>(key);
            return data ?? new();
        }

        public static async Task<List<UserSearchQueryBo>> RetrieveHistory(
            this ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            var key = SessionKeyNames.UserSearchHistory;
            var userbo = session.GetContextUser();
            if (userbo == null) { return []; }
            var expired = session.IsItemExpired<List<UserSearchQueryBo>>(key);
            if (expired) { await userbo.SaveHistory(session, api, wrapper); }
            var data = session.GetTimedItem<List<UserSearchQueryBo>>(key);
            return data ?? [];
        }

        public static async Task<List<MyPurchaseBo>> RetrievePurchases(
            this ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            var key = SessionKeyNames.UserSearchPurchases;
            var userbo = session.GetContextUser();
            if (userbo == null) { return []; }
            var expired = session.IsItemExpired<List<MyPurchaseBo>>(key);
            if (expired) { await userbo.SaveSearchPurchases(session, api, wrapper); }
            var data = session.GetTimedItem<List<MyPurchaseBo>>(key);
            return data ?? [];
        }

        public static UserSearchFilterBo RetrieveFilter(
            this ISession session,
            SearchFilterNames searchFilter = SearchFilterNames.History)
        {
            var key = searchFilter switch
            {
                SearchFilterNames.Purchases => SessionKeyNames.UserSearchPurchaseFilter,
                SearchFilterNames.Active => SessionKeyNames.UserSearchActiveFilter,
                _ => SessionKeyNames.UserSearchHistoryFilter
            };
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

        public static async Task<UserIdentityBo> RetrieveIdentity(
            this ISession session,
            IPermissionApi api,
            IApiWrapper? wrapper = null)
        {
            var key = SessionKeyNames.UserIdentity;
            var userbo = session.GetContextUser();
            if (userbo == null) { return new(); }
            var expired = session.IsItemExpired<UserIdentityBo>(key);
            if (expired)
            {
                await userbo.SaveUserIdentity(session, api, wrapper);
            }
            var data = session.GetTimedItem<UserIdentityBo>(key);
            return data ?? new();
        }

    }
}
