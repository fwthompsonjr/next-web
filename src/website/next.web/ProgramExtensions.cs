using Microsoft.AspNetCore.Rewrite;
using next.core.implementations;
using next.core.interfaces;
using next.web.core.interfaces;
using next.web.core.services;
using next.web.core.util;
using next.web.Services;
using System.Diagnostics.CodeAnalysis;

namespace next.web
{
    public static class ProgramExtensions
    {
        public static void Configure(this IServiceCollection services, IApiWrapper? api = null)
        {
            AppContainer.Build();
            api = ConfigureApiWrapper(api);
            var violationService = new ViolationService();
            // Add services to the container.
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(15));
            services.AddSingleton(a => api);
            services.AddSingleton<ICountyAuthorizationService, CountyAuthorizationService>();
            services.AddSingleton<IViolationService>(a => violationService);
            services.AddSingleton<IAccountMapService, AccountMapService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBeautificationService, BeautificationService>();
            var provider = AppContainer.ServiceProvider;
            ConfigureJsHandlers(api, provider);
        }

        public static void ConfigureApp(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.SetExceptionAndHsts(app.Environment.IsDevelopment());
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            // enforce lowercase URLs
            // by redirecting uppercase urls to lowercase urls
            var options = new RewriteOptions().Add(new RedirectLowerCaseRule());
            app.UseRewriter(options);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Test}/{id?}",
                defaults: new { controller = "Home", action = "Test" });
        }

        public static void SetExceptionAndHsts(this WebApplication app, bool isDevelopment)
        {
            if (isDevelopment) { return; }
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested through public accessor.")]
        private static IApiWrapper ConfigureApiWrapper(IApiWrapper? api)
        {
            var permissions = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            var parser = AppContainer.ServiceProvider?.GetService<IBeautificationService>() ?? new BeautificationService();
            api ??= permissions == null ? new UnavailableApiWrapper() : new ApiWrapper(permissions, parser);
            return api;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested through public accessor.")]
        private static void ConfigureJsHandlers(IApiWrapper? api, IServiceProvider? provider)
        {
            if (provider == null) return;
            var handlers = provider.GetServices<IJsHandler>()?.ToList();
            if (handlers == null || handlers.Count == 0) return;
            handlers.ForEach(h => h.Wrapper = api);
        }
    }
}
