using HtmlAgilityPack;
using legallead.desktop.entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.models;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;
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
            var filterNames = new List<string> {
                SessionKeyNames.UserSearchHistoryFilter,
                SessionKeyNames.UserSearchPurchaseFilter,
                SessionKeyNames.UserSearchActiveFilter
            };
            filterNames.ForEach(keyName =>
            {
                exists = session.Keys.ToList().Exists(x => x == keyName);
                if (exists) { session.Remove(keyName); }
                session.Set(keyName, Encoding.UTF8.GetBytes(json));
            });
        }
        public static void Save(this ISession session, PermissionChangedResponse response)
        {
            var key = SessionKeyNames.UserPermissionChanged;
            var exists = session.Keys.ToList().Exists(x => x == key);
            if (exists) { session.Remove(key); }
            var json = response.ToJsonString();
            session.Set(key, Encoding.UTF8.GetBytes(json));
        }

        public static T? Retrieve<T>(this ISession session, string key)
        {
            var exists = session.TryGetValue(key, out var value);
            if (!exists) return default;
            var json = Encoding.UTF8.GetString(value);
            return json.ToInstance<T>();
        }

        public static void UpdateFilter(this ISession session, UserSearchFilterBo filter, SearchFilterNames searchFilter = SearchFilterNames.History)
        {
            var key = searchFilter switch
            {
                SearchFilterNames.Purchases => SessionKeyNames.UserSearchPurchaseFilter,
                SearchFilterNames.Active => SessionKeyNames.UserSearchActiveFilter,
                _ => SessionKeyNames.UserSearchHistoryFilter
            };
            var exists = session.TryGetValue(key, out var _);
            if (exists) session.Remove(key);
            session.Set(key, Encoding.UTF8.GetBytes(filter.ToJsonString()));
        }

        public static UserBo? GetUser(this ISession session)
        {
            return GetContextUser(session)?.ToUserBo();
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

        public static UserContextBo? GetContextUser(this ISession session)
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

        public static DateTime? GetExpirationDate<T>(this ISession session, string keyName)
        {
            var exists = session.TryGetValue(keyName, out var bytes);
            if (!exists) return default;
            var data = Encoding.UTF8.GetString(bytes);
            var obj = data.ToInstance<UserTimedCollection<T>>();
            if (obj != null) return obj.ExpirationDate;
            return default;
        }

        [ExcludeFromCodeCoverage(Justification = "Function tested in intergration.")]
        public static void InjectSessionKeys(this ISession session, HtmlDocument document)
        {
            const string findtable = "//*[@id='detail-table']";
            const string findrows = "//table/tbody/tr[@name='detail-tbody-trow']";
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(findtable);
            var rows = node.SelectNodes(findrows);
            var tbody = table?.SelectSingleNode("tbody");
            if (table == null || tbody == null || rows == null || rows.Count == 0) return;
            var row = rows[0].OuterHtml;
            var data = new List<KeyNameDetail> {
                new (SessionKeyNames.UserMailbox, session),
                new (SessionKeyNames.UserSearchActive, session),
                new (SessionKeyNames.UserSearchPurchases, session),
                new (SessionKeyNames.UserSearchHistory, session),
                new (SessionKeyNames.UserIdentity, session)
            };
            // UserIdentityBo
            var builder = new StringBuilder(Environment.NewLine);
            data.ForEach(d =>
            {
                var indx = data.IndexOf(d);
                var target = indx switch
                {
                    0 => "correspondence",
                    4 => "identity",
                    _ => "history"
                };
                var template = row
                .Replace("~0", d.ItemName)
                .Replace("~1", d.ItemCount.ToString())
                .Replace("~2", d.ExpirationDate)
                .Replace("~3", d.ExpirationMinutes)
                .Replace("~4", target);
                builder.AppendLine(template);
            });
            tbody.InnerHtml = builder.ToString();
        }



    }
}
