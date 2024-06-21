using BlazorRedditDemo.Components;
using BlazorRedditDemo.Hubs;
using BRD.Services.Handlers;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Syncfusion.Blazor;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

//configureConfig();
configureLogging();
configureServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapHub<RedditHub>("/reddithub");
app.Run();

void configureConfig()
{

    var connectionString =  builder.Configuration.GetConnectionString("AppConfig");
    builder.Configuration.AddAzureAppConfiguration(config =>
    {
        //https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-labels-aspnet-core#load-configuration-values-with-a-specified-label

        config.UseFeatureFlags();

        /*
        if (!builder.Environment.IsProduction())
        {
            config.UseFeatureFlags(s => s.Label = "dev");
        }
        else if (builder.Environment.IsStaging())
        {
            config.UseFeatureFlags(s => s.Label = "stag");
        }
        else
        {
            config.UseFeatureFlags(s => s.Label = "prd");
        }
        */
        config.Connect(connectionString);
    });

}

void configureLogging()
{
    ILogger logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Azure.Core",LogEventLevel.Error)

        .MinimumLevel.Override("Microsoft.AspNetCore.ResponseCompression",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Antiforgery",LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.AspNetCore.StaticFiles",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.Matching",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware",LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.Authentication.Cookies",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Http",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Components",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authorization",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.SignalR",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.ModelBinding",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Extensions.Http.DefaultHttpClientFactory",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.FeatureManagement.FeatureManager",LogEventLevel.Warning)
        .MinimumLevel.Override("System.Net.Http",LogEventLevel.Warning)
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .CreateLogger();
    Log.Logger = logger;
    builder.Host.UseSerilog();
    builder.Services.AddSingleton(logger);

}

void configureServices()
{
    /*
    builder.Services.AddAutoMapper(typeof(BRD.Common.Mapping.MappingProfile));
    builder.Services.AddMediatR(config =>
    {
        //config.RegisterServicesFromAssembly(typeof(GetProRequest).Assembly);
    });
    */

    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Environment.GetEnvironmentVariable("SyncfusionKey"));
    builder.Services.AddSyncfusionBlazor();
    builder.Services.AddMemoryCache();
    builder.Services.AddSignalR();

    builder.Services.AddScoped<RedditHandler>();
    builder.Services.AddHttpClient("Reddit").AddHttpMessageHandler<RedditHandler>();
    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            ["application/octet-stream"]);
    });
}
