using CadflairBlazorServer.Data;
using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairDataAccess.Services;
using CadflairDataAccess.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);


// Add identity service
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddDefaultTokenProviders();

// Add data access services
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
builder.Services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
//builder.Services.AddTransient<AccountService>();
//builder.Services.AddTransient<UserService>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

//Identity configuration  
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Password settings  
//    options.Password.RequiredLength = 8;
//    options.Password.RequireDigit = true;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireLowercase = true;

//    // Lockout settings  
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
//    options.Lockout.MaxFailedAccessAttempts = 10;
//    options.Lockout.AllowedForNewUsers = true;

//    // User settings  
//    options.User.RequireUniqueEmail = true;
//});

// Cookie configuration  
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.Cookie.Expiration = TimeSpan.FromDays(30);
//    options.LoginPath = "/Account/Login";
//    options.LogoutPath = "/Account/Logout";
//    options.AccessDeniedPath = "/Account/AccessDenied";
//    options.SlidingExpiration = true;
//});

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
