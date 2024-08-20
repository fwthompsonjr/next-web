using Microsoft.AspNetCore.Rewrite;
using next.processor.api.services;

namespace next.processor.api
{
    internal static class ProgramExtensions
    {
        public static void Configure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.Configure<RouteOptions>(
                options => options.LowercaseUrls = true);
        }

        public static void ConfigureApp(this WebApplication app)
        {
            var isDevelopment = app.Environment.IsDevelopment();
            app.SetSwaggerOptions(isDevelopment);

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseRouting();
            // enforce lowercase URLs
            // by redirecting uppercase urls to lowercase urls
            var options = new RewriteOptions().Add(new RedirectLowerCaseRule());
            app.UseRewriter(options);
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }


        public static void SetSwaggerOptions(this WebApplication app, bool isDevelopment)
        {
            if (!isDevelopment) { return; }
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}