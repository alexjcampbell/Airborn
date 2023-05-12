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
    public class WeatherController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public WeatherController(ILogger<HomeController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;

        }

        public async Task<IActionResult> GetMetarForAirport(string airportCode)
        {

            using var myActivity = Telemetry.ActivitySource.StartActivity("GET to GetMetarForAirport");

            myActivity?.SetTag("AirportIdentifier", airportCode);

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