using next.web.core.util;
using System.Diagnostics.CodeAnalysis;

namespace next.web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            AppContainer.Build();
            var services = builder.Services;
            services.Configure();
            var app = builder.Build();
            app.ConfigureApp();
            app.Run();
        }
    }
}