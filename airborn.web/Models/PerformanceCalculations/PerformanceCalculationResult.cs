using System;

namespace Airborn.web.Models
{
    public class PerformanceCalculationResult
    {

        private PerformanceCalculationResult()
        {

        }


        public PerformanceCalculationResult(Runway runway, Wind wind)
        {
            Runway = runway;
            Wind = wind;
        }


        public Runway Runway
        {
            get;
        }

        public Wind Wind
        {
            get;
        }

        public decimal Takeoff_GroundRoll
        {
            get;
            set;
        }


        public decimal Takeoff_50FtClearance
        {
            get;
            set;
        }

        public decimal Landing_GroundRoll
        {
            get;
            set;
        }

        public decimal Landing_50FtClearance
        {
            get;
            set;
        }


        public decimal HeadwindComponent
        {
            get
            {
                return CalculationUtilities.CalculateHeadwindComponent(
                    Wind.StrengthKts,
                    WindRunwayAngularDifferenceMagnetic
                );
            }
        }

        public decimal CrosswindComponent
        {
            get
            {
                return CalculationUtilities.CalculateCrosswindComponent(
                    Wind.StrengthKts,
                    WindRunwayAngularDifferenceMagnetic
                );
            }
        }

        public decimal WindRunwayAngularDifferenceMagnetic
        {
            get
            {
                return CalculationUtilities.CalculateAngularDifferenceBetweenRunwayAndWind(
                      Runway.RunwayHeading.DirectionMagnetic,
                    Wind.Direction.DirectionMagnetic
                );
            }
        }

        public JsonFile JsonFileLowerWeight
        {
            get;
            set;
        }

        public JsonFile JsonFileHigherWeight
        {
            get;
            set;
        }

    }
}