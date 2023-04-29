using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using Honeycomb.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter.Jaeger;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Airborn.web.Models;
using Microsoft.AspNetCore.Routing;
using OpenTelemetry.Instrumentation.AspNetCore;

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

builder.Services.AddDbContext<AirportDbContext>(options => options.UseSqlite(@"Data Source=airborn.db;").LogTo(message => System.Diagnostics.Trace.WriteLine(message)));

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(otelBuilder =>
    otelBuilder
        .AddHoneycomb(honeycombOptions)
        .AddCommonInstrumentations()
        .AddSqlClientInstrumentation()
        .AddAspNetCoreInstrumentationWithBaggage()
        .AddConsoleExporter()
        .AddOtlpExporter()
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

var appResourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("airborn.co", "v1");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(appResourceBuilder);
        options.AddOtlpExporter(option =>
        {
            option.Endpoint = new Uri("https://api.honeycomb.io");
            option.Headers = "x-honeycomb-team=YOUR_API_KEY";
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Calculate}/{id?}");

app.MapDefaultControllerRoute();

app.Run();
