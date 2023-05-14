using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Airborn.web.Models
{
    public static class DatabaseUtilities
    {

        public static IWebHost MigrateDatabase<T>(this IWebHost webHost) where T : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<T>();
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return webHost;
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            var env = Environment.GetEnvironmentVariable("DATABASE_URL");

            string connectionString;

            if (env != null)
            {
                connectionString = ConvertDatabaseUrlToHerokuString(env);
            }
            else
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            return connectionString;
        }

        public static string ConvertDatabaseUrlToHerokuString(string databaseUrl)
        {
            // Provided by Npgsql, this will convert the Heroku-style Database URL into a format acceptable by Npgsql
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            return new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Database = databaseUri.LocalPath.TrimStart('/'),
                Username = userInfo[0],
                Password = userInfo[1],
                SslMode = SslMode.Require, // Heroku requires SSL
                TrustServerCertificate = true, // Heroku self-signs its SSL certificates
            }.ToString();
        }
    }
}