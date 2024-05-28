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

namespace Airborn.web.Models.ImportModels
{
    public class RunwayImportModel
    {
        // Properties of Runway
        public int ImportedRunway_Id { get; set; }
        public string Ident { get; set; }
        public double? LatitudeDeg { get; set; }
        public double? LongitudeDeg { get; set; }
        public int? ElevationFt { get; set; }
        public double? HeadingDegT { get; set; }
        public int? DisplacedThresholdFt { get; set; }
        public string RegionCode { get; set; }
        public string CountryCode { get; set; }
        public int ImportedAirport_ID { get; set; }
        public string Airport_Ident { get; set; }
        public int? RunwayLength { get; set; }
        public int? RunwayWidth { get; set; }
        public string Surface { get; set; }
        public bool? Lighted { get; set; }
        public bool? Closed { get; set; }        
    }

    public class RunwayImportPair
    {

        public RunwayImportModel Runway1 { get; set; }
        public RunwayImportModel Runway2 { get; set; }
    }

    public class RunwayMap : ClassMap<RunwayImportPair>
    {
        public RunwayMap()
        {
            // "id","airport_ref","airport_ident","length_ft","width_ft","surface","lighted","closed","le_ident","le_latitude_deg","le_longitude_deg","le_elevation_ft","le_heading_degT","le_displaced_threshold_ft","he_ident","he_latitude_deg","he_longitude_deg","he_elevation_ft","he_heading_degT","he_displaced_threshold_ft"
            Map(m => m.Runway1.ImportedRunway_Id).Name("id");
            Map(m => m.Runway1.ImportedAirport_ID).Name("airport_ref");
            Map(m => m.Runway1.Airport_Ident).Name("airport_ident");
            Map(m => m.Runway1.RunwayLength).Name("length_ft");
            Map(m => m.Runway1.RunwayWidth).Name("width_ft");
            Map(m => m.Runway1.Surface).Name("surface");
            Map(m => m.Runway1.Lighted).Name("lighted");
            Map(m => m.Runway1.Closed).Name("closed");
            Map(m => m.Runway1.Ident).Name("le_ident");
            Map(m => m.Runway1.LatitudeDeg).Name("le_latitude_deg");
            Map(m => m.Runway1.LongitudeDeg).Name("le_longitude_deg");
            Map(m => m.Runway1.ElevationFt).Name("le_elevation_ft");
            Map(m => m.Runway1.HeadingDegT).Name("le_heading_degT");
            Map(m => m.Runway1.DisplacedThresholdFt).Name("le_displaced_threshold_ft");

            Map(m => m.Runway2.ImportedRunway_Id).Name("id");
            Map(m => m.Runway2.ImportedAirport_ID).Name("airport_ref");
            Map(m => m.Runway2.Airport_Ident).Name("airport_ident");
            Map(m => m.Runway2.RunwayLength).Name("length_ft");
            Map(m => m.Runway2.RunwayWidth).Name("width_ft");
            Map(m => m.Runway2.Surface).Name("surface");
            Map(m => m.Runway2.Lighted).Name("lighted");
            Map(m => m.Runway2.Closed).Name("closed");            
            Map(m => m.Runway2.Ident).Name("he_ident");
            Map(m => m.Runway2.LatitudeDeg).Name("he_latitude_deg");
            Map(m => m.Runway2.LongitudeDeg).Name("he_longitude_deg");
            Map(m => m.Runway2.ElevationFt).Name("he_elevation_ft");
            Map(m => m.Runway2.HeadingDegT).Name("he_heading_degT");
            Map(m => m.Runway2.DisplacedThresholdFt).Name("he_displaced_threshold_ft");
        }
    }

    public class RunwayImporter
    {
        public RunwayImporter(ILogger logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;

        public async Task<(int createdCount, int updatedCount)> ImportRunways(AirbornDbContext _dbContext, List<RunwayImportPair> records)
        {

            var allCountries = await _dbContext.Countries.ToDictionaryAsync(c => c.CountryCode);
            var allRegions = await _dbContext.Regions.ToDictionaryAsync(r => r.RegionCode);
            var allAirports = await _dbContext.Airports.ToDictionaryAsync(r => r.ImportedAirport_Id);
            //var allRunways = await _dbContext.Runways.ToDictionaryAsync(r => r.ImportedRunway_Id + " " + r.Runway_Name);
            var allRunwaysList = await _dbContext.Runways.ToListAsync();

            var createdCount = 0;
            var updatedCount = 0;

            var batchSize = 1000;
            var batch = new List<RunwayImportPair>(batchSize);

            _logger.LogInformation("Beginning to import {count} runways", records.Count);

            for (int i = 0; i < records.Count; i += batchSize)
            {
                var currentBatch = records.Skip(i).Take(batchSize).ToList();

                foreach (var pair in currentBatch)
                {
                    ProcessRunway(pair.Runway1);
                    ProcessRunway(pair.Runway2);
                }

                await _dbContext.SaveChangesAsync();
            }

            void ProcessRunway(RunwayImportModel runway)
            {
                var existingRunway = allRunwaysList.FirstOrDefault(r =>
                    r.ImportedRunway_Id == runway.ImportedRunway_Id &&
                    r.Runway_Name == runway.Ident);


                if (existingRunway == null)
                {

                    Runway newRunway = new Runway();

                    Airport airport = allAirports[runway.ImportedAirport_ID];
                    
                    newRunway.Airport_Id = airport.Airport_Id;
                    newRunway.ImportedAirport_ID = runway.ImportedAirport_ID;
                    newRunway.RunwayLength = runway.RunwayLength;
                    newRunway.RunwayWidth = runway.RunwayWidth;
                    newRunway.SurfaceText = runway.Surface;
                    newRunway.Lighted = runway.Lighted.ToString();
                    newRunway.Closed = runway.Closed.ToString();
                    newRunway.ImportedRunway_Id = runway.ImportedRunway_Id;
                    newRunway.Airport_Ident = runway.Airport_Ident;
                    
                    newRunway.Runway_Name = runway.Ident;
                    newRunway.Latitude_Deg = runway.LatitudeDeg;
                    newRunway.Longitude_Deg = runway.LongitudeDeg;
                    newRunway.ElevationFt = runway.ElevationFt;
                    newRunway.HeadingDegreesTrue = runway.HeadingDegT;
                    newRunway.DisplacedThresholdFt = runway.DisplacedThresholdFt;
                    newRunway.LastUpdated = DateTime.UtcNow;

                    _dbContext.Runways.Add(newRunway);
                    createdCount++;
                }
                else
                {

                    existingRunway.RunwayLength = runway.RunwayLength;
                    existingRunway.RunwayWidth = runway.RunwayWidth;
                    existingRunway.SurfaceText = runway.Surface;
                    existingRunway.Lighted = runway.Lighted.ToString();
                    existingRunway.Closed = runway.Closed.ToString();

                    existingRunway.Latitude_Deg = runway.LatitudeDeg;
                    existingRunway.Longitude_Deg = runway.LongitudeDeg;
                    existingRunway.ElevationFt = runway.ElevationFt;
                    existingRunway.HeadingDegreesTrue = runway.HeadingDegT;
                    existingRunway.ElevationFt = runway.DisplacedThresholdFt;
                    existingRunway.Latitude_Deg = runway.LatitudeDeg;
                    existingRunway.Longitude_Deg = runway.LongitudeDeg;
                    existingRunway.HeadingDegreesTrue = runway.HeadingDegT;
                    existingRunway.DisplacedThresholdFt = runway.DisplacedThresholdFt;
                    existingRunway.LastUpdated = DateTime.UtcNow;

                    updatedCount++;
                }
            }

            _logger.LogInformation("Finished importing runways: {createdCount} created, {updatedCount} updated", createdCount, updatedCount);

            return (createdCount, updatedCount);


        }
    }
}