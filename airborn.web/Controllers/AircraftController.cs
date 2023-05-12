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
using Airborn.web.Models.PerformanceData;

namespace Airborn.web.Controllers
{
    public class AircraftController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public AircraftController(ILogger<HomeController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;

        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Details(string aircraftType)
        {

            AircraftPageModel model = new AircraftPageModel();

            model.Aircraft = Aircraft.GetAircraftFromAircraftType(
                Aircraft.GetAircraftTypeFromAircraftTypeString(aircraftType)
                );

            model.PerformanceDataList = new BookPerformanceDataList(model.Aircraft.LowestPossibleWeight, model.Aircraft.HighestPossibleWeight);

            model.PerformanceDataList.PopulateFromJsonStringPath(model.Aircraft, _env.WebRootPath);

            return View(model);
        }

    }
}