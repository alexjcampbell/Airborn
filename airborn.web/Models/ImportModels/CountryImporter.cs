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
    public class CountryMap : ClassMap<Country>
    {
        public CountryMap()
        {
            // "id","code","name","continent","wikipedia_link","keywords"

            Map(m => m.ImportedCountry_Id).Name("id");
            Map(m => m.CountryCode).Name("code");
            Map(m => m.CountryName).Name("name");
            Map(m => m.ContinentCode).Name("continent");
            Map(m => m.WikipediaLink).Name("wikipedia_link");
            Map(m => m.Keywords).Name("keywords");
        }
    }

    public class CountryImporter
    {
        public async Task<(int createdCount, int updatedCount)> ImportCountries(AirbornDbContext _dbContext, List<Country> records)
        {
            int createdCount = 0;
            int updatedCount = 0;

            foreach (var record in records)
            {
                // Check if a country with the same code already exists
                var existingCountry = await _dbContext.Countries
                    .FirstOrDefaultAsync(c => c.ImportedCountry_Id == record.ImportedCountry_Id);

                if (existingCountry == null)
                {
                    // If not, add the new country
                    _dbContext.Countries.Add(record);
                    record.Continent = _dbContext.Continents.FirstOrDefault(c => c.Code == record.ContinentCode);
                    record.LastUpdated = DateTime.Now.ToUniversalTime();

                    createdCount++;
                }
                else
                {
                    existingCountry.CountryCode = record.CountryCode;
                    existingCountry.CountryName = record.CountryName;
                    existingCountry.WikipediaLink = record.WikipediaLink;
                    existingCountry.Keywords = record.Keywords;
                    existingCountry.Continent = _dbContext.Continents.FirstOrDefault(c => c.Code == record.ContinentCode);
                    existingCountry.LastUpdated = DateTime.Now.ToUniversalTime();

                    updatedCount++;
                }
            }

            await _dbContext.SaveChangesAsync();

            return (createdCount, updatedCount);
        }
    }
}
