using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Airborn.web.Models;
using Microsoft.EntityFrameworkCore;

namespace Airborn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AirportDbContext _dbContext;

        private IWebHostEnvironment _env;

        public CalculatePageModel PageModel
        {
            get;
            set;
        }

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, AirportDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;
            PageModel = new CalculatePageModel();
            PageModel.RootPath = env.WebRootPath;

        }

        public IActionResult Index()
        {
            return View(PageModel);
        }

        public IActionResult Calculate()
        {


            if (Request.Cookies["TemperatureType"]?.Length > 0)
            {
                if (Request.Cookies["TemperatureType"] == "C")
                {
                    PageModel.TemperatureType = TemperatureType.C;
                }
                else if (Request.Cookies["TemperatureType"] == "F")
                {
                    PageModel.TemperatureType = TemperatureType.F;
                }
            }

            if (Request.Cookies["AltimeterSettingType"]?.Length > 0)
            {
                if (Request.Cookies["AltimeterSettingType"] == "MB")
                {
                    PageModel.AltimeterSettingType = AltimeterSettingType.MB;
                }
                else if (Request.Cookies["AltimeterSettingType"] == "HG")
                {
                    PageModel.AltimeterSettingType = AltimeterSettingType.HG;
                }
            }

            if (Request.Cookies["MagneticVariation"]?.Length > 0)
            {
                PageModel.MagneticVariation = int.Parse(Request.Cookies["MagneticVariation"]);
            }

            return View(PageModel);
        }

        // POST: Home/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(CalculatePageModel model)
        {

            // make sure we have the runways loaded in the model so the dropdown on the page can get them
            if (model.AirportIdentifier?.Length > 0)
            {
                _dbContext.GetRunwaysForAirport(model.AirportIdentifier);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // if we're running this from a Controller test, RootPath will already be set, so don't override it
                if(model.RootPath == null)
                {
                    model.RootPath = _env.WebRootPath;
                }
                model.LoadAircraftPerformance(_dbContext);
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

                return View(model);
            }

            catch (AircraftWeightOutOfRangeException e)
            {
                ModelState.AddModelError(
                    "AircraftWeight",
                    String.Format(
                        $"No performance data found for Weight {model.AircraftWeight}, weight must be between {e.MinWeight} and {e.MaxWeight}"
                    ))
                    ;

                return View(model);
            }
            /*
            catch (Exception e)
            {
                ModelState.AddModelError(
                    "AircraftType",
                    e.Message
                );

                return View(model);
            }
            */

            // check for null before we try to add cookies, so we can unit test this controller
            if(HttpContext != null)
            {
                HttpContext.Response.Cookies.Append("TemperatureType", model.TemperatureType.ToString());
                HttpContext.Response.Cookies.Append("AltimeterSettingType", model.AltimeterSettingType.ToString());
                HttpContext.Response.Cookies.Append("MagneticVariation", model.MagneticVariation.ToString());
            }

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

            term = term?.ToUpper();
            return Json(_dbContext.SearchForAirportsByIdentifier(term));

        }

        public JsonResult GetAirportInformation(string airportIdentifier)
        {
            return Json(_dbContext.GetAirport(airportIdentifier));
        }

    }
}
