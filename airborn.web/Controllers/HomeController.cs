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
                ).GetHigherWeight().ToString();
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

            return Aircraft.GetAircraftFromAircraftType(aircraftTypeEnum).HasAirconOption();
        }

        public async Task<IActionResult> GetMetarForAirport(string airportCode)
        {

            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to GetMetarForAirport");

            myActivity?.SetTag("AirportCode", airportCode);

            if (string.IsNullOrWhiteSpace(airportCode))
            {
                return BadRequest("Invalid airport code");
            }

            try
            {
                var metarData = await new AwcApiClient().GetLatestMetarForAirport(airportCode);

                metarData.AirportDbContext = _dbContext;

                if (metarData != null)
                {
                    return Json(new
                    {
                        stationId = metarData.StationId,
                        observationTime = metarData.ObservationTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        temperature = metarData.Temperature,
                        windSpeed = metarData.WindSpeed,
                        windDirection = metarData.WindDirection, // Include wind direction in the JSON response
                        windDirectionMagnetic = metarData.WindDirectionMagnetic, // Include wind direction in the JSON response
                        altimeterSetting = metarData.AltimeterSetting // Include altimeter in the JSON response
                    });
                }
                else
                {
                    return BadRequest($"No METAR data found for airport {airportCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                _logger.LogError(ex, "Error retrieving METAR data for airport {AirportCode}", airportCode);
                return BadRequest("Error retrieving METAR data");
            }
        }

        public string GetMagneticVariationForAirports()
        {
            FaaDataParser reader = new FaaDataParser();
            var airports = reader.Parse();

            int count = 0;

            foreach (var airport in airports)
            {
                Airport airportFromDb = _dbContext.GetAirport(airport.Key);

                if (airportFromDb != null)
                {
                    airportFromDb.MagneticVariation = double.Parse(airport.Value);
                    _dbContext.Update(airportFromDb);

                    _dbContext.SaveChanges();
                    count++;
                }
            }

            return count.ToString();
        }

        public async Task<IActionResult> GetMagneticVariationForNonUSAirports()
        {
            GeomagClient client = new GeomagClient();

            List<MagVarResult> updates = new List<MagVarResult>();

            List<Airport> airports = _dbContext.GetAirports().Where(a => a.MagneticVariation == null).ToList();

            foreach (var airport in airports)
            {
                if (
                    (airport.MagneticVariation.HasValue && airport.MagneticVariation != 0)
                )
                {
                    continue;
                }
                {
                    double? result = await
                        client.GetGeomagDataAsync(airport.Latitude_Deg.Value, airport.Longitude_Deg.Value, airport.FieldElevation.Value, DateTime.Now);

                    MagVarResult magVarResult = new MagVarResult
                    {
                        AirportIdent = airport.Ident,
                        Latitude = airport.Latitude_Deg.Value,
                        Longitude = airport.Longitude_Deg.Value,
                        FieldElevation = airport.FieldElevation.Value,
                        MagneticVariation = result
                    };

                    airport.MagneticVariation = result;
                    _dbContext.SaveChanges();
                    updates.Add(magVarResult);
                }
            }

            return Json(updates);
        }

        public class MagVarResult
        {
            public string AirportIdent { get; set; }
            public double? MagneticVariation { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double FieldElevation { get; set; }

            public override string ToString()
            {
                return $"{AirportIdent} {Latitude} {Longitude} {FieldElevation} {MagneticVariation}";
            }
        }
    }
}
