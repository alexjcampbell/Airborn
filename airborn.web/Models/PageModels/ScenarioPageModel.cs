using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Airborn.web.Models
{

    

    public class ScenarioPageModel
    {

        public Aircraft Aircraft
        {
            get;
            set;
        }

        public Scenario Scenario
        {
            get;
            set;
        }        

        public PerformanceCalculation Calculation
        {
            get;
            set;
        }

        private List<Runway> _runways = new List<Runway>();

        public List<Runway> Runways
        {
            get
            {
                return _runways;
            }
            set
            {
                _runways = value;
            }
        }

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
        public int? WindStrength
        {
            get; set;
        }

        [Required]
        [Range(0, 359,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
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
        [Range(0, 359,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? RunwayHeading
        {
            get; set;
        }

        [Required]
        [Range(-2000, 20000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? FieldElevation
        {
            get; set;
        }

        [Range(0, 20000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? RunwayLength
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
                return Scenario?.DensityAltitude;
            }
        }

        public double? PressureAltitude
        {
            get
            {
                return Scenario?.PressureAltitude;
            }
        }


        public double? CrosswindComponent
        {
            get
            {
                return Scenario?.CrosswindComponent;
            }
        }

        public double? CrosswindComponentAbs
        {
            get
            {

                var crosswindComponent = Scenario?.CrosswindComponent;

                if (crosswindComponent < 0)
                {
                    crosswindComponent = crosswindComponent * -1;
                }

                return crosswindComponent;
            }
        }

        public string CrosswindComponentText
        {
            get
            {
                if (Scenario?.CrosswindComponent > 0)
                {
                    return "Right";
                }
                else if (Scenario?.CrosswindComponent < 0)
                {
                    return "Left";
                }
                return "";
            }
        }

        public string HeadwindComponentText
        {
            get
            {
                if (Scenario?.HeadwindComponent > 0)
                {
                    return "Headwind";
                }
                else if (Scenario?.HeadwindComponent < 0)
                {
                    return "Tailwind";
                }
                return "Headwind";
            }
        }
        public double? HeadwindComponent
        {
            get
            {
                return Scenario?.HeadwindComponent;
            }

        }

        public double? HeadwindComponentAbs
        {
            get
            {

                // if it's a tailwind we'll describe it as such, and the page caller needs an absolute value
                // of the wind component we calculated to show the tailwind amount

                var headwindComponent = Scenario?.HeadwindComponent;

                if (headwindComponent < 0)
                {
                    headwindComponent = headwindComponent * -1;
                }

                return headwindComponent;
            }
        }

        public double? Takeoff_GroundRoll
        {
            get
            {
                return Calculation?.Takeoff_GroundRoll;
            }
        }


        public string Takeoff_Groundroll_Formatted
        {
            get
            {
                if (Calculation != null)
                {

                    return
                        Calculation?.Takeoff_GroundRoll.ToString()
                        +
                        " ft ("
                        +
                        Takeoff_PercentageRunwayUsed_GroundRoll?.ToString("P0")
                        + " of available)"
                        ;
                }
                
                return "";
            }
        }


        public double? Takeoff_50FtClearance
        {
            get
            {
                return Calculation?.Takeoff_50FtClearance;
            }
        }

        public string Takeoff_50FtClearance_Formatted
        {
            get
            {
                if (Calculation != null)
                {
                    return
                        Calculation?.Takeoff_50FtClearance.ToString()
                        +
                        " ft ("
                        +
                        Takeoff_PercentageRunwayUsed_DistanceToClear50Ft?.ToString("P0")
                        + " of available)"
                        ;
                }

                return "";
            }
        }        

        public double? Landing_GroundRoll
        {
            get
            {
                return Calculation?.Landing_GroundRoll;
            }
        }

        public string Landing_Groundroll_Formatted
        {
            get
            {
                if (Calculation != null)
                {
                    return
                        Calculation?.Landing_GroundRoll.ToString()
                        +
                        " ft ("
                        +
                        Landing_PercentageRunwayUsed_GroundRoll?.ToString("P0")
                        + " of available)"
                        ;
                }
                
                return "";

            }
        }

        public double? Landing_50FtClearance
        {
            get
            {
                return Calculation?.Landing_50FtClearance;
            }
        }

        public string Landing_50FtClearance_Formatted
        {
            get
            {
                if (Calculation != null)
                {
                    return
                        Calculation?.Landing_50FtClearance.ToString()
                        +
                        " ft ("
                        +
                        Landing_PercentageRunwayUsed_DistanceToClear50Ft?.ToString("P0")
                        + " of available)"
                        ;
                }
                return "";
            }
        }



        public double? Takeoff_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Takeoff_GroundRoll / RunwayLength;
            
            }
        }

        public double? Landing_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                 return Landing_GroundRoll / RunwayLength;
            }
        }

        public double? Takeoff_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Takeoff_50FtClearance / RunwayLength;
            }
        }

        public double? Landing_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Landing_50FtClearance / RunwayLength;
            }
        }        

        public void LoadAircraftPerformance(string rootPath)
        {

            Runway runway = Runway.FromMagnetic(
                    RunwayHeading.Value,

                    // assume magnetic variation is zero if not supplied
                    MagneticVariation.GetValueOrDefault()
                );

            Wind wind = Wind.FromMagnetic(
                WindDirectionMagnetic.Value,
                MagneticVariation.GetValueOrDefault(),
                WindStrength.Value
                );

            Scenario = new Scenario(runway, wind);

            Scenario.FieldElevation = FieldElevation.Value;

            // runway length is not mandatory as it is only used for the 
            // '% of runway used' calculation
            Scenario.RunwayLength = RunwayLength.GetValueOrDefault();

            if (TemperatureType == Models.TemperatureType.F)
            {
                Scenario.TemperatureCelcius = Scenario.ConvertFahrenheitToCelcius(Temperature);
            }
            else
            {
                Scenario.TemperatureCelcius = (int)Temperature.Value;
            }

            if (AltimeterSettingType == Models.AltimeterSettingType.HG)
            {

                Scenario.QNH = Scenario.ConvertInchesOfMercuryToMillibars(AltimeterSetting);
            }
            else
            {
                Scenario.QNH = (int)AltimeterSetting.Value;
            }

            Calculation = PerformanceCalculator.Calculate(Scenario, AircraftType, rootPath);

        }



    }
}