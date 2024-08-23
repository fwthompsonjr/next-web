using Microsoft.AspNetCore.Rewrite;
using next.processor.api.backing;
using next.processor.api.interfaces;
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

            services.AddTransient<IApiWrapper, ApiWrapperService>();
            services.AddTransient<IExcelGenerator, ExcelGenerator>();
            services.AddTransient<IWebInteractiveWrapper, WebInteractiveWrapper>();
            services.AddKeyedTransient<IQueueProcess, QueueProcessBegin>("begin");
            services.AddKeyedTransient<IQueueProcess, QueueProcessParameter>("parameter");
            // search
            services.AddKeyedTransient<IQueueProcess, QueueProcessSearch>("search", (a, b) =>
            {
                var api = a.GetRequiredService<IApiWrapper>();
                var generator = a.GetRequiredService<IExcelGenerator>();
                var wrapper = a.GetRequiredService<IWebInteractiveWrapper>();
                return new QueueProcessSearch(api, generator, wrapper);
            });
            services.AddSingleton(s => s);
            services.AddTransient<IQueueExecutor, QueueExecutor>();

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