using Microsoft.Extensions.Options;
using SignalRTrial.Configurations;
using SignalRTry.Configurations;
using SignalRTry.Hubs;
using SignalRTry.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

var configuration = builder.Configuration;

builder.Services.Configure<ExternalSiteSettings>(builder.Configuration.GetSection("ExternalSiteSettings"));
builder.Services.Configure<ConnectionCleanupSettings>(builder.Configuration.GetSection("ConnectionCleanupSettings"));

builder.Services.AddSingleton<UserConnectionService>();
builder.Services.AddHostedService<ConnectionCleanupService>();

var corsPermissionDomains = builder.Configuration.GetValue<string[]>("ExternalSiteSettings:CorsPermissionDomains");

var externalSiteSettings = builder.Configuration.GetSection("ExternalSiteSettings").Get<ExternalSiteSettings>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("mycors",
                      policy =>
                      {
                          policy.WithOrigins(externalSiteSettings?.ArrayCorsPermissionDmains ?? []).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });
});

var app = builder.Build();
var environment = builder.Environment;

app.UseCors("mycors");

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/hub");

app.Run();
