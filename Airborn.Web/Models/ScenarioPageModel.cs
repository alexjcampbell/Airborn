using System;
using System.ComponentModel.DataAnnotations;

namespace Airborn.web.Models
{

    public enum Type
    {
        Takeoff,
        Landing       
    }

    public class ScenarioPageModel
    {

        public AircraftPerformanceBase AircraftPerformance
        {
            get;
            set;
        }

        [Required]
        public Type ScenarioType {
            get;set;
        }

        [Required]
        [Range(0, 200, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int WindStrength
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int WindDirectionMagnetic
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int RunwayHeading
        {
            get;set;
        }

        [Required]
        [Range(-2000, 20000, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int FieldElevation
        {
            get;set;
        }

        [Required]
        [Range(0, 20000, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int RunwayLength
        {
            get;set;
        }

    
        [Required]
        [Range(-50, 50, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int MagneticVariation
        {
            get;set;
        }

        [Required]
        [Range(0, 100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int TemperatureCelcius
        {
            get;set;
        }

        [Range(900, 1100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        [Required]
        public int QNH
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

        public double? HeadwindComponent
        {
            get {
                return AircraftPerformance?.Scenario?.HeadwindComponent;
            }
            
        }

        public void Initialise()
        {
            
                Runway runway = new Runway(Direction.FromMagnetic(RunwayHeading, MagneticVariation));
                Wind wind = new Wind(Direction.FromMagnetic(WindDirectionMagnetic, MagneticVariation), WindStrength);
                
                Scenario scenario = new Scenario(runway, wind);
                scenario.QNH = QNH;
                scenario.FieldElevation = FieldElevation;
                scenario.RunwayLength = RunwayLength;
                scenario.TemperatureCelcius = TemperatureCelcius;

                AircraftPerformance = AircraftPerformanceBase.CreateFromJson(scenario);

        }


    }
}