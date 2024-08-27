using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using next.processor.api.backing;
using next.processor.api.Health;
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
            services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddTransient<IApiWrapper, ApiWrapperService>();
            services.AddTransient<IExcelGenerator, ExcelGenerator>();
            services.AddTransient<IWebInteractiveWrapper, WebInteractiveWrapper>();
            services.AddSingleton<IWebInstallOperation, WebInstallOperation>();
            services.AddSingleton<CheckContainerServices>();
            services.AddSingleton<CheckPostApiRequest>();
            // firefox installation
            services.AddKeyedSingleton<IWebContainerInstall, WebFireFoxInstall>("firefox");
            services.AddKeyedSingleton<IWebContainerInstall, WebGeckoDriverInstall>("geckodriver");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyInstall>("verification");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadCollin>("read-collin");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadDenton>("read-denton");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadHarris>("read-harris");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadTarrant>("read-tarrant");

            // queue processes
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
            services.AddSingleton<IQueueExecutor, QueueExecutor>();
            services.AddSingleton(s =>
            {
                var queue = s.GetRequiredService<IQueueExecutor>();
                return new SearchGenerationService(queue);
            });
            services.AddHostedService<SearchGenerationService>();
            services.Configure<RouteOptions>(
                options => options.LowercaseUrls = true);

            RegisterHealthChecks(services);
        }

        public static void RegisterHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<CheckContainerServices>("INFRASTRUCTURE")
                .AddCheck<CheckPostApiRequest>("API");
        }

        public static void ConfigureApp(this WebApplication app)
        {
            var isDevelopment = app.Environment.IsDevelopment();
            app.SetSwaggerOptions(isDevelopment);

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            var statuscodes = new Dictionary<HealthStatus, int>()
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            };
            var health = new HealthCheckOptions { ResultStatusCodes = statuscodes };
            var details = new HealthCheckOptions
            {
                ResultStatusCodes = statuscodes,
                ResponseWriter = WriteHealthResponse.WriteResponseAsync
            };
            app.MapHealthChecks("/health", health);
            app.MapHealthChecks("/health-details", details);

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