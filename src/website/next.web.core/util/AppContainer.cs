using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using next.core.implementations;
using next.core.interfaces;
using next.core.utilities;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.services;

namespace next.web.core.util
{
    public static class AppContainer
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static IConfiguration? Configuration { get; private set; }

        public static string? PaymentSessionKey { get; private set; }
        public static string? PermissionApiBase { get; private set; }
        public static string? InitialViewName { get; private set; }
        public static string? PostLoginPage { get; private set; }
        public static void Build()
        {
            lock (locker)
            {
                if (Configuration == null)
                {
                    var builder = new CoreConfigurationModel();
                    Configuration = builder.GetConfiguration;
                }
                if (string.IsNullOrEmpty(PermissionApiBase))
                {
                    PermissionApiBase = GetPermissionApi(Configuration);
                }
                if (string.IsNullOrEmpty(PaymentSessionKey))
                {
                    PaymentSessionKey = GetPaymentKey(Configuration);
                }
                if (string.IsNullOrEmpty(InitialViewName))
                {
                    InitialViewName = Configuration["Initial_View"] ?? "introduction";
                }
                if (string.IsNullOrEmpty(PostLoginPage))
                {
                    PostLoginPage = Configuration["Post_Login_Page"] ?? "/my-account/home";
                }
                if (ServiceProvider == null)
                {
                    var serviceCollection = new ServiceCollection();
                    ConfigureServices(serviceCollection);
                    ServiceProvider = serviceCollection.BuildServiceProvider();
                }
            }
        }
        internal static IContentSanitizer GetSanitizer(string name)
        {
            if (ServiceProvider == null) Build();
            var svc = ServiceProvider?.GetKeyedService<IContentSanitizer>(name) ?? defaultSanitizer;
            return svc;
        }

        internal static IDocumentView? GetDocumentView(string name)
        {
            if (ServiceProvider == null) Build();
            var svc = ServiceProvider?.GetKeyedService<IDocumentView>(name);
            return svc;
        }

        internal static IReMapContent? GetReMapper(string name)
        {
            if (ServiceProvider == null) Build();
            var svc = ServiceProvider?.GetKeyedService<IReMapContent>(name);
            return svc;
        }

        private static string GetPermissionApi(IConfiguration configuration)
        {
            var keys = sourceArray.ToList();
            var keyvalues = new List<string> { };
            foreach (var item in keys)
            {
                var value = configuration[item] ?? string.Empty;
                keyvalues.Add(value);
            }
            if (string.IsNullOrEmpty(keyvalues[1])) return keyvalues[0];
            return keyvalues[1] == "local" ? keyvalues[3] : keyvalues[2];
        }

        private static string GetPaymentKey(IConfiguration configuration)
        {
            var keys = PaymentKeyNames;
            var keyvalues = new List<string> { };
            keys.ForEach(item =>
            {
                var value = configuration[item] ?? string.Empty;
                keyvalues.Add(value);
            });
            if (string.IsNullOrEmpty(keyvalues[0])) return string.Empty;
            return keyvalues[0] == "test" ? keyvalues[1] : keyvalues[2];
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var provider = DesktopCoreServiceProvider.Provider;
            var permissionapi = new PermissionPageClient(PermissionApiBase ?? string.Empty);
            services.AddTransient<IPermissionApi>(s => new PermissionPageClient(PermissionApiBase ?? string.Empty));
            services.AddSingleton<ISearchBuilder>(s =>
            {
                var api = s.GetRequiredService<IPermissionApi>();
                return new SearchBuilder(api);
            });
            services.AddTransient<IBeautificationService, BeautificationService>();
            if (provider == null) return;
            services.AddTransient(s => provider.GetRequiredService<IContentParser>());
            services.AddSingleton(s => provider.GetRequiredService<IInternetStatus>());
            services.AddSingleton(s => provider.GetRequiredService<IErrorContentProvider>());
            services.AddSingleton(s => provider.GetRequiredService<IUserProfileMapper>());
            services.AddSingleton(s => provider.GetRequiredService<IUserPermissionsMapper>());
            services.AddSingleton(s => provider.GetRequiredService<ICopyrightBuilder>());
            services.AddSingleton(s => provider.GetRequiredService<IQueueStopper>());
            services.AddSingleton(s => provider.GetRequiredService<IQueueStarter>());
            services.AddSingleton(s => provider.GetRequiredService<IMailPersistence>());
            services.AddSingleton(s => provider.GetRequiredService<IMailReader>());
            services.AddSingleton(s => provider.GetRequiredService<IHistoryReader>());
            services.AddSingleton(s => provider.GetRequiredService<IUserMailboxMapper>());
            services.AddSingleton(s => provider.GetRequiredService<CommonMessageList>());
            services.AddSingleton(s => provider.GetRequiredService<IHistoryPersistence>());
            // form submission handlers
            services.AddSingleton<IJsHandler, JsAuthenicateHandler>();
            services.AddSingleton<IJsHandler, JsRegistrationHandler>();
            services.AddSingleton<IJsHandler, JsSearchHandler>();
            // content view selectors
            services.AddKeyedSingleton<IDocumentView>("account-home", new DocumentViewAccount());
            services.AddKeyedSingleton<IDocumentView>("account-profile", new DocumentViewProfile());
            services.AddKeyedSingleton<IDocumentView>("account-permissions", new DocumentViewPermissions());
            services.AddKeyedSingleton<IDocumentView>("mysearch-home", new DocumentViewSearch());
            services.AddKeyedSingleton<IDocumentView>("mysearch-active", new DocumentViewSearchActive());
            services.AddKeyedSingleton<IDocumentView>("mysearch-purchases", new DocumentViewSearchPurchases());
            // content rewrite and beautify
            services.AddKeyedSingleton<IReMapContent, ReMapStyles>("styles");
            services.AddKeyedSingleton<IReMapContent, ReMapScripts>("scripts");

            // content formatters
            services.AddKeyedSingleton("default", defaultSanitizer);
            services.AddKeyedSingleton<IContentSanitizer>("post-login", new ContentSanitizerHome());
            services.AddKeyedSingleton<IContentSanitizer>("logout", new ContentSanitizerLogout());
            services.AddKeyedSingleton<IContentSanitizer>("myaccount", new ContentSanitizerMyAccount());
            services.AddKeyedSingleton<IContentSanitizer>("mailbox", new ContentSanitizerMailBox());
            services.AddKeyedSingleton<IContentSanitizer>("viewhistory", new ContentSanitizerHistory());
            services.AddKeyedSingleton<IContentSanitizer>("mysearch", new ContentSanitizerSearch());
            services.AddKeyedSingleton<IContentSanitizer>("invoice-subscription", new ContentSanitizerInvoice());
            services.AddKeyedSingleton<IContentSanitizer>("payment-confirmation", new ContentSanitizerConfirmation());
            services.AddKeyedSingleton<IContentSanitizer>("payment-checkout", new ContentSanitizerPayment());
            services.AddKeyedSingleton<IContentSanitizer>("download", new ContentSanitizerDownload());
            services.AddKeyedSingleton<IContentSanitizer>("cache-manager", new ContentSanitizerCache());
            // form submission handlers
            services.AddKeyedSingleton<IJsHandler, JsAuthenicateHandler>("form-login", (s, o) =>
            {
                var item = s.GetServices<IJsHandler>().FirstOrDefault(w => w.GetType() == typeof(JsAuthenicateHandler));
                if (item is JsAuthenicateHandler handler) return handler;
                return new JsAuthenicateHandler(permissionapi);
            });
            services.AddKeyedSingleton<IJsHandler, JsRegistrationHandler>("form-register", (s, o) =>
            {
                var item = s.GetServices<IJsHandler>().FirstOrDefault(w => w.GetType() == typeof(JsRegistrationHandler));
                if (item is JsRegistrationHandler handler) return handler;
                return new JsRegistrationHandler(permissionapi);
            });
            services.AddKeyedSingleton<IJsHandler, JsSearchHandler>("frm-search", (s, o) =>
            {
                var item = s.GetServices<IJsHandler>().FirstOrDefault(w => w.GetType() == typeof(JsSearchHandler));
                if (item is JsSearchHandler handler) return handler;
                return new JsSearchHandler(permissionapi);
            });
            var accounts = new List<string>();
            accounts.AddRange(ProfileForms);
            accounts.AddRange(PermissionForms);
            accounts.ForEach(acct =>
            {
                services.AddKeyedSingleton<IJsHandler, JsAccountHandler>(acct, (s, o) =>
                {
                    var item = s.GetServices<IJsHandler>().FirstOrDefault(w => w.GetType() == typeof(JsAccountHandler));
                    if (item is JsAccountHandler handler) return handler;
                    return new JsAccountHandler(permissionapi);
                });
            });

        }
        private static readonly object locker = new();
        private static readonly string[] sourceArray = [
              "Permissions_API",
            "api.permissions:destination",
            "api.permissions:remote",
            "api.permissions:local"];
        private static readonly IContentSanitizer defaultSanitizer = new ContentSanitizerBase();
        public static readonly List<string> ProfileForms =
        [
            "frm-profile-personal",
            "frm-profile-address",
            "frm-profile-phone",
            "frm-profile-email"
        ];
        public static readonly List<string> PermissionForms =
        [
            "permissions-subscription-group",
            "permissions-discounts",
            "form-change-password"
        ];
        internal static readonly Dictionary<string, string> AddressMap = new()
        {
            { "frm-profile-personal", "profile-edit-contact-name" },
            { "frm-profile-address", "profile-edit-contact-address" },
            { "frm-profile-phone", "profile-edit-contact-phone" },
            { "frm-profile-email", "profile-edit-contact-email" },
            { "Changes", "permissions-change-password" },
            { "Discounts", "permissions-set-discount" },
            { "Subscription", "permissions-set-permission" },
            { "permissions-set-permission", "permissions-set-permission" }
        };
        private static readonly List<string> PaymentKeyNames =
        [
          "stripe.payment:key",
          "stripe.payment:names:test",
          "stripe.payment:names:prod"
         ];
    }
}
