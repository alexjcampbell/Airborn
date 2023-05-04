using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Airborn.web.Models;
using System.Text.Json;

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
            ReadCookies();

            return View(PageModel);
        }

        private void ReadCookies()
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
                else
                {
                    throw new ArgumentOutOfRangeException
                    (
                        $"Unknown TemperatureType {Request.Cookies["TemperatureType"]}"
                    );
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
                else
                {
                    throw new ArgumentOutOfRangeException
                    (
                        $"Unknown AltimeterSettingType {Request.Cookies["AltimeterSettingType"]}"
                    );
                }
            }

            if (Request.Cookies["MagneticVariation"]?.Length > 0)
            {
                PageModel.MagneticVariation = int.Parse(Request.Cookies["MagneticVariation"]);
            }

            if (Request.Cookies["Airport"]?.Length > 0)
            {
                PageModel.AirportIdentifier = Request.Cookies["Airport"];
            }

            if (Request.Cookies["AircraftType"]?.Length > 0)
            {
                PageModel.AircraftType = Aircraft.GetAircraftTypeFromAircraftTypeString(Request.Cookies["AircraftType"]);
            }
        }

        // POST: Home/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(CalculatePageModel model)
        {
            using var myActivity = Telemetry.ActivitySource.StartActivity("POST to Calculate");

            myActivity?.SetTag("AirportIdentifier", model.AirportIdentifier);
            myActivity?.SetTag("AircraftType", model.AircraftType.ToString());
            myActivity?.SetTag("AircraftWeight", model.AircraftWeight.ToString());
            myActivity?.SetTag("AltimeterSetting", model.AltimeterSetting.ToString());
            myActivity?.SetTag("AltimeterSettingType", model.AltimeterSettingType.ToString());
            myActivity?.SetTag("Temperature", model.Temperature.ToString());
            myActivity?.SetTag("WindDirection", model.WindDirectionMagnetic.ToString());
            myActivity?.SetTag("WindSpeed", model.WindStrength.ToString());
            myActivity?.SetTag("TemperatureType", model.TemperatureType.ToString());
            myActivity?.SetTag("IsModelValid", ModelState.IsValid.ToString());

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
                if (model.RootPath == null)
                {
                    model.RootPath = _env.WebRootPath;
                }
                model.Calculate(_dbContext);
            }
            catch (NoPerformanceDataFoundException e)
            {
                ModelState.AddModelError(
                    "AltimeterSetting",
                    e.Message
                );

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
            catch (AirportNotFoundException e)
            {
                ModelState.AddModelError(
                    "AirportIdentifier",
                    String.Format(
                        $"No airport found for '{e.AirportName}'"
                    )
                );

                return View(model);
            }

            myActivity?.SetTag("PerformanceResultsFound", model.Results.Count);
            myActivity?.SetTag("FieldElevation", model.FieldElevation.ToString());
            myActivity?.SetTag("PressureAltitude", model.PressureAltitude.ToString());
            myActivity?.SetTag("DensityAltitude", model.DensityAltitude.ToString());

            SetCookies(model);

            return View(model);
        }

        private void SetCookies(CalculatePageModel model)
        {
            // check for null before we try to add cookies, so we can unit test this controller
            if (HttpContext != null)
            {
                HttpContext.Response.Cookies.Append("TemperatureType", model.TemperatureType.ToString());
                HttpContext.Response.Cookies.Append("AltimeterSettingType", model.AltimeterSettingType.ToString());
                HttpContext.Response.Cookies.Append("MagneticVariation", model.MagneticVariation.ToString());
                HttpContext.Response.Cookies.Append("Airport", model.AirportIdentifier);
            }
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
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

        public string GetAircraftMaxGrossWeight(string aircraftType)
        {
            return Aircraft.GetAircraftFromAircraftType(
                    Enum.Parse<AircraftType>(aircraftType)
                ).GetHigherWeight().ToString();
        }

    }
}
