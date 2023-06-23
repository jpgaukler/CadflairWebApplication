using FluentEmail.Graph;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry();
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
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
});

// Signal R configuration
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

// Fluent Email
builder.Services.AddFluentEmail("donotreply@cadflair.com")
                .AddGraphSender(new GraphSenderOptions()
                {
                    ClientId = builder.Configuration["MailCredentials:ClientId"],
                    TenantId = builder.Configuration["MailCredentials:TenantId"],
                    Secret = builder.Configuration["MailCredentials:Secret"],
                })
                .AddRazorRenderer();

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

// Cadflair services
builder.Services.AddScoped<DataServicesManager>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ForgeServicesManager>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<FileHandlingService>();

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
