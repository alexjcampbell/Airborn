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
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

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
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(appResourceBuilder);
        options.AddOtlpExporter(option =>
        {
            option.Endpoint = new Uri("https://api.honeycomb.io");
            option.Headers = "x-honeycomb-team=" + Environment.GetEnvironmentVariable("HONEYCOMB__APIKEY");
        });
    });
});

var logger = loggerFactory.CreateLogger<Program>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

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
