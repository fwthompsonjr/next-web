using Microsoft.Extensions.DependencyInjection;
using next.core.entities;
using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;

namespace next.core.utilities
{
    internal class PermissionApi : IPermissionApi
    {
        protected readonly string _baseUri;
        protected readonly IInternetStatus? _connectionStatus;

        public PermissionApi(string baseUri)
        {
            _baseUri = baseUri.TrimSlash();
            var provider = DesktopCoreServiceProvider.Provider;
            if (provider == null) return;
            _connectionStatus ??= provider.GetService<IInternetStatus>();
        }

        public PermissionApi(string baseUri, IInternetStatus status) : this(baseUri)
        {
            _connectionStatus = status;
        }

        public IInternetStatus? InternetUtility => _connectionStatus;

        public ApiResponse CheckAddress(string name)
        {
            if (string.IsNullOrEmpty(_baseUri))
            {
                return new ApiResponse { StatusCode = 503, Message = "Base api address is missing or not defined." };
            }
            var pageName = GetAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            pageName ??= PostAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (pageName == null)
            {
                return new ApiResponse { StatusCode = 404, Message = "Invalid page address." };
            }
            if (!GetConnectionStatus(name, pageName))
            {
                return new ApiResponse { StatusCode = 503, Message = "Page is not available." };
            }
            return new ApiResponse { StatusCode = 200, Message = GetUrl(pageName) };
        }

        public KeyValuePair<bool, ApiResponse> CanGet(string name)
        {
            var internetOn = InternetUtility?.GetConnectionStatus() ?? true;
            if (!internetOn)
            {
                return new KeyValuePair<bool, ApiResponse>(false, nointernet);
            }
            var isGetPage = GetAddresses.Keys.Any(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (!isGetPage)
            {
                var notfound = new ApiResponse { StatusCode = 404, Message = "Invalid page address." };
                return new KeyValuePair<bool, ApiResponse>(false, notfound);
            }
            var addressCheck = CheckAddress(name);
            if (addressCheck.StatusCode != 200) return new KeyValuePair<bool, ApiResponse>(false, addressCheck);
            return new KeyValuePair<bool, ApiResponse>(true, addressCheck);
        }

        public KeyValuePair<bool, ApiResponse> CanPost(string name, object payload, UserBo user)
        {
            var postPage = PostAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (postPage == null)
            {
                var notfound = new ApiResponse { StatusCode = 404, Message = "Invalid page address." };
                return new KeyValuePair<bool, ApiResponse>(false, notfound);
            }
            if (!user.IsInitialized)
            {
                var current = new ApiResponse
                {
                    StatusCode = 500,
                    Message = "User account is not initialized. Please check application settings"
                };
                return new KeyValuePair<bool, ApiResponse>(false, current);
            }
            var address = GetUrl(postPage);
            var accepted = new ApiResponse
            {
                StatusCode = 200,
                Message = address
            };
            return new KeyValuePair<bool, ApiResponse>(true, accepted);
        }


        public virtual async Task<ApiResponse> Get(string name, Dictionary<string, string> parameters)
        {
            var resp = await Task.Run(() =>
            {
                return new ApiResponse { StatusCode = 500, Message = $"Invalid procedure call" };
            });
            return resp;
        }

        public virtual async Task<ApiResponse> Get(string name, UserBo user)
        {
            if (!user.IsAuthenicated)
            {
                return new ApiResponse { StatusCode = 401, Message = "Unauthorized" };
            }
            var canGet = await Get(name);
            return canGet;
        }

        public virtual async Task<ApiResponse> Get(string name)
        {
            try
            {
                var response = await Task.Run(() =>
                    {
                        var verify = CanGet(name);
                        if (!verify.Key) return verify.Value;
                        return new ApiResponse
                        {
                            StatusCode = 200,
                            Message = "API call is to be executed from derived class."
                        };
                    });
                return response;
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = 500, Message = ex.Message };
            }
        }

        public virtual async Task<ApiResponse> Get(string name, UserBo user, Dictionary<string, string> parameters)
        {
            var resp = await Task.Run(() =>
            {
                return new ApiResponse { StatusCode = 500, Message = $"Invalid procedure call" };
            });
            return resp;
        }
        public virtual async Task<ApiResponse> Post(string name, object payload, UserBo user)
        {
            try
            {
                var response = await Task.Run(() =>
                    {
                        var verify = CanPost(name, payload, user);
                        if (!verify.Key) return verify.Value;
                        return new ApiResponse
                        {
                            StatusCode = 200,
                            Message = "API call is to be executed from derived class."
                        };
                    });
                return response;
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = 500, Message = ex.Message };
            }
        }

        protected string GetUrl(string address)
        {
            var isGet = GetAddresses.ContainsKey(address);
            var pageFragment = isGet ? GetAddresses[address] : PostAddresses[address];
            return string.Format(pageFragment, _baseUri);
        }

        protected virtual bool GetConnectionStatus(string name, string address)
        {
            return true;
        }

        protected static readonly Dictionary<string, string> GetAddresses = new()
        {
            { "application-read-me", "{0}/api/application/read-me" },
            { "application-list", "{0}/api/application/apps" },
            { "application-state-configuration", "{0}/api/application/state-configuration" },
            { "payment-process-type", "{0}/api/payment/payment-process-type" },
            { "user-permissions-list", "{0}/api/lists/user-permissions" },
            { "user-us-county-list", "{0}/api/lists/us-county-list" },
            { "user-us-state-list", "{0}/api/lists/us-state-list" },
            { "user-zero-payment", "{0}/payment-result?sts=success&id=~0" },
            { "user-purchase-history", "{0}/api/search/list-my-purchases?userName=~0" }
        };

        protected static readonly Dictionary<string, string> PostAddresses = new()
        {
            { "signon-login", "{0}/api/signon/login" },
            { "signon-refresh", "{0}/api/signon/refresh-token" },
            { "signon-change-password", "{0}/api/signon/change-password" },
            { "application-register", "{0}/api/Application/register" },
            { "profile-get-contact-index", "{0}/api/profiles/get-contact-index" },
            { "profile-get-contact-identity", "{0}/api/profiles/get-contact-identity" },
            { "profile-get-contact-detail", "{0}/api/profiles/get-contact-detail" },
            { "profile-edit-contact-address", "{0}/api/profiles/edit-contact-address" },
            { "profile-edit-contact-email", "{0}/api/profiles/edit-contact-email" },
            { "profile-edit-contact-name", "{0}/api/profiles/edit-contact-name" },
            { "profile-edit-contact-phone", "{0}/api/profiles/edit-contact-phone" },
            { "permissions-change-password", "{0}/api/signon/change-password" },
            { "permissions-set-discount", "{0}/api/permissions/set-discount" },
            { "permissions-set-permission", "{0}/api/permissions/set-permission" },
            { "setting-application-key", "{0}/api/settings/appkey" },
            { "search-begin", "{0}/api/search/search-begin" },
            { "search-get-history", "{0}/api/search/my-searches" },
            { "search-get-preview", "{0}/api/search/my-search-preview" },
            { "search-get-invoice", "{0}/api/payment/create-checkout-session" },
            { "search-get-actives", "{0}/api/search/my-active-searches" },
            { "search-get-purchases", "{0}/api/search/my-purchases" },
            { "search-get-restriction", "{0}/api/search/my-restriction-status" },
            { "search-extend-restriction", "{0}/api/search/search-extend-restriction" },
            { "make-search-purchase", "{0}/payment-fetch-search" },
            { "reset-download", "{0}/rollback-download" },
            { "mailbox-message-body", "{0}/api/mailbox/message-body" },
            { "mailbox-message-count", "{0}/api/mailbox/message-count" },
            { "mailbox-message-list", "{0}/api/mailbox/message-list" }
        };

        protected static bool CanConnectToPage(string address, IPingAddress? ping = null)
        {
            ping ??= new PingAddress();
            try
            {
                var reply = ping.CheckStatus(address);
                if (reply != IPStatus.Success)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected const string PageKeyName = "page-{0}-status";

        private static readonly ApiResponse nointernet = new() { StatusCode = 503, Message = "Application is unable to connect to internet." };

        [ExcludeFromCodeCoverage(Justification = "This method is private and tested through public members.")]
        private sealed class PingAddress : IPingAddress
        {
            public IPStatus CheckStatus(string address)
            {
                if (!Uri.IsWellFormedUriString(address, UriKind.RelativeOrAbsolute))
                    return IPStatus.BadDestination;
                return IPStatus.Success;
            }
        }
    }
}