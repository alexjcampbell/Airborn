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

namespace Airborn.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public CalculatePageModel PageModel
        {
            get;
            set;
        }

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
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

            PageModel.AircraftWeight = 3400;

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

            if (_dbContext.GetAirport(model.AirportIdentifier) == null)
            {
                ModelState.AddModelError("AirportIdentifier", "Airport not found.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // if we're running this from a Controller test, RootPath will already be set, so don't override it
            if (model.RootPath == null)
            {
                model.RootPath = _env.WebRootPath;
            }

            model.Calculate(_dbContext);

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
            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to Error");

            myActivity?.SetTag("RequestId", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            myActivity?.SetTag("Headers", HttpContext.Request.Headers);

            return View(new ErrorPageModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult AutocompleteAirportIdentifier(string term)
        {
            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to AutocompleteAirportIdentifier");

            myActivity?.SetTag("AirportIdentifier", term);

            term = term?.ToUpper();
            return Json(_dbContext.SearchForAirportsByIdentifier(term));
        }

        public JsonResult GetAirportInformation(string airportIdentifier)
        {
            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to GetAirportInformation");

            myActivity?.SetTag("AirportIdentifier", airportIdentifier);

            return Json(_dbContext.GetAirport(airportIdentifier));
        }

        public string GetAircraftMaxGrossWeight(string aircraftType)
        {
            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to GetAircraftMaxGrossWeight");

            myActivity?.SetTag("AircraftType", aircraftType);

            AircraftType aircraftTypeEnum;

            if (Enum.TryParse(aircraftType, out aircraftTypeEnum))
            {
                myActivity?.SetTag("AircraftTypeEnum", aircraftTypeEnum.ToString());
            }
            else
            {
                myActivity?.SetTag("AircraftTypeEnum", "Unknown: " + aircraftType);
            }

            return Aircraft.GetAircraftFromAircraftType(
                    aircraftTypeEnum
                ).HighestPossibleWeight.ToString();
        }

        public bool IsAirconditioned(string aircraftType)
        {
            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to IsAirconditioned");

            myActivity?.SetTag("AircraftType", aircraftType);

            AircraftType aircraftTypeEnum;

            if (Enum.TryParse(aircraftType, out aircraftTypeEnum))
            {
                myActivity?.SetTag("AircraftTypeEnum", aircraftTypeEnum.ToString());
            }
            else
            {
                myActivity?.SetTag("AircraftTypeEnum", "Unknown: " + aircraftType);
            }

            return Aircraft.GetAircraftFromAircraftType(aircraftTypeEnum).HasAirconOption;
        }

        public async Task<IActionResult> GetMetarForAirport(string airportCode)
        {

            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to GetMetarForAirport");

            myActivity?.SetTag("AirportCode", airportCode);

            if (string.IsNullOrWhiteSpace(airportCode))
            {
                return BadRequest("Invalid airport code");
            }

            var metarData = await new AwcApiClient().GetLatestMetarForAirport(airportCode);

            metarData.DbContext = _dbContext;

            return Json(new
            {
                isError = metarData.IsError,
                stationId = metarData.StationId,
                observationTime = metarData.ObservationTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                temperature = metarData.Temperature,
                windSpeed = metarData.WindSpeed,
                windDirection = metarData.WindDirection, // Include wind direction in the JSON response
                windDirectionMagnetic = metarData.WindDirectionMagnetic, // Include wind direction in the JSON response
                altimeterSetting = metarData.AltimeterSetting // Include altimeter in the JSON response
            });

        }
    }
}
