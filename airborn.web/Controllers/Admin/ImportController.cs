using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;
using CsvHelper.Configuration;
using Airborn.web.Models;
using Airborn.web.Models.ImportModels;

namespace Airborn.web.Controllers
{
    [Authorize(Roles = "AirbornAdmin")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public IActionResult Index()
        {
            return View();
        }

        public ImportController(ILogger<ImportController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;
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
        public async Task<IActionResult> UploadAirports(IFormFile file)
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

            AirportImporter airportImporter = new AirportImporter();
            var (createdCount, updatedCount) = await airportImporter.ImportAirports(_dbContext, records);

            return Ok($"Airports imported successfully. {createdCount} created, {updatedCount} updated.");
        }

        public IActionResult UploadRunways()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadRunways(IFormFile file)
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

            RunwayImporter runwayImporter = new RunwayImporter();
            var (createdCount, updatedCount) = await runwayImporter.ImportRunways(_dbContext, records);

            return Ok($"Runways imported successfully. {createdCount} created, {updatedCount} updated.");
        }

        public async Task<IActionResult> AddMissingSlugs()
        {

            var countries = _dbContext.Countries.ToList();

            foreach (var country in countries)
            {
                country.Slug = TextUtilities.ToUrlSlug(country.CountryName);
            }


            var continents = _dbContext.Continents.ToList();

            foreach (var continent in continents)
            {
                continent.Slug = TextUtilities.ToUrlSlug(continent.ContinentName);
            }


            var regions = _dbContext.Regions.ToList();

            foreach (var region in regions)
            {
                region.Slug = TextUtilities.ToUrlSlug(region.Name);
            }

            await _dbContext.SaveChangesAsync();

            return Ok("Slugs updated successfully.");
        }


    }
}