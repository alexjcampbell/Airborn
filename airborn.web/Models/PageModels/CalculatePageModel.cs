using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    public class CalculatePageModel
    {

        public CalculatePageModel()
        {
            Runways = new List<Runway>();
            Results = new List<PerformanceCalculationResultPageModel>();
        }

        private PerformanceCalculator PerformanceCalculator
        {
            get;
            set;
        }

        public List<PerformanceCalculationResultPageModel> Results
        {
            get;
            set;
        }


        public List<Runway> Runways
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Aircraft Weight")]
        public int? AircraftWeight
        {
            get; set;
        }

        public RunwaySurface RunwaySurface
        {
            get; set;
        }

        [Required]
        [Range(0, 200,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Wind Strength")]
        public int? WindStrength
        {
            get; set;
        }

        [Required]
        [Range(0, 359,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Wind Direction")]
        public int? WindDirectionMagnetic
        {
            get; set;
        }

        public string AirportIdentifier
        {
            get; set;
        }

        public string RunwayIdentifier
        {
            get; set;
        }

        public AircraftType AircraftType
        {
            get; set;
        }

        [Required]
        [Range(-2000, 20000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Field Elevation")]
        public int? FieldElevation
        {
            get; set;
        }

        [Range(-50, 50,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? MagneticVariation
        {
            get; set;
        }

        [Required]
        [Range(-50, 100,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal? Temperature
        {
            get; set;
        }

        [Required]
        [Display(Name = "Altimeter Setting")]
        public decimal? AltimeterSetting
        {
            get; set;
        }

        [Required]
        public TemperatureType? TemperatureType
        {
            get; set;
        }

        [Required]
        public AltimeterSettingType? AltimeterSettingType
        {
            get; set;
        }

        public double? DensityAltitude
        {
            get
            {
                return PerformanceCalculator?.DensityAltitude;
            }
        }

        public double? PressureAltitude
        {
            get
            {
                return PerformanceCalculator?.PressureAltitude;
            }
        }



        public void LoadAircraftPerformance(string rootPath)
        {


            Wind wind = Wind.FromMagnetic(
                WindDirectionMagnetic.Value,
                MagneticVariation.GetValueOrDefault(),
                WindStrength.Value
                );

            PerformanceCalculator = new PerformanceCalculator(
                AircraftType,
                GetAirport(AirportIdentifier),
                wind,
                rootPath
                )
                ;

            PerformanceCalculator.Airport.FieldElevation = FieldElevation.Value;

            PerformanceCalculator.AircraftWeight = AircraftWeight.Value;

            if (TemperatureType == Models.TemperatureType.F)
            {
                PerformanceCalculator.TemperatureCelcius = CalculationUtilities.ConvertFahrenheitToCelcius(Temperature);
            }
            else
            {
                PerformanceCalculator.TemperatureCelcius = (int)Temperature.Value;
            }

            if (AltimeterSettingType == Models.AltimeterSettingType.HG)
            {

                PerformanceCalculator.QNH = CalculationUtilities.ConvertInchesOfMercuryToMillibars(AltimeterSetting);
            }
            else
            {
                PerformanceCalculator.QNH = (int)AltimeterSetting.Value;
            }

            PerformanceCalculator.Calculate();

            foreach (PerformanceCalculationResult result in PerformanceCalculator.Results)
            {
                PerformanceCalculationResultPageModel pageModelResult =
                    new PerformanceCalculationResultPageModel(result, result.Runway);

                Results.Add(pageModelResult);
            }

        }


        public Airport GetAirport(string airportIdentifier)
        {

            using (var db = new AirportDbContext())
            {
                Airport airport = db.Airports.Single<Airport>(
                    a => a.Ident.Equals(airportIdentifier.ToUpper())
                    );

                return airport;
            }
        }

        public void GetRunwaysForAirport()
        {
            using (var db = new AirportDbContext())
            {
                this.Runways = db.Runways.Where<Runway>
                    (r => r.Airport_Ident.StartsWith(this.AirportIdentifier.ToUpper())
                    ).ToList<Runway>();
            }
        }        

        public static List<KeyValuePair<string, string>> SearchForAirportsByIdentifier(string term)
        {
            using (var db = new AirportDbContext())
            {
                DateTime start = DateTime.Now;

                var airportIdentifiers = (from airport in db.Airports
                                          where EF.Functions.Like(airport.Ident, term + "%")
                                          select new
                                          {
                                              label = airport.Ident,
                                              val = airport.Id
                                          }).AsNoTracking().Take(20);

                DateTime end = DateTime.Now;

                List<KeyValuePair<string, string>> airportIdentifiersList = new List<KeyValuePair<string, string>>();

                foreach(var airport in airportIdentifiers)
                {
                    airportIdentifiersList.Add(new KeyValuePair<string, string>(airport.val.ToString(), airport.label));
                }

                // Remnants of past performance debugging:
                // Trace.WriteLine($"Query time taken: {(end - start).TotalSeconds}");

                return airportIdentifiersList;
            }        
        }

    }
}