using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Airborn.web.Models
{

    public enum Type
    {
        Takeoff,
        Landing       
    }

    public enum TemperatureType
    {
        C,
        F       
    }

    public enum AltimeterSettingType
    {
        HG,
        MB       
    }

    public class ScenarioPageModel
    {

        public AircraftPerformanceBase AircraftPerformance
        {
            get;
            set;
        }

        private List<Runway> _runways = new List<Runway>();

        public List<Runway> Runways
        {
            get {
                return _runways;
            }
            set {
                _runways = value;
            }
        }

        [Required]
        public Type ScenarioType {
            get;set;
        }

        [Required]
        [Range(0, 200, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? WindStrength
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int? WindDirectionMagnetic
        {
            get;set;
        }

        public string AirportIdentifier
        {
            get;set;
        }

        public string RunwayIdentifier
        {
            get;set;
        }        

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int? RunwayHeading
        {
            get;set;
        }

        [Required]
        [Range(-2000, 20000, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int? FieldElevation
        {
            get;set;
        }

        [Range(0, 20000, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int? RunwayLength
        {
            get;set;
        }

        [Range(-50, 50, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int? MagneticVariation
        {
            get;set;
        }

        [Required]
        [Range(-50, 100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public decimal? Temperature
        {
            get;set;
        }
   
        [Required]
        public decimal? AltimeterSetting
        {
            get;set;
        }

        [Required]
        public TemperatureType? TemperatureType
        {
            get;set;
        }

        [Required]
        public AltimeterSettingType? AltimeterSettingType
        {
            get;set;
        }

        public string AircraftType{
            get;set;
        }

        public double? DensityAltitude{
            get {
                return AircraftPerformance?.Scenario?.DensityAltitude;
            }
        }

        public double? PressureAltitude{
            get {
                return AircraftPerformance?.Scenario?.PressureAltitude;
            }
        }

        public double? Distance_GroundRoll
        {
            get
            {
                switch (ScenarioType) {
                    case Type.Takeoff:
                        return AircraftPerformance?.Takeoff_GroundRoll;
                    case Type.Landing:
                        return AircraftPerformance?.Landing_GroundRoll;                        
                }

                return null;
            }
        }

        public double? Distance_Clear50Ft
        {
            get
            {

                switch (ScenarioType) {
                    case Type.Takeoff:
                        return AircraftPerformance?.Takeoff_50FtClearance;
                    case Type.Landing:
                        return AircraftPerformance?.Landing_50FtClearance;                        
                }

                return null;
            }
        }

        public double? CrosswindComponent
        {
            get {
                return AircraftPerformance?.Scenario?.CrosswindComponent;
            }
        }

        public double? CrosswindComponentAbs
        {
            get {
                
                var crosswindComponent = AircraftPerformance?.Scenario?.CrosswindComponent;

                if(crosswindComponent < 0)
                {
                    crosswindComponent = crosswindComponent * -1;
                }

                return crosswindComponent;
            }
        }        

        public string CrosswindComponentText
        {
            get {
                if(AircraftPerformance?.Scenario?.CrosswindComponent > 0)
                {
                    return "Right";
                }
                else if(AircraftPerformance?.Scenario?.CrosswindComponent < 0)
                {
                    return "Left";
                }                
                return "";
            }
        }

        public string HeadwindComponentText
        {
            get {
                if(AircraftPerformance?.Scenario?.HeadwindComponent > 0)
                {
                    return "Headwind";
                }
                else if(AircraftPerformance?.Scenario?.HeadwindComponent < 0)
                {
                    return "Tailwind";
                }                
                return "Headwind";
            }
        }
        public double? HeadwindComponent
        {
            get {
                return AircraftPerformance?.Scenario?.HeadwindComponent;
            }
            
        }

       public double? HeadwindComponentAbs
        {
            get {
                
                var headwindComponent = AircraftPerformance?.Scenario?.HeadwindComponent;

                if(headwindComponent < 0)
                {
                    headwindComponent = headwindComponent * -1;
                }

                return headwindComponent;
            }
        }      

        public double? PercentageRunwayUsed_GroundRoll
        {
            get
            {

                switch (ScenarioType) {
                    case Type.Takeoff:
                        return AircraftPerformance?.Takeoff_GroundRoll / RunwayLength;
                    case Type.Landing:
                        return AircraftPerformance?.Landing_GroundRoll / RunwayLength;                        
                }

                return null;
            }
        }
        public double? PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {

                switch (ScenarioType) {
                    case Type.Takeoff:
                        return AircraftPerformance?.Takeoff_50FtClearance / RunwayLength;
                    case Type.Landing:
                        return AircraftPerformance?.Landing_50FtClearance / RunwayLength;                        
                }

                return null;
            }
        }
        public void LoadAircraftPerformance(string path)
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
            
            Scenario scenario = new Scenario(runway, wind);
            
            scenario.FieldElevation = FieldElevation.Value;

            // runway length is not mandatory as it is only used for the 
            // '% of runway used' calculation
            scenario.RunwayLength = RunwayLength.GetValueOrDefault();

            if(TemperatureType == Models.TemperatureType.F){
                scenario.TemperatureCelcius = Scenario.ConvertFahrenheitToCelcius(Temperature);
            }
            else{
                scenario.TemperatureCelcius = (int)Temperature.Value;
            }

           if(AltimeterSettingType == Models.AltimeterSettingType.HG){

                scenario.QNH = Scenario.ConvertInchesOfMercuryToMillibars(AltimeterSetting);
            }
            else{
                scenario.QNH = (int)AltimeterSetting.Value;
            }            
            

            AircraftPerformance = new AircraftPerformance_SR22_G2(scenario, path);
                
            
        }
        


    }
}