using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Rewrite;
using next.web.Controllers;
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
            // Add services to the container.
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(15));
            services.AddSingleton(a => api);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void ConfigureApp(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.SetExceptionAndHsts(app.Environment.IsDevelopment());
            app.UseHttpsRedirection();
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
                pattern: "{controller=Home}/{action=Test}/{id?}");
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
            api ??= permissions == null ? new UnavailableApiWrapper() : new ApiWrapper(permissions);
            return api;
        }
    }
}
