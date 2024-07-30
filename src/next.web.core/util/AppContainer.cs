using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private static IServiceProvider? WebServices { get; set; }
        public static void Build(IServiceProvider? webServices = null)
        {
            lock (locker)
            {
                WebServices ??= webServices;
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
                if (ServiceProvider == null)
                {
                    var serviceCollection = new ServiceCollection();
                    ConfigureServices(serviceCollection);
                    ServiceProvider = serviceCollection.BuildServiceProvider();
                }
            }
        }
        internal static IContentSanitizer? GetSanitizer(string name)
        {
            if (ServiceProvider == null) Build();
            var svc = ServiceProvider?.GetKeyedService<IContentSanitizer>(name) ?? defaultSanitizer;
            return svc;
        }
        internal static IHttpContextAccessor? GetAccessor()
        {
            return WebServices?.GetService<HttpContextAccessor>();
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
            var keys = new[] {
              "stripe.payment:key",
              "stripe.payment:names:test",
              "stripe.payment:names:prod", }.ToList();
            var keyvalues = new List<string> { };
            foreach (var item in keys)
            {
                var value = configuration[item] ?? string.Empty;
                keyvalues.Add(value);
            }
            if (string.IsNullOrEmpty(keyvalues[0])) return string.Empty;
            return keyvalues[0] == "test" ? keyvalues[1] : keyvalues[2];
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var provider = DesktopCoreServiceProvider.Provider;
            services.AddTransient<IPermissionApi>(s => new PermissionPageClient(PermissionApiBase ?? string.Empty));
            services.AddSingleton<ISearchBuilder>(s =>
            {
                var api = s.GetRequiredService<IPermissionApi>();
                return new SearchBuilder(api);
            });
            if (provider == null) return;
            services.AddSingleton<IAuthorizedUserService>(s =>
            {
                var http = GetAccessor();
                return new AuthorizedUserService(http);
            });
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
            services.AddKeyedSingleton("default", defaultSanitizer);
        }
        private static readonly object locker = new();
        private static readonly string[] sourceArray = [
              "Permissions_API",
            "api.permissions:destination",
            "api.permissions:remote",
            "api.permissions:local"];
        private static readonly IContentSanitizer defaultSanitizer = new ContentSanitizerBase();
    }
}
