using next.core.entities;

namespace next.core.extensions
{
    public static class ViolationExtensions
    {
        public static bool Check(this List<ViolationBo> source, string ip, string sessionId)
        {
            if (CheckByIp(source, ip)) return true;
            return CheckBySession(source, sessionId);
        }

        public static bool CheckBySession(this List<ViolationBo> source, string sessionId)
        {
            var subset = source.FindAll(x => x.ExpiryDate > DateTime.UtcNow);
            var count = subset.Count(x => x.SessionId.Equals(sessionId, Oic));
            return count >= MaxViolations;
        }

        public static bool CheckByIp(this List<ViolationBo> source, string ip)
        {
            var subset = source.FindAll(x => x.ExpiryDate > DateTime.UtcNow);
            var count = subset.Count(x => x.IpAddress.Equals(ip, Oic));
            return count >= MaxViolations;
        }

        private const int MaxViolations = 5;
        private const StringComparison Oic = StringComparison.OrdinalIgnoreCase;
    }
}
