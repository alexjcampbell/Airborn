using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Airborn.web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace Airborn.web.Controllers
{
    public class AirportsController : Controller
    {


        private readonly ILogger<AirportsController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public AirportsController(ILogger<AirportsController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;

        }


        // GET: Airports
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to Airports/Index");
            myActivity?.SetTag("SearchString", searchString);
            myActivity?.SetTag("SortOrder", sortOrder);
            myActivity?.SetTag("PageNumber", pageNumber);

            ViewData["IdentSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Ident" : "Ident_Desc";
            ViewData["CurrentFilter"] = searchString;

            int pageSize = 20;

            var airports = await GetAirports(sortOrder, searchString, pageNumber ?? 1, pageSize);

            return View(airports);
        }

        private async Task<PaginatedList<Airport>> GetAirports(string sortOrder, string searchString, int pageNumber, int pageSize)
        {


            var airports = from a in _dbContext.Airports
                           .Include(a => a.Runways)
                           select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                airports = airports.Include(a => a.Runways).Where(s => s.Ident.Contains(searchString.ToUpper())
                                    || s.Name.Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "Ident":
                    airports = airports.OrderBy(a => a.Name);
                    break;
                case "Ident_Desc":
                    airports = airports.OrderByDescending(a => a.Ident);
                    break;
                default:
                    airports = airports.OrderBy(a => a.Ident);
                    break;
            }

            return await PaginatedList<Airport>.CreateAsync(airports.AsNoTracking(), pageNumber, pageSize);
        }

        // GET: Airports/Details/5
        public ActionResult Details(string ident)
        {


            Airport airport = _dbContext.Airports.Include(a => a.Runways).Where(
                a => a.Ident == ident
                ).FirstOrDefault();

            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to Airports/Details/{ident}");
            myActivity?.SetTag("AirportIdentifier", ident);

            if (airport == null)
            {
                myActivity?.SetTag("AirportIdentifierNotFound", ident);

                return NotFound();
            }

            return View(airport);
        }

        // GET: Airports/Create
        public ActionResult Create()
        {
            throw new NotImplementedException();

            //return View();
        }

        // POST: Airports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            throw new NotImplementedException();

            /*

            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }

            */
        }

        // GET: Airports/Edit/5
        public ActionResult Edit(int id)
        {
            throw new NotImplementedException();

            // return View();
        }

        // POST: Airports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            throw new NotImplementedException();

            /*
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
            */
        }

        // GET: Airports/Delete/5
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();

            // return View();
        }

        // POST: Airports/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            throw new NotImplementedException();

            /*           try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
            
        */
        }

        // GET: Airports/Continents
        public IActionResult Continents()
        {
            var continents = _dbContext.Continents.Include(c => c.Countries).ToList();
            return View(continents);
        }


        public IActionResult Continent(int id)
        {
            var continent = _dbContext.Continents
                .Include(c => c.Countries)
                .FirstOrDefault(c => c.Continent_Id == id);

            if (continent == null)
            {
                return NotFound();
            }

            return View(continent);
        }

        public IActionResult Countries()
        {
            var countries = _dbContext.Countries.Include(c => c.Continent).ToList();
            return View(countries);
        }

        public IActionResult Country(int id)
        {
            var country = _dbContext.Countries
                .Include(c => c.Continent)
                .Include(a => a.Airports)
                .FirstOrDefault(c => c.Country_Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

    }
}