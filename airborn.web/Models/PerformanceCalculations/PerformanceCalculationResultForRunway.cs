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

        public PerformanceCalculationResultForRunway(Runway runway, Wind wind, PerformanceCalculationLogItem logItem, Distance pressureAltitude)
        {
            Runway = runway;
            Wind = wind;
            LogItem = logItem;
            PressureAltitude = pressureAltitude;
        }


        public Runway Runway
        {
            get;
        }

        public Wind Wind
        {
            get;
        }

        public Distance Takeoff_GroundRoll
        {
            get;
            set;
        }


        public Distance Takeoff_50FtClearance
        {
            get;
            set;
        }

        public Distance Landing_GroundRoll
        {
            get;
            set;
        }

        public Distance Landing_50FtClearance
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

        public PerformanceCalculationLogItem LogItem
        {
            get;
            set;
        }

        public Distance PressureAltitude
        {
            get;
            set;
        }

    }
}