using Microsoft.Extensions.DependencyInjection;
using next.core.entities;
using next.core.implementations;
using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.core.utilities
{
    internal class PermissionPageClient : PermissionPageStatus
    {
        public PermissionPageClient(string baseUri) : base(baseUri)
        {
        }

        public PermissionPageClient(string baseUri, IInternetStatus status) : base(baseUri, status)
        {
        }
        public override async Task<ApiResponse> Get(string name, Dictionary<string, string> parameters)
        {

            var fallback = new ApiResponse { StatusCode = 500, Message = "Unexpected Error" };
            var verify = await base.Get(name);
            verify ??= fallback;
            if (verify.StatusCode != 200) return verify;
            var address = GetAddress(name);
            if (address.StatusCode != 200) return address;
            var url = address.Message;
            foreach (var item in parameters)
            {
                url = url.Replace(item.Key, item.Value);
            }
            using var client = GetHttpClient();
            var result = await client.GetStringAsync(client.Client, url);
            if (string.IsNullOrEmpty(result))
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Unable to communicate with remote server"
                };
            }
            return new ApiResponse
            {
                StatusCode = 200,
                Message = result
            };
        }
        public override async Task<ApiResponse> Get(string name, UserBo user)
        {
            var fallback = new ApiResponse { StatusCode = 500, Message = "Unexpected Error" };
            var verify = await base.Get(name);
            verify ??= fallback;
            if (verify.StatusCode != 200) return verify;
            var address = GetAddress(name);
            if (address.StatusCode != 200) return address;
            var url = address.Message;
            using var client = GetHttpClient();
            client.AppendAuthorization(user);
            var result = await client.GetStringAsync(client.Client, url);
            if (string.IsNullOrEmpty(result))
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Unable to communicate with remote server"
                };
            }
            return new ApiResponse
            {
                StatusCode = 200,
                Message = result
            };
        }

        public override async Task<ApiResponse> Get(string name, UserBo user, Dictionary<string, string> parameters)
        {
            var fallback = new ApiResponse { StatusCode = 500, Message = "Unexpected Error" };
            var verify = await base.Get(name);
            verify ??= fallback;
            if (verify.StatusCode != 200) return verify;
            var address = GetAddress(name);
            if (address.StatusCode != 200) return address;
            var url = address.Message;
            foreach (var item in parameters)
            {
                url = url.Replace(item.Key, item.Value);
            }
            using var client = GetHttpClient();
            client.AppendAuthorization(user);
            var result = await client.GetStringAsync(client.Client, url);
            if (string.IsNullOrEmpty(result))
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Unable to communicate with remote server"
                };
            }
            return new ApiResponse
            {
                StatusCode = 200,
                Message = result
            };
        }
        public override async Task<ApiResponse> Get(string name)
        {
            var fallback = new ApiResponse { StatusCode = 500, Message = "Unexpected Error" };
            try
            {
                var verify = await base.Get(name);
                verify ??= fallback;
                if (verify.StatusCode != 200) return verify;
                var address = GetAddress(name);
                if (address.StatusCode != 200) return address;
                var url = address.Message;
                var user = GetUserOrDefault();
                using var client = GetHttpClient();
                client.AppendAuthorization(user);
                var result = await client.GetStringAsync(client.Client, url);
                if (string.IsNullOrEmpty(result))
                {
                    return new ApiResponse
                    {
                        StatusCode = 500,
                        Message = "Unable to communicate with remote server"
                    };
                }
                return new ApiResponse
                {
                    StatusCode = 200,
                    Message = result
                };
            }
            catch (Exception ex)
            {
                fallback.Message = ex.Message;
                return fallback;
            }
        }

        public override async Task<ApiResponse> Post(string name, object payload, UserBo user)
        {
            var fallback = new ApiResponse { StatusCode = 500, Message = "Unexpected Error" };
            try
            {
                var address = PostAddress(name, user);
                if (address.StatusCode != 200) return address;
                var url = address.Message;
                using var client = GetHttpClient();
                client.AppendAuthorization(user);
                client.AppendHeader("APP_IDENTITY", user.GetAppServiceHeader());
                var result = await client.PostAsJsonAsync(client.Client, url, payload);
                var content = await result.Content.ReadAsStringAsync();
                if (!result.IsSuccessStatusCode)
                {
                    return new ApiResponse
                    {
                        StatusCode = (int)result.StatusCode,
                        Message = content
                    };
                }
                return new ApiResponse
                {
                    StatusCode = (int)result.StatusCode,
                    Message = content
                };
            }
            catch (Exception ex)
            {
                fallback.Message = ex.Message;
                return fallback;
            }
        }

        protected virtual IHttpClientWrapper GetHttpClient()
        {
            var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) };
            var wrapper = new HttpClientWrapper(client);
            return wrapper;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public call.")]
        private static UserBo GetUserOrDefault()
        {
            try
            {
                var user = DesktopCoreServiceProvider.Provider.GetService<UserBo>();
                return user ?? new();
            }
            catch (Exception)
            {
                return new();
            }
        }
    }
}