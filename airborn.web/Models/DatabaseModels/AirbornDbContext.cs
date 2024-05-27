using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Airborn.web.Models
{

    public class AirbornDbContext : DbContext, IAirportDbContext
    {

        public AirbornDbContext()
        {
        }

        public AirbornDbContext(DbContextOptions<AirbornDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airport> Airports { get; set; }

        public virtual DbSet<Runway> Runways { get; set; }

        public virtual DbSet<Region> Regions { get; set; }

        public virtual DbSet<Continent> Continents { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Must call this first

            // primary keys
            builder.Entity<Runway>()
                .HasKey(r => r.Runway_Id)
                .HasName("PK_Runway_Id");

            builder.Entity<Airport>()
                .HasKey(a => a.Airport_Id)
                .HasName("PK_Airport_Id");

             builder.Entity<Country>()
                .HasKey(c => c.Country_Id)
                .HasName("PK_Country_Id");

             builder.Entity<Continent>()
                .HasKey(c => c.Continent_Id)
                .HasName("PK_Continent_Id");
                
             builder.Entity<Region>()
                .HasKey(r => r.Region_Id)
                .HasName("PK_Region_Id");                                

            // foreign keys
            builder.Entity<Airport>()
                .HasMany(a => a.Runways)
                .WithOne(r => r.Airport)
                .HasForeignKey(r => r.Airport_Id)
                .HasConstraintName("FK_Runway_Airport_Id");

            builder.Entity<Continent>()
                .HasMany(cc => cc.Countries)
                .WithOne(co => co.Continent)
                .HasForeignKey(cc =>cc.Continent_Id)
                .HasConstraintName("FK_Country_Continent_Id");

            builder.Entity<Country>()
                .HasMany(c => c.Airports)
                .WithOne(a => a.Country)
                .HasForeignKey(a => a.Country_Id)
                .HasConstraintName("FK_Airport_Country_Id");

            builder.Entity<Country>()
                .HasMany(c => c.Regions)
                .WithOne(r => r.Country)
                .HasForeignKey(a => a.Country_Id)
                .HasConstraintName("FK_Region_Country_Id");

            builder.Entity<Region>()
                .HasMany(a => a.Airports)
                .WithOne(r => r.Region)
                .HasForeignKey(a => a.Region_Id)
                .HasConstraintName("FK_Airport_Region_Id");                

            // go fast indices
            builder.Entity<Airport>()
                .HasIndex(a => a.Ident)
                .IsUnique()
                .HasDatabaseName("IX_Airport_Ident");

            builder.Entity<Airport>()
                .HasIndex(a => a.Type)
                .HasDatabaseName("IX_Airport_Type");

            builder.Entity<Runway>()
                .HasIndex(r => r.Airport_Id)
                .HasDatabaseName("IX_Runway_Airport_Id");

            builder.Entity<Runway>()
                .HasIndex(r => r.Runway_Name)
                .HasDatabaseName("IX_Runway_Runway_Name");

                
        }

        public List<Runway> GetRunwaysForAirport(string airportIdentifer)
        {

            // We defensively check for r.Airport_Ident being null before we check StartsWith,
            // because otherwise EF will throw a NullReferenceException if no Airport_Ident
            // is set on a row
            return Runways.Where<Runway>
                (
                        r => r.Airport_Ident != null
                        &&
                        r.Airport_Ident.StartsWith(airportIdentifer.ToUpper())
                        &&
                        (!r.Runway_Name.StartsWith("H") // remove helipads
                        )
                ).ToList<Runway>();
        }

        public Airport GetAirport(string airportIdentifier)
        {
            if (Airports.Count() == 0)
            {
                throw new ArgumentOutOfRangeException("Airports database is empty.");
            }
            if (airportIdentifier == null)
            {
                return null;
            }

            if (Airports.Count<Airport>(
                a => a.Ident.Equals(airportIdentifier.ToUpper())
                ) == 0)
            {
                return null;
            };

            Airport airport = Airports.Single<Airport>(
                a => a.Ident.Equals(airportIdentifier.ToUpper())
                );

            airport.Runways = GetRunwaysForAirport(airportIdentifier);

            return airport;

        }

        public List<Airport> GetAirports()
        {
            return Airports.ToList<Airport>();
        }

        public List<Airport> GetAirportsPaginated(int pageNumber, int pageSize)
        {
            return Airports.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList<Airport>();
        }

        public List<KeyValuePair<string, string>> SearchForAirportsByIdentifier(string term)
        {

            DateTime start = DateTime.Now;

            var airportIdentifiers = (from airport in Airports
                                      where EF.Functions.Like(airport.Ident, "%" + term + "%")
                                      select new
                                      {
                                          label = airport.Ident,
                                          val = airport.Airport_Id
                                      }).AsNoTracking().Take(20);

            DateTime end = DateTime.Now;

            List<KeyValuePair<string, string>> airportIdentifiersList = new List<KeyValuePair<string, string>>();

            foreach (var airport in airportIdentifiers)
            {
                airportIdentifiersList.Add(new KeyValuePair<string, string>(airport.val.ToString(), airport.label));
            }

            // Remnants of past performance debugging:
            // Trace.WriteLine($"Query time taken: {(end - start).TotalSeconds}");

            return airportIdentifiersList;

        }

    }
}
