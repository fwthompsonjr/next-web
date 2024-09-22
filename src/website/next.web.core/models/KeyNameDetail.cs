using Microsoft.AspNetCore.Http;
using next.core.entities;
using next.web.core.extensions;
using next.web.core.util;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.models
{
    internal class KeyNameDetail
    {
        public KeyNameDetail(string keyName, ISession session)
        {
            if (!KeyNames.ContainsValue(keyName)) return;
            KeyIndex = KeyNames.FirstOrDefault(w => w.Value == keyName).Key;
            ItemName = KeyAlias[KeyIndex];
            var datePlus5 = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));
            var datePlus10 = DateTime.UtcNow.Add(TimeSpan.FromMinutes(10));
            var datePlus60 = DateTime.UtcNow.Add(TimeSpan.FromSeconds(60));
            var identityDate = session.GetExpirationDate<UserIdentityBo>(SessionKeyNames.UserIdentity);
            ExpirationDt = KeyIndex switch
            {
                1 => session.GetExpirationDate<List<MailItem>>(SessionKeyNames.UserMailbox) ?? datePlus5,
                2 => session.GetExpirationDate<List<UserSearchQueryBo>>(SessionKeyNames.UserSearchHistory) ?? datePlus60,
                3 => session.GetExpirationDate<List<MyPurchaseBo>>(SessionKeyNames.UserSearchPurchases) ?? datePlus60,
                4 => session.GetExpirationDate<List<UserSearchQueryBo>>(SessionKeyNames.UserSearchHistory) ?? datePlus60,
                5 => identityDate ?? datePlus10,
                _ => datePlus5
            };
            ItemCount = KeyIndex switch
            {
                1 => GetCount<MailItem>(session, SessionKeyNames.UserMailbox),
                2 => GetCount<UserSearchQueryBo>(session, SessionKeyNames.UserSearchHistory, true),
                3 => GetCount<MyPurchaseBo>(session, SessionKeyNames.UserSearchPurchases),
                4 => GetCount<UserSearchQueryBo>(session, SessionKeyNames.UserSearchHistory),
                5 => identityDate == null ? 0 : 1,
                _ => 0
            };

        }
        public int KeyIndex { get; }
        public string ItemName { get; } = string.Empty;
        public int ItemCount { get; }
        public DateTime ExpirationDt { get; }
        public string ExpirationDate => ExpirationDt.ToString(DateFormat);
        public string ExpirationMinutes => ExpirationDt.Subtract(DateTime.UtcNow).TotalMinutes.ToString("F3");

        [ExcludeFromCodeCoverage]
        private static int GetCount<T>(ISession session, string key, bool activeFilter = false)
        {
            var collection = session.Retrieve<UserTimedCollection<List<T>>>(key);
            var list = collection?.GetValue();
            if (list == null) { return 0; }
            if (!activeFilter || list is not List<UserSearchQueryBo> query) return list.Count;
            var filtered = query.FindAll(x => IsActive(x.SearchProgress));
            return filtered.Count;
        }

        private static readonly Dictionary<int, string> KeyNames = new() {
            { 1, SessionKeyNames.UserMailbox },
            { 2, SessionKeyNames.UserSearchActive },
            { 3, SessionKeyNames.UserSearchPurchases },
            { 4, SessionKeyNames.UserSearchHistory },
            { 5, SessionKeyNames.UserIdentity },
        };
        private static readonly Dictionary<int, string> KeyAlias = new() {
            { 1, "Correspondence" },
            { 2, "Active Searches" },
            { 3, "Purchased Searches" },
            { 4, "Search History" },
            { 5, "Identity" },
        };
        private static bool IsActive(string? searchProgress)
        {
            var find = new List<string> { "submitted", "processing" };
            if (string.IsNullOrEmpty(searchProgress)) return false;
            var exists = find.Exists(x => searchProgress.Contains(x, StringComparison.OrdinalIgnoreCase));
            return exists;
        }
        private const string DateFormat = "MMM dd yyyy - hh:mm:ss";
    }
}
