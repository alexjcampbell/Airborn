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
    public class ContinentMap : ClassMap<Continent>
    {
        public ContinentMap()
        {
            Map(m => m.Code).Name("ContinentCode");
            Map(m => m.ContinentName).Name("ContinentName");
        }
    }

    public class ContinentImporter
    {
        public async Task<(int createdCount, int updatedCount)> ImporContinents(AirbornDbContext _dbContext, List<Continent> records)
        {
            int createdCount = 0;
            int updatedCount = 0;

            foreach (var record in records)
            {
                // Check if a continent with the same code already exists
                var existingContinent = await _dbContext.Continents
                    .FirstOrDefaultAsync(c => c.Code == record.Code);

                if (existingContinent == null)
                {
                    // If not, add the new continent
                    _dbContext.Continents.Add(record);
                    record.LastUpdated = DateTime.Now.ToUniversalTime();
                    createdCount++;
                }
                else
                {
                    // If it exists, update the existing continent
                    existingContinent.ContinentName = record.ContinentName;
                    existingContinent.LastUpdated = DateTime.Now.ToUniversalTime();

                    updatedCount++;
                }
            }

            await _dbContext.SaveChangesAsync();

            return (createdCount, updatedCount);
        }
    }
}
