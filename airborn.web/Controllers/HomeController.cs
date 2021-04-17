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

            if(Request.Cookies["FieldElevation"]?.Length > 0)
            {
                model.FieldElevation = int.Parse(Request.Cookies["FieldElevation"]);
            }

            if(Request.Cookies["MagneticVariation"]?.Length > 0)
            {
                model.MagneticVariation = int.Parse(Request.Cookies["MagneticVariation"]);
            }

            return View(model);
        }

        // POST: Home/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(ScenarioPageModel model)
        {
            if (!ModelState.IsValid)    
            {
                return View();
            }
            
            model.Initialise(System.IO.Path.Combine(_env.WebRootPath, "../SR22_G2.json"));

            HttpContext.Response.Cookies.Append("FieldElevation", model.FieldElevation.ToString());
            HttpContext.Response.Cookies.Append("MagneticVariation", model.MagneticVariation.ToString());

            return View(model);
        }

        public IActionResult Terms()
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
