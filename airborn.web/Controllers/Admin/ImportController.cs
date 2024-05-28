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
using Airborn.web.Models.ImportModels;
using Hangfire;

namespace Airborn.web.Controllers
{
    [Authorize(Roles = "AirbornAdmin")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;

        private readonly IBackgroundJobClient _backgroundJobClient;

        private readonly AirbornDbContext _dbContext;
        

        private IWebHostEnvironment _env;

       public ImportController(
        IBackgroundJobClient backgroundJobClient,
        ILogger<ImportController> logger,
        IWebHostEnvironment env,
        AirbornDbContext dbContext)
    {
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
        _env = env;
        _dbContext = dbContext;
    }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadContinents()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> UploadContinents(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            if (Path.GetExtension(file.FileName) != ".csv")
            {
                return BadRequest("File is not a CSV file");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<ContinentMap>();
            var records = csv.GetRecords<Continent>();

            ContinentImporter continentImporter = new ContinentImporter();
            var (createdCount, updatedCount) = await continentImporter.ImporContinents(_dbContext, records.ToList());

            return Ok($"Continents imported successfully. {createdCount} created, {updatedCount} updated.");
        }



        public IActionResult UploadCountries()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> UploadCountries(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            if (Path.GetExtension(file.FileName) != ".csv")
            {
                return BadRequest("File is not a CSV file");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<CountryMap>();
            var records = csv.GetRecords<Country>();

            CountryImporter countryImporter = new CountryImporter();

            var (createdCount, updatedCount) = await countryImporter.ImportCountries(_dbContext, records.ToList());

            return Ok($"Countries imported successfully. {createdCount} created, {updatedCount} updated.");
        }


        public IActionResult UploadRegions()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadRegions(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            if (Path.GetExtension(file.FileName) != ".csv")
            {
                return BadRequest("File is not a CSV file");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<RegionMap>();
            var records = csv.GetRecords<Region>();

            RegionImporter regionImporter = new RegionImporter();

            var (createdCount, updatedCount) = await regionImporter.ImportRegions(_dbContext, records.ToList());

            return Ok($"Countries imported successfully. {createdCount} created, {updatedCount} updated.");
        }

        public IActionResult UploadAirports()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UploadAirports(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            if (Path.GetExtension(file.FileName) != ".csv")
            {
                return BadRequest("File is not a CSV file");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<AirportMap>();
            var records = csv.GetRecords<Airport>().ToList();

            _backgroundJobClient.Enqueue<IAirportImportJob>(x => x.Execute(records));

            return Ok($"Airports import queued successfully to import {records.Count}. Check the logs for progress.");
        }

        public IActionResult UploadRunways()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UploadRunways(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            if (Path.GetExtension(file.FileName) != ".csv")
            {
                return BadRequest("File is not a CSV file");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<RunwayMap>();
            var records = csv.GetRecords<RunwayImportPair>().ToList();

            RunwayImporter runwayImporter = new RunwayImporter(_logger);

            Hangfire.BackgroundJob.Enqueue(() => runwayImporter.ImportRunways(_dbContext, records));

            return Ok($"Runways import queued successfully to import {records.Count}. Check the logs for progress.");

        }


    }
}