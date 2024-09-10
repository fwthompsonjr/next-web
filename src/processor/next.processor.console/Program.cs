using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using next.processor.api.backing;
using next.processor.api.utility;
using next.processor.console.extensions;
using System.Diagnostics.CodeAnalysis;

namespace next.processor.console
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static IServiceProvider? provider;
        private static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args).UseSystemd();
            builder.ConfigureServices(services =>
            {
                services.AddWindowsService();
                services.Configure();
            });

            var host = builder.Build();
            provider = host.Services;            
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            await host.RunAsync();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            if (provider == null) return;
            TerminateServices(provider);
        }

        private static void TerminateServices(IServiceProvider services)
        {
            TerminateBatch(services);
            TerminateReader(services);
        }
        
        private static void TerminateBatch(IServiceProvider services)
        {
            try
            {
                var config = services.GetRequiredService<IConfiguration>();
                config[Constants.KeyServiceInstallation] = "false";
                config[Constants.KeyQueueProcessEnabled] = "false";
            }
            catch (Exception)
            {
                // no action on failure
            }
        }
        
        private static void TerminateReader(IServiceProvider services)
        {
            try
            {
                var searchSvc = services.GetRequiredService<SearchGenerationService>();
                searchSvc.Dispose();
            }
            catch (Exception)
            {
                // no action on failure
            }
        }
    }
}