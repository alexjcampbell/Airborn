using System;

namespace Airborn.web.Models
{
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

        private double _takeoff_GroundRoll;

        public double Takeoff_GroundRoll
        {
            get
            {
                return _takeoff_GroundRoll;
            }
            set
            {
                _takeoff_GroundRoll = value;
            }
        }

        private double _takeoff_50FtClearance;

        public double Takeoff_50FtClearance
        {
            get
            {
                return _takeoff_50FtClearance;
            }
            set
            {
                _takeoff_50FtClearance = value;
            }
        }

        private double _landing_Groundroll;

        public double Landing_GroundRoll
        {
            get
            {
                return _landing_Groundroll;
            }
            set
            {
                _landing_Groundroll = value;
            }
        }

        private double _landing_50FtClearance;

        public double Landing_50FtClearance
        {
            get
            {
                return _landing_50FtClearance;
            }
            set
            {
                _landing_50FtClearance = value;
            }
        }


        public int HeadwindComponent
        {
            get
            {
                return Convert.ToInt32(
                Wind.StrengthKts * Math.Cos(
                    CalculationUtilities.ConvertDegreesToRadians(WindRunwayAngularDifferenceMagnetic)
                    )
                    );
            }
        }

        public int CrosswindComponent
        {
            get
            {
                return Convert.ToInt32(
                     Wind.StrengthKts * Math.Sin(
                        CalculationUtilities.ConvertDegreesToRadians(WindRunwayAngularDifferenceMagnetic)
                        )
                );
            }
        }


        public int WindRunwayAngularDifferenceMagnetic
        {
            get
            {

                int difference = (int)CalculationUtilities.AngularDifference(Runway.RunwayHeading.DirectionMagnetic, Wind.Direction.DirectionMagnetic);

                return difference;
            }
        }

        public JsonFile JsonFile
        {
            get;
            set;
        }

    }
}