using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Runtime.Caching;

namespace next.core.utilities
{
    internal class InternetStatus : IInternetStatus
    {
        public bool GetConnectionStatus()
        {
            var memoryCache = MemoryCache.Default;

            if (!memoryCache.Contains(IsConnectedKeyName))
            {
                var expiration = DateTimeOffset.UtcNow.AddMinutes(5);
                var keyvalue = IsConnectedToInternet();
                memoryCache.Add(IsConnectedKeyName, keyvalue, expiration);
            }

            return ConvertResponse(memoryCache.Get(IsConnectedKeyName));
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested through public call.")]
        private static bool ConvertResponse(object? response)
        {
            if (response == null) return false;
            return Convert.ToBoolean(response);
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested through public call.")]
        private static bool IsConnectedToInternet()
        {
            string host = "google.com";
            bool result = false;
            Ping p = new();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch
            {
                return result;
            }
            return result;
        }

        private const string IsConnectedKeyName = "internet-connection-status";
    }
}