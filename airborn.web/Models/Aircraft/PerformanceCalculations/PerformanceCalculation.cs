using System;

namespace Airborn.web.Models
{
    public class PerformanceCalculation
    {
        private double _takeoff_GroundRoll;

        public double Takeoff_GroundRoll
        {
            get {
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

        public JsonFile JsonFile
        {
            get;
            set;
        }

    }
}