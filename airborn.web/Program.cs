using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Auth0.AspNetCore.Authentication;
using Airborn.web.Models;
using Airborn.web.Models.ImportModels;
using airborn.web.Support;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppConfiguration();
builder.ConfigureGlobalization();
builder.ConfigureServices();
builder.ConfigureLogging();

var app = builder.Build();

app.ConfigureExceptionHandling();
app.ConfigureForwardedHeaders();
app.ConfigureMiddleware();
app.ConfigureEndpoints();

app.Run();

// Extension methods for configuring the app

static class StartupExtensions
{
    public static void ConfigureAppConfiguration(this WebApplicationBuilder builder)
    {
        var basePath = AppContext.BaseDirectory;

        builder.Configuration
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    public static void ConfigureGlobalization(this WebApplicationBuilder builder)
    {
        var cultureInfo = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews();

        builder.Services.ConfigureSameSiteNoneCookies();

        builder.Services.AddAuth0WebAppAuthentication(options =>
        {
            options.Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN") ?? builder.Configuration["Auth0:Domain"];
            options.ClientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID") ?? builder.Configuration["Auth0:ClientId"];
        });

        builder.Services.AddDbContext<AirbornDbContext>(options =>
        {
            var connectionString = DatabaseUtilities.GetConnectionString(builder.Configuration);
            options.UseNpgsql(connectionString); // Use PostgreSQL
        });

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.ConsentCookieValue = "true";
        });

        var honeycombOptions = builder.Configuration.GetHoneycombOptions();
        var serviceName = "airborn.co";
        var serviceVersion = "v1";

        builder.Services.AddOpenTelemetry().WithTracing(otelBuilder =>
            otelBuilder
                .AddHoneycomb(honeycombOptions)
                .AddCommonInstrumentations()
                .AddSqlClientInstrumentation()
                .AddAspNetCoreInstrumentationWithBaggage()
                .AddConsoleExporter()
                .AddOtlpExporter()
                .ConfigureResource(resource =>
                    resource.AddService(serviceName, serviceVersion))
        );

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });        
    }

    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        var honeycombOptions = builder.Configuration.GetHoneycombOptions();
        var serviceName = "airborn.co";
        var serviceVersion = "v1";

        builder.Logging.AddOpenTelemetry(loggingBuilder =>
        {
            loggingBuilder.IncludeFormattedMessage = true;
            loggingBuilder.IncludeScopes = true;
            loggingBuilder.ParseStateValues = true;
        });

        builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
        {
            opt.IncludeScopes = true;
            opt.ParseStateValues = true;
            opt.IncludeFormattedMessage = true;
        });

        builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(serviceName)
            .ConfigureResource(resource =>
                resource.AddService(serviceName, serviceVersion))
            .AddOtlpExporter(option =>
            {
                option.Endpoint = new Uri("https://api.honeycomb.io");
                option.Headers = "x-honeycomb-team=" + Environment.GetEnvironmentVariable("HONEYCOMB__APIKEY");
            })
            .Build();

        var appResourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion);

        using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(appResourceBuilder);
                options.AddOtlpExporter(option =>
                {
                    option.Endpoint = new Uri("https://api.honeycomb.io");
                    option.Headers = "x-honeycomb-team=" + Environment.GetEnvironmentVariable("HONEYCOMB__APIKEY");
                });
            });
        });
    }

    public static void ConfigureExceptionHandling(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
            app.UseHttpsRedirection();
        }
    }

    public static void ConfigureForwardedHeaders(this WebApplication app)
    {
        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };

        forwardedHeadersOptions.KnownNetworks.Clear(); // Clear the default networks
        forwardedHeadersOptions.KnownProxies.Clear(); // Clear the default proxies

        app.UseForwardedHeaders(forwardedHeadersOptions);
    }

    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseAuthorization();
    }

    public static void ConfigureEndpoints(this WebApplication app)
    {

        app.MapControllerRoute(
            name: "Airports",
            pattern: "Airports/Airport/{ident}",
            defaults: new { controller = "Airports", action = "Airport" });

        app.MapControllerRoute(
            name: "Airports",
            pattern: "Airports/Continent/{slug}",
            defaults: new { controller = "Airports", action = "Continent" });

        app.MapControllerRoute(
            name: "Airports",
            pattern: "Airports/Country/{slug}",
            defaults: new { controller = "Airports", action = "Country" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Calculate}/{id?}");

        app.MapDefaultControllerRoute();
    }
}