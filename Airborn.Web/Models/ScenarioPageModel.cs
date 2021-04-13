using System;
using System.ComponentModel.DataAnnotations;

namespace Airborn.web.Models{
    public class ScenarioPageModel
    {
        [Required]
        public int TakeoffWindStrength
        {
            get;set;
        }

        [Required]
        public int TakeoffWindDirectionMagnetic
        {
            get;set;
        }

        [Required]
        public int TakeoffRunwayHeading
        {
            get;set;
        }

        [Required]
        public int TakeoffMagneticVariation
        {
            get;set;
        }

        [Required]
        public int TakeoffTemperatureCelcius
        {
            get;set;
        }

        [Required]
        public int TakeoffQNH
        {
            get;set;
        }

              [Required]
        public int LandingWindStrength
        {
            get;set;
        }

        [Required]
        public int LandingWindDirectionMagnetic
        {
            get;set;
        }

        [Required]
        public int LandingRunwayHeading
        {
            get;set;
        }

        [Required]
        public int LandingMagneticVariation
        {
            get;set;
        }

        [Required]
        public int LandingTemperatureCelcius
        {
            get;set;
        }

        [Required]
        public int LandingQNH
        {
            get;set;
        }

        public Scenario TakeoffScenario
        {
            get
            {
            
                Runway runway = new Runway(new Direction(TakeoffRunwayHeading, TakeoffMagneticVariation));
                Wind wind = new Wind(new Direction(TakeoffWindDirectionMagnetic, TakeoffMagneticVariation), TakeoffWindStrength);
                
                Scenario scenario = new Scenario(runway, wind);

                return scenario;
            }
        }


        public Scenario LandingScenario
        {
            get
            {
            
                Runway runway = new Runway(new Direction(LandingRunwayHeading, LandingMagneticVariation));
                Wind wind = new Wind(new Direction(LandingWindDirectionMagnetic,LandingMagneticVariation), LandingWindStrength);
                
                Scenario scenario = new Scenario(runway, wind);

                return scenario;
            }
        }


    }
}