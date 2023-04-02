using System;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    // >dotnet ef migration add testMigration
    public class AirportDbContext : DbContext, IAirportDbContext
    {
        public AirportDbContext(){
            
        }

        public AirportDbContext(DbContextOptions<AirportDbContext> options)
        : base(options) {

        }

        public virtual DbSet<Airport> Airports { get; set; }

        public virtual DbSet<Runway> Runways { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Runway>()
                .HasKey(r => r.Runway_Id)
                .HasName("Runway_Id");
        }            
    }
}
