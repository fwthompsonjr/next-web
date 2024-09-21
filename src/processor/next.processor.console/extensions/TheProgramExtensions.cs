using Microsoft.Extensions.DependencyInjection;
using next.processor.api.backing;
using next.processor.api.interfaces;
using next.processor.api.services;
using next.processor.api.utility;
using next.processor.models;

namespace next.processor.console.extensions
{
    public static class TheProgramExtensions
    {
        public static ServiceProvider GetServiceProvider()
        {
            lock (locker)
            {
                var provider = new ServiceCollection();
                provider.Configure();
                return provider.BuildServiceProvider();
            }
        }
        public static void Configure(this IServiceCollection services)
        {
            services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddTransient<IApiWrapper, ApiWrapperService>();
            services.AddTransient<IExcelGenerator, ExcelGenerator>();
            services.AddTransient<IWebInteractiveWrapper, WebInteractiveWrapper>();
            services.AddSingleton<IWebInstallOperation, WebInstallOperation>();
            services.AddSingleton<CheckContainerServices>();
            services.AddSingleton<CheckPostApiRequest>();
            services.AddSingleton<DrillDownModel>();
            // firefox installation
            services.AddKeyedSingleton<IWebContainerInstall, WebFireFoxLinuxInstall>("linux-firefox");
            services.AddKeyedSingleton<IWebContainerInstall, WebGeckoDriverInstall>("linux-geckodriver");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyInstall>("verification");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadCollin>("read-collin");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadDenton>("read-denton");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadHarris>("read-harris");
            services.AddKeyedSingleton<IWebContainerInstall, WebVerifyPageReadHarrisJp>("read-harris-jp");
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
            services.AddSingleton(s =>
            {
                var api = s.GetRequiredService<IApiWrapper>();
                return new NonPersonQueueService(api);
            });
            services.AddSingleton(TheSettingsProvider.Configuration);
            services.AddSingleton<InitializationService>();
            services.AddSingleton<HomeReportingService>();
            services.AddSingleton<IStatusChanger, StatusChangeService>();
            services.AddHostedService<SearchGenerationService>();
            services.AddHostedService<InitializationService>();
            services.AddHostedService<HomeReportingService>();
            services.AddHostedService<NonPersonQueueService>();
        }


        private static readonly object locker = new();
    }
}
