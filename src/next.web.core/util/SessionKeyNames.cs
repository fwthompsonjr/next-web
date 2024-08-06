namespace next.web.core.util
{
    internal static class SessionKeyNames
    {
        public const string UserBo = "user_current_business_object";
        public const string UserIdentity = "user_current_identity";
        public const string UserMailbox = "user_current_mail_box";
        public const string UserMailboxItemFormat = "user_mail_{0}";
        public const string UserRestriction = "user_restrictions";
        public const string UserSearchActive = "user_search_active";
        public const string UserSearchHistory = "user_search_history";
        public const string UserSearchPurchases = "user_search_purchases";
        public const string UserSearchHistoryFilter = "user_search_history_filter";
        public const string UserSearchPurchaseFilter = "user_search_purchase_filter";
        public const string UserSearchActiveFilter = "user_search_active_filter";

    }
    public enum SearchFilterNames
    {
        History = 0,
        Purchases = 1,
        Active = 2
    }
}
