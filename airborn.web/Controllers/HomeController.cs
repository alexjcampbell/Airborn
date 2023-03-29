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

        private IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            CalculatePageModel model = new CalculatePageModel();

            return View(model);
        }

        public IActionResult Calculate()
        {

            CalculatePageModel model = new CalculatePageModel();


            if (Request.Cookies["TemperatureType"]?.Length > 0)
            {
                if (Request.Cookies["TemperatureType"] == "C")
                {
                    model.TemperatureType = TemperatureType.C;
                }
                else if (Request.Cookies["TemperatureType"] == "F")
                {
                    model.TemperatureType = TemperatureType.F;
                }
            }

            if (Request.Cookies["AltimeterSettingType"]?.Length > 0)
            {
                if (Request.Cookies["AltimeterSettingType"] == "MB")
                {
                    model.AltimeterSettingType = AltimeterSettingType.MB;
                }
                else if (Request.Cookies["AltimeterSettingType"] == "HG")
                {
                    model.AltimeterSettingType = AltimeterSettingType.HG;
                }
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
        public IActionResult Calculate(CalculatePageModel model)
        {

            // make sure we have the runways loaded in the model so the dropdown on the page can get them
            if (model.AirportIdentifier?.Length > 0)
            {
                model.GetRunwaysForAirport();
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
            catch (Exception e)
            {
                ModelState.AddModelError(
                    "AircraftType",
                    e.Message
                );

                return View(model);
            }

            HttpContext.Response.Cookies.Append("TemperatureType", model.TemperatureType.ToString());
            HttpContext.Response.Cookies.Append("AltimeterSettingType", model.AltimeterSettingType.ToString());
            HttpContext.Response.Cookies.Append("MagneticVariation", model.MagneticVariation.ToString());

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

            term = term.ToUpper();

            return Json(CalculatePageModel.SearchForAirportsByIdentifier(term));

        }

        public JsonResult GetAirportInformation(string airportIdentifier)
        {

            return Json(CalculatePageModel.GetAirport(airportIdentifier));
        }

    }
}
