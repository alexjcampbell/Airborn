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
            Results = new List<CalculationResultForRunwayPageModel>();
        }

        private Calculation PerformanceCalculator
        {
            get;
            set;
        }

        public string RootPath
        {
            get;
            set;
        }

        public List<CalculationResultForRunwayPageModel> Results
        {
            get;
            private set;
        }

        public List<CalculationResultForRunwayPageModel> ResultsSortedByHeadwind
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
        [Display(Name = "Aircraft Weight")]
        [ValidAircraftWeight]
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

        [ValidAircraftType(ErrorMessage = "Aircraft Type is required.")]
        public string AircraftType
        {
            get; set;
        }

        public AirconOptions AirconOption
        {
            get; set;
        }

        public Airport Airport
        {
            get;
            set;
        }

        private PerformanceCalculationLog _logger = new PerformanceCalculationLog();

        public PerformanceCalculationLog Logger
        {
            get
            {
                return _logger;
            }
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

        [Required]
        public TemperatureType? TemperatureType
        {
            get; set;
        }

        [Required(ErrorMessage = "Altimeter setting is required.")]
        [Display(Name = "Altimeter Setting")]
        [ValidAltimeterSetting]
        public double? AltimeterSetting
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
        public Distance? DensityAltitude
        {
            get
            {
                return PerformanceCalculator?.DensityAltitude;
            }
        }


        [Display(Name = "Pressure Altitude")]
        [Editable(false)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public Distance? PressureAltitude
        {
            get
            {
                return PerformanceCalculator?.PressureAltitude;
            }
        }

        [Display(Name = "Field Elevation")]
        [Editable(false)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public Distance? FieldElevation
        {
            get
            {
                return Airport?.FieldElevationAsDistance;
            }
        }

        public double? PressureAltitudeAlwaysPositiveOrZero
        {
            get
            {
                return PerformanceCalculator?.PressureAltitudeAdjustedForBounds.TotalFeet;
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
        public void Calculate(AirbornDbContext db)
        {


            Wind wind = Wind.FromMagnetic(
                WindDirectionMagnetic.Value,
                MagneticVariation.GetValueOrDefault(),
                WindStrength.Value
                );

            Airport = db.GetAirport(AirportIdentifier);

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(AircraftType);

            PerformanceCalculator = new Calculation(
                aircraft,
                Airport,
                wind,
                AircraftWeight.Value,
                RootPath
                )
                ;

            SetTemperatureBasedOnDegreesForC();
            SetAltimeterSettingBasedOnMborHg();

            PerformanceCalculator.AirconOption = AirconOption;

            PerformanceCalculator.Calculate(db);

            foreach (CalculationResultForRunway result in PerformanceCalculator.Results)
            {
                CalculationResultForRunwayPageModel pageModelResult =
                    new CalculationResultForRunwayPageModel(result, result.Runway);

                Results.Add(pageModelResult);
            }

            SetIsBestWindIfRunwayIsMostIntoWind();

            _logger = PerformanceCalculator.Logger;

        }

        /// <summary>
        /// Sets the result field IsBestWind to true if the runway is the most into wind.
        /// </summary>
        private void SetIsBestWindIfRunwayIsMostIntoWind()
        {
            foreach (CalculationResultForRunwayPageModel result in Results)
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

                PerformanceCalculator.AltimeterSettingInMb = CalculationUtilities.InchesOfMercuryToMillibars(AltimeterSetting.Value);
            }
            else if (AltimeterSettingType == Models.AltimeterSettingType.MB)
            {
                PerformanceCalculator.AltimeterSettingInMb = (int)AltimeterSetting.Value;
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