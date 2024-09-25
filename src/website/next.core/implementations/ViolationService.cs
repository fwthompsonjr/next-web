using next.core.entities;
using next.core.extensions;
using next.core.interfaces;

namespace next.core.implementations
{
    public class ViolationService(bool allowBackDating = false) : IViolationService
    {
        private readonly bool _allowBackDating = allowBackDating;
        public void Append(ViolationBo incident)
        {
            lock (locker)
            {
                if (!_allowBackDating) incident.CreateDate = DateTime.UtcNow;
                violations.Add(incident);
            }
        }

        public void Expire()
        {
            lock (locker)
            {
                var currentDate = DateTime.UtcNow;
                violations.RemoveAll(x => x.ExpiryDate < currentDate);
            }
        }

        public bool IsViolation(ViolationBo incident)
        {
            lock (locker)
            {
                Expire();
                var ipaddress = incident.IpAddress;
                var sessionid = incident.SessionId;
                return violations.Check(ipaddress, sessionid);
            }
        }

        private readonly List<ViolationBo> violations = [];
        private static readonly object locker = new();
    }
}
