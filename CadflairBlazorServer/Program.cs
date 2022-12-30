using CadflairBlazorServer.Controllers;
using CadflairDataAccess;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();

// Authetication and authorization
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAdB2C");
builder.Services.AddControllers();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddAuthorization(options =>
{
    // Need this for [Authorize] on controllers to use bearer token for authentication
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

// Signal R configuration
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

// MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// Cadflair data access 
builder.Services.AddSingleton<DataServicesManager>();

// Forge services
builder.Services.AddSingleton<ForgeServicesManager>();

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
app.UseRewriter(new RewriteOptions().Add(context =>
{
    if (context.HttpContext.Request.Path == "/MicrosoftIdentity/Account/SignedOut")
    {
        //this will redirect users to the home page when they sign out
        context.HttpContext.Response.Redirect("/");
    };
}));

// controllers, hubs, pages 
app.MapBlazorHub();
app.MapHub<ForgeCallbackHub>("/forgecallbackhub");
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();
