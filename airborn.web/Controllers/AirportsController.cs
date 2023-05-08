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

namespace Airborn.web.Controllers
{
    public class AirportsController : Controller
    {


        private readonly ILogger<AirportsController> _logger;
        private readonly AirportDbContext _dbContext;

        private IWebHostEnvironment _env;

        public AirportsController(ILogger<AirportsController> logger, IWebHostEnvironment env, AirportDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;

        }


        // GET: Airports
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {

            ViewData["IdentSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Ident" : "Ident_Desc";
            ViewData["CurrentFilter"] = searchString;


            var airports = from a in _dbContext.Airports
                           select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                airports = airports.Where(s => s.Ident.Contains(searchString.ToUpper())
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

            int pageSize = 20;

            return View(await PaginatedList<Airport>.CreateAsync(airports.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Airports/Details/5
        public ActionResult Details(int id)
        {
            Airport airport = _dbContext.Airports.Where(a => a.Airport_Id == id).FirstOrDefault();

            airport.Runways = _dbContext.Runways.Where(r => r.Airport_Id == airport.Airport_Id).ToList();

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
    }
}