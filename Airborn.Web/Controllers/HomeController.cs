using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Airborn.web.Models;

namespace Airborn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Takeoff()
        {

           AircraftPerformance ap = AircraftPerformance.CreateFromJson();

            Scenario scenario = new Scenario(300, -20, 250, 20);

            scenario.TemperatureCelcius = 15;
            scenario.PressureAltitude = 1500;

            int takeoffGroundRoll = ap.CalculateTakeoffDistanceGroundRoll(scenario, AircraftPerformance.ScenarioMode.Takeoff_GroundRoll);

            return View();
        }

        // POST: Home/Takeoff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Takeoff(ScenarioPageModel model)
        {
            
            return View(model);
        }

       public IActionResult Landing()
        {
            return View();
        }

        // POST: Home/Calculator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Landing(ScenarioPageModel model)
        {
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
