using System;
using System.ComponentModel.DataAnnotations;

namespace Airborn.web.Models{
    public class ScenarioPageModel
    {
        [Required]
        [Range(0, 200, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int TakeoffWindStrength
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int TakeoffWindDirectionMagnetic
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int TakeoffRunwayHeading
        {
            get;set;
        }


        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        public int TakeoffMagneticVariation
        {
            get;set;
        }

        [Required]
        [Range(0, 100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int TakeoffTemperatureCelcius
        {
            get;set;
        }

        [Range(900, 1100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]        
        [Required]
        public int TakeoffQNH
        {
            get;set;
        }

        [Required]
        [Range(0, 200, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int LandingWindStrength
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int LandingWindDirectionMagnetic
        {
            get;set;
        }

        [Required]
        [Range(0, 359, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int LandingRunwayHeading
        {
            get;set;
        }

        [Required]
        [Range(-75, 75, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int LandingMagneticVariation
        {
            get;set;
        }

        [Required]
        [Range(0, 100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int LandingTemperatureCelcius
        {
            get;set;
        }

        [Required]
        [Range(900, 1100, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]                
        public int LandingQNH
        {
            get;set;
        }

        public Scenario TakeoffScenario
        {
            get
            {
            
                Runway runway = new Runway(Direction.FromMagnetic(TakeoffRunwayHeading, TakeoffMagneticVariation));
                Wind wind = new Wind(Direction.FromMagnetic(TakeoffWindDirectionMagnetic, TakeoffMagneticVariation), TakeoffWindStrength);
                
                Scenario scenario = new Scenario(runway, wind);

                return scenario;
            }
        }


        public Scenario LandingScenario
        {
            get
            {
            
                Runway runway = new Runway(Direction.FromMagnetic(LandingRunwayHeading, LandingMagneticVariation));
                Wind wind = new Wind(Direction.FromMagnetic(LandingWindDirectionMagnetic,LandingMagneticVariation), LandingWindStrength);
                
                Scenario scenario = new Scenario(runway, wind);

                return scenario;
            }
        }


    }
}