using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Airborn.web.Models;
using Microsoft.AspNetCore.Hosting;

namespace Airborn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Calculate()
        {

            ScenarioPageModel model = new ScenarioPageModel();

            if (Request.Cookies["FieldElevation"]?.Length > 0)
            {
                model.FieldElevation = int.Parse(Request.Cookies["FieldElevation"]);
            }

            if (Request.Cookies["MagneticVariation"]?.Length > 0)
            {
                model.MagneticVariation = int.Parse(Request.Cookies["MagneticVariation"]);
            }

            return View(model);
        }

        // POST: Home/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(ScenarioPageModel model)
        {
            // make sure we have the runways loaded in the model so the dropdown on the page can get them
            if (model.AirportIdentifier?.Length > 0)
            {
                using (var db = new AirportDbContext())
                {
                    model.Runways = db.Runways.Where<Runway>
                        (r => r.Airport_Ident.StartsWith(model.AirportIdentifier.ToUpper())
                        ).ToList<Runway>();
                }
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.LoadAircraftPerformance(_env.WebRootPath);
            }
            catch (PressureAltitudePerformanceProfileNotFoundException e)
            {
                ModelState.AddModelError(
                    "FieldElevation",
                    e.Message
                );

                return View(model);
            }

            catch (TemperaturePerformanceProfileNotFoundException)
            {
                ModelState.AddModelError(
                    "TemperatureCelcius",
                    String.Format(
                        $"No performance data found for Temperature {model.Temperature} {model.TemperatureType}"
                    ))
                    ;

                return View();
            }

            HttpContext.Response.Cookies.Append("FieldElevation", model.FieldElevation.ToString());
            HttpContext.Response.Cookies.Append("MagneticVariation", model.MagneticVariation.ToString());

            // todo: move this out of the controller and into a ScenarioPageModel


            return View(model);
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorPageModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult AutocompleteAirportIdentifier(string term)
        {
            // todo: move this out of the controller and into a ScenarioPageModel

            using (var db = new AirportDbContext())
            {
                var airportIdentifiers = (from airport in db.Airports
                                          where airport.Ident.StartsWith(term.ToUpper())
                                          select new
                                          {
                                              label = airport.Ident,
                                              val = airport.Id
                                          }).ToList();

                return Json(airportIdentifiers);
            }
        }

        public JsonResult PopulateRunways(string airportIdentifier)
        {
            // todo: move this out of the controller and into a ScenarioPageModel

            using (var db = new AirportDbContext())
            {
                var runways = (from runway in db.Runways
                               where runway.Airport_Ident.Equals(airportIdentifier.ToUpper())
                               select new
                               {
                                   label = runway.RunwayIdentifier_Primary,
                                   val = runway.RunwayIdentifier_Primary
                               }
                    ).ToList();

                // this is not the best approach from a performance or code cleanness perspective
                // but the data source we're using stores two runways in one row, so we have to 
                // 're normalise' it somehow
                var runways2 = (from runway in db.Runways
                                where runway.Airport_Ident.Equals(airportIdentifier.ToUpper())
                                select new
                                {
                                    label = runway.RunwayIdentifier_Secondary,
                                    val = runway.RunwayIdentifier_Secondary
                                }
                    ).ToList();

                runways.AddRange(runways2);

                return Json(runways);
            }
        }

        public JsonResult GetAirportInformation(string airportIdentifier)
        {
            // todo: move this out of the controller and into a ScenarioPageModel

            using (var db = new AirportDbContext())
            {
                Airport airport = db.Airports.Single<Airport>(
                    a => a.Ident.Equals(airportIdentifier.ToUpper())
                    );

                return Json(airport);
            }
        }

        public JsonResult GetRunwayDetail(string airportIdentifier, string runwayId)
        {
            // todo: move this out of the controller and into a ScenarioPageModel

            using (var db = new AirportDbContext())
            {
                Runway runway = db.Runways.Single<Runway>(
                    a => (
                            (a.Airport_Ident.Equals(airportIdentifier.ToUpper()))
                            &&
                            (
                                a.RunwayIdentifier_Primary == runwayId
                                ||
                                a.RunwayIdentifier_Secondary == runwayId

                            )
                        )
                    )
                    ;

                return Json(runway);
            }
        }

    }
}
