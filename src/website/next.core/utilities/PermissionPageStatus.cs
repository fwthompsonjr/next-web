using next.core.entities;
using next.core.interfaces;
using System.Runtime.Caching;

namespace next.core.utilities
{
    internal class PermissionPageStatus : PermissionApi
    {
        public PermissionPageStatus(string baseUri) : base(baseUri)
        {
        }

        public PermissionPageStatus(string baseUri, IInternetStatus status) : base(baseUri, status)
        {
        }

        protected ApiResponse PostAddress(string name, UserBo user)
        {
            var pageName = PostAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(pageName)) { return new ApiResponse { StatusCode = 404, Message = "Invalid page address." }; }
            if (!user.IsInitialized) { return new ApiResponse { StatusCode = 400, Message = "Invalid user state. Please initialize user context." }; }
            var address = string.Format(PostAddresses[pageName], _baseUri);
            return new ApiResponse
            {
                StatusCode = 200,
                Message = address
            };
        }

        protected ApiResponse GetAddress(string name)
        {
            var pageName = GetAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(pageName)) { return new ApiResponse { StatusCode = 404, Message = "Invalid page address." }; }
            var address = string.Format(GetAddresses[pageName], _baseUri);
            return new ApiResponse
            {
                StatusCode = 200,
                Message = address
            };
        }

        protected override bool GetConnectionStatus(string name, string address)
        {
            var keyName = string.Format(PageKeyName, name);
            var memoryCache = MemoryCache.Default;
            var url = GetUrl(address);

            if (!memoryCache.Contains(keyName))
            {
                var expiration = DateTimeOffset.UtcNow.AddMinutes(15);
                memoryCache.Add(keyName, CanConnectToPage(url), expiration);
            }

            return Convert.ToBoolean(memoryCache.Get(keyName));
        }
    }
}