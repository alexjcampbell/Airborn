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
    /// <summary>
    /// The model for the Calculate page
    /// </summary>
    public class CalculatePageModel
    {

        public CalculatePageModel()
        {
            Runways = new List<Runway>();
            Results = new List<CalculationResultPageModel>();
        }

        private PerformanceCalculator PerformanceCalculator
        {
            get;
            set;
        }

        public string RootPath
        {
            get;
            set;
        }

        public List<CalculationResultPageModel> Results
        {
            get;
            private set;
        }

        public List<CalculationResultPageModel> ResultsSortedByHeadwind
        {
            get
            {
                return Results.OrderByDescending(o => o.HeadwindComponent).ToList();
            }
        }

        public List<Runway> Runways
        {
            get;
            private set;
        }

        [Required(ErrorMessage = "Aircraft Weight is required.")]
        [Range(2000, 4000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Aircraft Weight")]
        public int? AircraftWeight
        {
            get; set;
        }

        [Required(ErrorMessage = "Wind Strength is required.")]
        [Range(0, 200,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Wind Strength")]
        public int? WindStrength
        {
            get; set;
        }

        [Required(ErrorMessage = "Wind Direction is required.")]
        [Range(0, 359,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Wind Direction")]
        public int? WindDirectionMagnetic
        {
            get; set;
        }

        [Required(ErrorMessage = "Airport Identifier is required.")]
        [MaxLength(4)]
        public string AirportIdentifier
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

        [Required(ErrorMessage = "Wind Direction is required.")]
        [Range(-50, 100,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Temperature
        {
            get; set;
        }

        [Required(ErrorMessage = "Wind Direction is required.")]
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

        [Display(Name = "Density Altitude")]
        [Editable(false)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public decimal? DensityAltitude
        {
            get
            {
                return PerformanceCalculator?.DensityAltitude;
            }
        }


        [Display(Name = "Pressure Altitude")]
        [Editable(false)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public decimal? PressureAltitude
        {
            get
            {
                return PerformanceCalculator?.PressureAltitude;
            }
        }

        [Display(Name = "Field Elevation")]
        [Editable(false)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public decimal? FieldElevation
        {
            get
            {
                return Airport?.FieldElevation;
            }
        }

        public decimal? PressureAltitudeAlwaysPositiveOrZero
        {
            get
            {
                return PerformanceCalculator?.PressureAltitudeAlwaysPositiveOrZero;
            }
        }

        public List<string> Notes
        {
            get { return PerformanceCalculator?.Notes; }
        }

        /// <summary>
        /// Sets up the PerformanceCalculator and calculates the results.
        /// </summary>
        /// <param name="db">The DbContext with the airport and runway number</param>
        public void Calculate(AirportDbContext db)
        {


            Wind wind = Wind.FromMagnetic(
                WindDirectionMagnetic.Value,
                MagneticVariation.GetValueOrDefault(),
                WindStrength.Value
                );

            Airport = db.GetAirport(AirportIdentifier);

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(AircraftType);

            PerformanceCalculator = new PerformanceCalculator(
                aircraft,
                Airport,
                wind,
                AircraftWeight.Value,
                RootPath
                )
                ;

            SetTemperatureBasedOnDegreesForC();
            SetAltimeterSettingBasedOnMborHg();

            PerformanceCalculator.Calculate(db);

            foreach (PerformanceCalculationResultForRunway result in PerformanceCalculator.Results)
            {
                CalculationResultPageModel pageModelResult =
                    new CalculationResultPageModel(result, result.Runway);

                Results.Add(pageModelResult);
            }

            SetIsBestWindIfRunwayIsMostIntoWind();

        }

        /// <summary>
        /// Sets the result field IsBestWind to true if the runway is the most into wind.
        /// </summary>
        private void SetIsBestWindIfRunwayIsMostIntoWind()
        {
            foreach (CalculationResultPageModel result in Results)
            {
                if (this.ResultsSortedByHeadwind.First().HeadwindComponent == result.HeadwindComponent)
                {
                    result.IsBestWind = true;
                }
                else
                {
                    result.IsBestWind = false;
                }
            }
        }

        /// <summary>
        /// Sets the QNH based on the AltimeterSettingType
        /// </summary>
        private void SetAltimeterSettingBasedOnMborHg()
        {
            if (AltimeterSettingType == Models.AltimeterSettingType.HG)
            {

                PerformanceCalculator.QNH = CalculationUtilities.InchesOfMercuryToMillibars(AltimeterSetting.Value);
            }
            else if (AltimeterSettingType == Models.AltimeterSettingType.MB)
            {
                PerformanceCalculator.QNH = (int)AltimeterSetting.Value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"AltimeterSettingType {AltimeterSetting.Value} is not valid");
            }
        }

        /// <summary>
        /// Sets the TemperatureInCelcius field, and converts from fahrenheit if necessary.
        /// </summary>
        private void SetTemperatureBasedOnDegreesForC()
        {
            if (TemperatureType == Models.TemperatureType.F)
            {
                PerformanceCalculator.TemperatureCelcius = CalculationUtilities.FahrenheitToCelcius(Temperature.Value);
            }
            else if (TemperatureType == Models.TemperatureType.C)
            {
                PerformanceCalculator.TemperatureCelcius = (int)Temperature.Value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"TemperatureType {TemperatureType.Value} is not valid");
            }
        }
    }
}