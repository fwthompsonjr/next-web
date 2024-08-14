using Microsoft.AspNetCore.Http;
using next.web.core.interfaces;
using System.Text;

namespace next.web.core.services
{
    public class SessionStringWrapper(ISession session) : ISessionStringWrapper
    {
        private readonly ISession _session = session;

        public string? GetString(string key)
        {
            return _session.GetString(key);
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public void SetString(string key, string value)
        {
            _session.SetString(key, value);
        }
    }
}
