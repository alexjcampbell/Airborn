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
    public class RegionMap : ClassMap<Region>
    {
        public RegionMap()
        {
            // "id","code","local_code","name","continent","iso_country","wikipedia_link","keywords"

            Map(m => m.ImportedRegion_ID).Name("id");
            Map(m => m.RegionCode).Name("code");
            Map(m => m.LocalCode).Name("local_code");
            Map(m => m.Name).Name("name");
            Map(m => m.CountryCode).Name("iso_country");
            Map(m => m.WikipediaLink).Name("wikipedia_link");

        }
    }

    public class RegionImporter
    {
        public async Task<(int createdCount, int updatedCount)> ImportRegions(AirbornDbContext _dbContext, List<Region> records)
        {
            // Load all countries into memory
            var allCountries = await _dbContext.Countries.ToDictionaryAsync(c => c.CountryCode);

            int createdCount = 0;
            int updatedCount = 0;

            var newRegions = new List<Region>();
            var existingRegions = new List<Region>();

            foreach (var record in records)
            {
                var existingRegion = await _dbContext.Regions
                    .FirstOrDefaultAsync(r => r.ImportedRegion_ID == record.ImportedRegion_ID);

                if (existingRegion == null)
                {
                    record.Country_Id = allCountries[record.CountryCode].Country_Id;
                    record.LastUpdated = DateTime.UtcNow;
                    newRegions.Add(record);

                    createdCount++;
                }
                else
                {
                    existingRegion.ImportedRegion_ID = record.ImportedRegion_ID;
                    existingRegion.RegionCode = record.RegionCode;
                    existingRegion.LocalCode = record.LocalCode;
                    existingRegion.Name = record.Name;
                    existingRegion.CountryCode = record.CountryCode;
                    existingRegion.WikipediaLink = record.WikipediaLink;
                    existingRegion.LastUpdated = DateTime.Now.ToUniversalTime();
                    existingRegion.Country = allCountries[record.CountryCode];

                    existingRegions.Add(existingRegion);
                    updatedCount++;
                }
            }

            await _dbContext.Regions.AddRangeAsync(newRegions);
            _dbContext.Regions.UpdateRange(existingRegions);
            await _dbContext.SaveChangesAsync();

            return (createdCount, updatedCount);


        }
    }
}
