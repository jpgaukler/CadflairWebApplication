using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CadflairDataAccess;

namespace BlazorAppTest3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<IDataAccess, DataAccess>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/_Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            //use this to map a webhook for sending the translation complete call without needing to set up a full api controller
            //app.MapGet("/minimalapi/values", () =>
            //    {
            //        string[] data = new string[] {
            //        "Hello World!",
            //        "Hello Galaxy!",
            //        "Hello Universe!"
            //    };

            //    return Results.Ok(data);
            //});

            app.Run();
        }
    }
}