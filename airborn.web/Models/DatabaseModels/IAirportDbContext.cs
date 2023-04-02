using System;
using Microsoft.EntityFrameworkCore;


namespace Airborn.web.Models
{
    public interface IAirportDbContext
    {
        public DbSet<Airport> Airports { get; set; }

        public DbSet<Runway> Runways { get; set; }

    }
}