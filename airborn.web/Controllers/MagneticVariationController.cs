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

namespace Airborn.Controllers
{
    public class MagneticVariationController : Controller
    {
        private readonly ILogger<MagneticVariationController> _logger;
        private readonly AirbornDbContext _dbContext;

        private IWebHostEnvironment _env;

        public MagneticVariationController(ILogger<MagneticVariationController> logger, IWebHostEnvironment env, AirbornDbContext dbContext)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;
        }

        /*

        public string GetMagneticVariationForUSAirports()
        {

            FaaDataParser reader = new FaaDataParser();
            var airports = reader.Parse();

            int count = 0;

            foreach (var airport in airports)
            {
                Airport airportFromDb;

                try
                {
                    airportFromDb = _dbContext.GetAirport(airport.Key);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }

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

        */

        /*

        public async Task<IActionResult> GetMagneticVariationForNonUSAirports()
        {
            throw new NotImplementedException();



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
           */

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
