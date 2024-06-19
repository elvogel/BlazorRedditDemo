using BlazorRedditDemo.Components;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

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

app.Run();

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
        .MinimumLevel.Override("Microsoft.AspNetCore.Http.Connections",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Components",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authorization",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.SignalR",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.ModelBinding",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Extensions.Http.DefaultHttpClientFactory",LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.FeatureManagement.FeatureManager",LogEventLevel.Warning)
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .CreateLogger();
    Log.Logger = logger;
    builder.Host.UseSerilog();
    builder.Services.AddSingleton(logger);

}

void configureServices()
{
    builder.Services.AddAutoMapper(typeof(BRD.Common.Mapping.MappingProfile));
    builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(GetProRequest).Assembly);
    });
}
