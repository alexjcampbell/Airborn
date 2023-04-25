using System;

namespace Airborn.web.Models
{

    /// <summary>
    /// This class is responsible for storing the results of a performance calculation for a given runway
    /// </summary>
    public class PerformanceCalculationResultForRunway
    {

        private PerformanceCalculationResultForRunway()
        {

        }

        public PerformanceCalculationResultForRunway(Runway runway, Wind wind)
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
                return CalculationUtilities.HeadwindComponent(
                    Wind.StrengthKts,
                    WindRunwayAngularDifferenceMagnetic
                );
            }
        }

        public decimal CrosswindComponent
        {
            get
            {
                return CalculationUtilities.CrosswindComponent(
                    Wind.StrengthKts,
                    WindRunwayAngularDifferenceMagnetic
                );
            }
        }

        public decimal WindRunwayAngularDifferenceMagnetic
        {
            get
            {
                return CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(
                      Runway.RunwayHeading.DirectionMagnetic,
                    Wind.Direction.DirectionMagnetic
                );
            }
        }

    }
}