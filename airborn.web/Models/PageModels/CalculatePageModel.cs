using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public string RootPath{
            get;
            set;
        }

        public List<PerformanceCalculationResultPageModel> Results
        {
            get;
            set;
        }

        public List<PerformanceCalculationResultPageModel> ResultsSortedByHeadwind
        {
            get
            {
                return Results.OrderByDescending(o => o.HeadwindComponent).ToList();
            }
        }

        public List<Runway> Runways
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Aircraft Weight")]
        public double? AircraftWeight
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

        public Airport Airport
        {
            get;
            set;
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

        public decimal? DensityAltitude
        {
            get
            {
                return PerformanceCalculator?.DensityAltitude;
            }
        }

        public decimal? PressureAltitude
        {
            get
            {
                return PerformanceCalculator?.PressureAltitude;
            }
        }



        public void LoadAircraftPerformance(AirportDbContext db)
        {


            Wind wind = Wind.FromMagnetic(
                WindDirectionMagnetic.Value,
                MagneticVariation.GetValueOrDefault(),
                WindStrength.Value
                );

            Airport = db.GetAirport(AirportIdentifier);

            PerformanceCalculator = new PerformanceCalculator(
                AircraftType,
                Airport,
                wind,
                RootPath
                )
                ;

            PerformanceCalculator.AircraftWeight = AircraftWeight.Value;

            SetTemperatureBasedOnDegreesForC();
            SetAltimeterSettingBasedOnMborHg();

            PerformanceCalculator.Calculate(db);

            foreach (PerformanceCalculationResult result in PerformanceCalculator.Results)
            {
                PerformanceCalculationResultPageModel pageModelResult =
                    new PerformanceCalculationResultPageModel(result, result.Runway);

                Results.Add(pageModelResult);
            }

        }

        private void SetAltimeterSettingBasedOnMborHg()
        {
            if (AltimeterSettingType == Models.AltimeterSettingType.HG)
            {

                PerformanceCalculator.QNH = CalculationUtilities.ConvertInchesOfMercuryToMillibars(AltimeterSetting);
            }
            else
            {
                PerformanceCalculator.QNH = (int)AltimeterSetting.Value;
            }
        }

        private void SetTemperatureBasedOnDegreesForC()
        {
            if (TemperatureType == Models.TemperatureType.F)
            {
                PerformanceCalculator.TemperatureCelcius = CalculationUtilities.ConvertFahrenheitToCelcius(Temperature);
            }
            else
            {
                PerformanceCalculator.TemperatureCelcius = (int)Temperature.Value;
            }
        }
    }
}