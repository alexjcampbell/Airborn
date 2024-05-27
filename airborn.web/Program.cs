using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using Airborn.web.Models;
using Auth0.AspNetCore.Authentication;
using airborn.web.Support;
using System.Collections.Generic;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

var basePath = AppContext.BaseDirectory;

// Add configuration files
builder.Configuration
    .SetBasePath(basePath) 
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Cookie configuration for HTTP to support cookies with SameSite=None
// TODO: this wouldn't compile, later let's figure out why
// builder.Services.ConfigureSameSiteNoneCookies();

// Cookie configuration for HTTPS
//  builder.Services.Configure<CookiePolicyOptions>(options =>
//  {
//     options.MinimumSameSitePolicy = SameSiteMode.None;
//  });
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN") ?? builder.Configuration["Auth0:Domain"];
    options.ClientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID") ?? builder.Configuration["Auth0:ClientId"];

	
});


builder.Services.ConfigureSameSiteNoneCookies();

// Add the following line:
builder.WebHost.UseSentry(o =>
{
    o.Dsn = Environment.GetEnvironmentVariable("SENTRY_KEY");
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
    // We recommend adjusting this value in production.
    o.TracesSampleRate = 1.0;
    o.AttachStacktrace = true;
    o.SendDefaultPii = true;
});

builder.Services.AddDbContext<AirbornDbContext>(options =>
{

    var connectionString = DatabaseUtilities.GetConnectionString(builder.Configuration);
    options.UseNpgsql(connectionString);
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.ConsentCookieValue = "true";
});

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

var serviceName = "airborn.co";
var serviceVersion = "v1";

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(otelBuilder =>
    otelBuilder
        .AddHoneycomb(honeycombOptions)
        .AddCommonInstrumentations()
        .AddSqlClientInstrumentation()
        .AddAspNetCoreInstrumentationWithBaggage()
        .AddConsoleExporter()
        .AddOtlpExporter().
        ConfigureResource(resource =>
          resource.AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion))
);

// Configure logging
builder.Logging.AddOpenTelemetry(builder =>
{
    builder.IncludeFormattedMessage = true;
    builder.IncludeScopes = true;
    builder.ParseStateValues = true;
});

builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
{
    opt.IncludeScopes = true;
    opt.ParseStateValues = true;
    opt.IncludeFormattedMessage = true;
});

// Register Tracer so it can be injected into other components (eg Controllers)
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .ConfigureResource(resource =>
        resource.AddService(
          serviceName: serviceName,
          serviceVersion: serviceVersion))
    .AddOtlpExporter(option =>
        {
            option.Endpoint = new Uri("https://api.honeycomb.io");
            option.Headers = "x-honeycomb-team=" + Environment.GetEnvironmentVariable("HONEYCOMB__APIKEY");
        })
    .Build();

var appResourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName, serviceVersion);

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry((OpenTelemetry.Logs.OpenTelemetryLoggerOptions options) =>
    {
        options.SetResourceBuilder(appResourceBuilder);
        options.AddOtlpExporter((OpenTelemetry.Exporter.OtlpExporterOptions option) =>
        {
            option.Endpoint = new Uri("https://api.honeycomb.io");
            option.Headers = "x-honeycomb-team=" + Environment.GetEnvironmentVariable("HONEYCOMB__APIKEY");
        });
    });
});

var logger = loggerFactory.CreateLogger<Program>();

var app = builder.Build();

// Perform migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AirbornDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();

    
}

// insane workaround to get around the fact that Heroku only sees http
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

forwardedHeadersOptions.KnownNetworks.Clear(); // Clear the default networks
forwardedHeadersOptions.KnownProxies.Clear(); // Clear the default proxies

app.UseForwardedHeaders(forwardedHeadersOptions);


app.UseStaticFiles();
app.UseCookiePolicy();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Calculate}/{id?}");

app.MapControllerRoute(
    name: "Airports",
    pattern: "Airports/Details/{ident}",
    defaults: new { controller = "Airports", action = "Details" }
);

app.MapDefaultControllerRoute();

app.Run();



