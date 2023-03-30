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

        private decimal _takeoff_GroundRoll;

        public decimal Takeoff_GroundRoll
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

        private decimal _takeoff_50FtClearance;

        public decimal Takeoff_50FtClearance
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

        private decimal _landing_Groundroll;

        public decimal Landing_GroundRoll
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

        private decimal _landing_50FtClearance;

        public decimal Landing_50FtClearance
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


        public decimal HeadwindComponent
        {
            get
            {
                return Convert.ToDecimal(
                Wind.StrengthKts * Math.Cos(
                    (double)CalculationUtilities.ConvertDegreesToRadians(WindRunwayAngularDifferenceMagnetic)
                    )
                    );
            }
        }

        public decimal CrosswindComponent
        {
            get
            {
                return Convert.ToDecimal(
                     Wind.StrengthKts * Math.Sin(
                        (double)CalculationUtilities.ConvertDegreesToRadians(WindRunwayAngularDifferenceMagnetic)
                        )
                );
            }
        }


        public decimal WindRunwayAngularDifferenceMagnetic
        {
            get
            {

                decimal difference = (int)CalculationUtilities.AngularDifference(Runway.RunwayHeading.DirectionMagnetic, Wind.Direction.DirectionMagnetic);

                return difference;
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