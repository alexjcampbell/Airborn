using System;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    // >dotnet ef migration add testMigration
    public class AirportDbContext : DbContext
    {
        public DbSet<Airport> Airports { get; set; }

        public DbSet<Runway> Runways_Flat { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=airborn.db;").LogTo(message => System.Diagnostics.Trace.WriteLine(message));
    }
}
