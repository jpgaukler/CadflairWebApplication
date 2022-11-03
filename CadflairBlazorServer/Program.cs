using CadflairBlazorServer.Data;
using CadflairDataAccess;
using CadflairDataAccess.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddTransient<AccountService>();

var app = builder.Build();

//Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzUxNzU1QDMyMzAyZTMzMmUzME9JZTdsYkhPcWhJbnJiSXpoVE8rZWhNYjlLQUpFQjVKSlY1TE5seWtYSDQ9");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/_Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//confiugure use methods
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//configure map methods
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
