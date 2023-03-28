using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Airborn.web.Models
{
    public class ScenarioPageModel
    {

        public ScenarioPageModel()
        {
            Runways = new List<Runway>();
            Results = new List<PerformanceCalculationResultForRunwayPageModel>();
        }

        private PerformanceCalculator PerformanceCalculator
        {
            get;
            set;
        }

        public List<PerformanceCalculationResultForRunwayPageModel> Results
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
                PerformanceCalculationResultForRunwayPageModel pageModelResult =
                    new PerformanceCalculationResultForRunwayPageModel(result, result.Runway);

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

    }
}