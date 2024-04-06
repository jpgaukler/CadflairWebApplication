using CadflairDataAccess;
using CadflairForgeAccess;
using FluentEmail.Graph;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

string policyName = "CadflairPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173",
                                             "http://localhost:4200",
                                             "https://www.cadflair.com",
                                             "https://cadflair.com",
                                             "https://polite-plant-02e2de80f.4.azurestaticapps.net")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

builder.Services.AddScoped(x => new DataServicesManager(builder.Configuration.GetConnectionString("CadflairStaging")));
builder.Services.AddScoped<ForgeServicesManager>();

// Fluent Email
builder.Services.AddFluentEmail("donotreply@cadflair.com")
                .AddGraphSender(new GraphSenderOptions()
                {
                    ClientId = builder.Configuration["MailCredentials:ClientId"],
                    TenantId = builder.Configuration["MailCredentials:TenantId"],
                    Secret = builder.Configuration["MailCredentials:Secret"],
                });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(policyName);
app.UseAuthorization();
app.MapControllers();
app.Run();
