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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Web;


namespace Airborn.web.Controllers
{
    [Authorize(Roles = "AirbornAdmin")]
    public class AirportImportController : Controller
    {
        private readonly ILogger<AirportImportController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public AirportImportController(ILogger<AirportImportController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;
        }

        public IActionResult UploadContinents()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadContinents(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "No file selected";
            }

            return View("Index");
        }
    }
}