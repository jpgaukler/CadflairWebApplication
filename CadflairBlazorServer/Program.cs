using CadflairBlazorServer.Authentication;
using CadflairDataAccess;
using CadflairDataAccess.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// Authentication services
builder.Services.AddScoped<AuthenticationStateProvider, CadflairAuthenticationStateProvider>();
builder.Services.AddScoped<ProtectedSessionStorage>();

// Data Access services
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/_Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//configure use methods
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//configure map methods
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
