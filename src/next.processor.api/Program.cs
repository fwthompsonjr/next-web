using Microsoft.AspNetCore.Rewrite;
using next.processor.api.services;
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
            services.AddControllersWithViews();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
            app.Run();
        }
    }
}