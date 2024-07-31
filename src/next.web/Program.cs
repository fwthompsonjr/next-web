using Microsoft.AspNetCore.Rewrite;
using next.web.core.util;
using next.web.Services;
using System.Diagnostics.CodeAnalysis;

namespace next.web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            // Add services to the container.
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var app = builder.Build();
            AppContainer.Build(app.Services);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}