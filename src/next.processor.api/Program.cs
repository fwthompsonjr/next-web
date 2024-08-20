using System.Diagnostics.CodeAnalysis;

namespace next.processor.api
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            services.Configure();

            var app = builder.Build();
            app.ConfigureApp();
            app.Run();
        }
    }
}