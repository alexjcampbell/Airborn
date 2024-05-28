using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Airborn.web.Models;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace Airborn.web.Models.ImportModels
{

    public class AirportMap : ClassMap<Airport>
    {
        public AirportMap()
        {
            // "id","ident","type","name","latitude_deg","longitude_deg","elevation_ft","continent","iso_country","iso_region","municipality","scheduled_service","gps_code","iata_code","local_code","home_link","wikipedia_link","keywords"
            Map(m => m.ImportedAirport_Id).Name("id");
            Map(m => m.Ident).Name("ident");
            Map(m => m.Type).Name("type");
            Map(m => m.Name).Name("name");
            Map(m => m.Latitude_Deg).Name("latitude_deg");
            Map(m => m.Longitude_Deg).Name("longitude_deg");
            Map(m => m.FieldElevation).Name("elevation_ft");
            Map(m => m.CountryCode).Name("iso_country");
            Map(m => m.RegionCode).Name("iso_region");
            Map(m => m.Location).Name("municipality");
            Map(m => m.ScheduledService).Name("scheduled_service");
            Map(m => m.GPSCode).Name("gps_code");
            Map(m => m.IATACode).Name("iata_code");
            Map(m => m.LocalCode).Name("local_code");
            Map(m => m.HomeLink).Name("home_link");
            Map(m => m.WikipediaLink).Name("wikipedia_link");
            Map(m => m.Keywords).Name("keywords");
        }
    }

    public interface IAirportImportJob
    {
        void Execute(List<Airport> records);
    }

    public class AirportImportJob : IAirportImportJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AirportImportJob> _logger;

        public AirportImportJob(IServiceScopeFactory serviceScopeFactory, ILogger<AirportImportJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public void Execute(List<Airport> records)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AirbornDbContext>();

                // Log starting the job
                _logger.LogInformation("Starting AirportImportJob execution.");

                try
                {

                    var allCountries = dbContext.Countries.ToDictionary(c => c.CountryCode);
                    var allRegions = dbContext.Regions.ToDictionary(r => r.RegionCode);

                    var batchSize = 1000;
                    var batch = new List<Airport>(batchSize);

                    _logger.LogInformation("Beginning to import {count} airports", records.Count);

                    var createdCount = 0;
                    var updatedCount = 0;

                    for (int i = 0; i < records.Count; i += batchSize)
                    {
                        var currentBatch = records.Skip(i).Take(batchSize).ToList();

                        var existingAirports = dbContext.Airports
                            .Where(a => currentBatch.Select(c => c.ImportedAirport_Id).Contains(a.ImportedAirport_Id))
                            .ToDictionary(a => a.ImportedAirport_Id);

                        foreach (var record in currentBatch)
                        {
                            if (!existingAirports.TryGetValue(record.ImportedAirport_Id, out var existingAirport))
                            {
                                // If not, add the new airport
                                record.Region_Id = allRegions[record.RegionCode].Region_Id;
                                record.Country_Id = allCountries[record.CountryCode].Country_Id;
                                record.LastUpdated = DateTime.UtcNow;

                                dbContext.Airports.Add(record);
                                createdCount++;
                            }
                            else
                            {
                                existingAirport.Ident = record.Ident;
                                existingAirport.Type = record.Type;
                                existingAirport.Name = record.Name;
                                existingAirport.Latitude_Deg = record.Latitude_Deg;
                                existingAirport.Longitude_Deg = record.Longitude_Deg;
                                existingAirport.FieldElevation = record.FieldElevation;
                                existingAirport.CountryCode = record.CountryCode;
                                existingAirport.RegionCode = record.RegionCode;
                                existingAirport.Location = record.Location;
                                existingAirport.ScheduledService = record.ScheduledService;
                                existingAirport.GPSCode = record.GPSCode;
                                existingAirport.IATACode = record.IATACode;
                                existingAirport.LocalCode = record.LocalCode;
                                existingAirport.HomeLink = record.HomeLink;
                                existingAirport.WikipediaLink = record.WikipediaLink;
                                existingAirport.Keywords = record.Keywords;
                                existingAirport.Region = allRegions[record.RegionCode];
                                existingAirport.Country = allCountries[record.CountryCode];
                                existingAirport.LastUpdated = DateTime.UtcNow;
                                updatedCount++;
                            }
                        }

                        dbContext.SaveChanges();
                    }

                    _logger.LogInformation("Finished importing runways: {createdCount} created, {updatedCount} updated", createdCount, updatedCount);


                    // Log successful execution
                    _logger.LogInformation("AirportImportJob executed successfully.");
                }
                catch (Exception ex)
                {
                    // Log any exceptions
                    _logger.LogError(ex, "An error occurred while executing AirportImportJob.");
                }
            }
        }
    }

}