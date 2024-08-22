using Microsoft.AspNetCore.Http;
using next.web.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace next.web.core.services
{
    [ExcludeFromCodeCoverage(Justification = "Wrapper class. All functions are pass through to a tested entity.")]
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
