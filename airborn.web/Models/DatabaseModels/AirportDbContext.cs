using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    // >dotnet ef migration add testMigration
    public class AirportDbContext : DbContext, IAirportDbContext
    {
        public AirportDbContext()
        {

        }

        public AirportDbContext(DbContextOptions<AirportDbContext> options)
        : base(options)
        {

        }

        public virtual DbSet<Airport> Airports { get; set; }

        public virtual DbSet<Runway> Runways { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Runway>()
                .HasKey(r => r.Runway_Id)
                .HasName("Runway_Id");

            modelBuilder.Entity<Airport>()
                .HasKey(a => a.Id)
                .HasName("Id");                

            modelBuilder.Entity<Airport>()
                .HasMany(a => a.Runways)
                .WithOne(r => r.Airport)
                .HasForeignKey(r => r.AirportRef);
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
                        (!r.Runway_Name.StartsWith("H") // remove helipadss
                        )
                ).ToList<Runway>();
        }

        public Airport GetAirport(string airportIdentifier)
        {
            if (Airports.Count() == 0)
            {
                throw new ArgumentOutOfRangeException("Airports database is empty.");
            }

            if (Airports.Count<Airport>(
                a => a.Ident.Equals(airportIdentifier.ToUpper())
                ) == 0)
            {
                throw new AirportNotFoundException("Airport not found: " + airportIdentifier);
            };

            Airport airport = Airports.Single<Airport>(
                a => a.Ident.Equals(airportIdentifier.ToUpper())
                );

            return airport;

        }

        public List<Airport> GetAirports()
        {
            return Airports.ToList<Airport>();
        }

        public List<KeyValuePair<string, string>> SearchForAirportsByIdentifier(string term)
        {

            DateTime start = DateTime.Now;

            var airportIdentifiers = (from airport in Airports
                                      where EF.Functions.Like(airport.Ident, "%" + term + "%")
                                      select new
                                      {
                                          label = airport.Ident,
                                          val = airport.Id
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
