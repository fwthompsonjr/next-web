using Newtonsoft.Json;

namespace next.core.entities
{
    internal class UserBo
    {
        private AccessTokenBo? token;

        public virtual bool IsAuthenicated => Token != null && Token.Expires.HasValue && !IsExpired(Token.Expires.Value);
        public string UserName { get; set; } = string.Empty;
        public string SessionId { get; protected set; } = unset;
        public ApiContext[]? Applications { get; set; }
        public bool IsInitialized => Applications != null && Applications.Length > 0;

        public AccessTokenBo? Token
        {
            get { return token; }
            internal set
            {
                token = value;
                if (token == null || string.IsNullOrEmpty(token.AccessToken))
                {
                    SessionId = unset;
                }
                else
                {
                    SessionId = Guid.NewGuid().ToString().Split('-')[^1];
                }
                AuthenicatedChanged?.Invoke();
            }
        }

        public bool IsSessionTimeout()
        {
            var sessionid = GetSessionId();
            if (sessionid.Equals(unset)) return false;
            if (Token == null || !Token.Expires.HasValue) return false;
            var difference = Token.Expires.Value - DateTime.UtcNow;
            return Math.Abs(difference.TotalMinutes) < 4;
        }

        public bool IsSessionExpired()
        {
            var sessionid = GetSessionId();
            if (sessionid.Equals(unset)) return false;
            if (Token == null || !Token.Expires.HasValue) return false;
            var difference = Token.Expires.Value - DateTime.UtcNow;
            var isExpired = Math.Abs(difference.TotalMinutes) < 1;
            if (isExpired) Token = null;
            return isExpired;
        }

        public Action? AuthenicatedChanged { get; internal set; }

        public string GetAppServiceHeader()
        {
            if (Applications == null) return string.Empty;
            if (Applications.Length <= 0) return string.Empty;
            var item = Applications[0];
            return JsonConvert.SerializeObject(item);
        }

        private static bool IsExpired(DateTime expires)
        {
            var difference = expires - DateTime.UtcNow;
            return difference.TotalMinutes < 1;
        }

        internal virtual string GetSessionId()
        {
            if (Token == null || !Token.Expires.HasValue) SessionId = unset;
            if (Token != null && Token.Expires.HasValue && IsExpired(Token.Expires.Value)) SessionId = expiring;
            return SessionId;
        }
        private const string unset = "-unset-";
        private const string expiring = "-expiring-";
    }
}