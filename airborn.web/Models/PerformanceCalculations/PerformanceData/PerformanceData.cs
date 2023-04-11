using System;

namespace Airborn.web.Models
{
    /// <summary>Represents the groundroll distance and the distance to clear a 50' obstacle
    /// for a given pressure altitude and temperature. This is an abstract class because it's only 
    /// intended to provide the foundation for the two concrete classes BookPerformanceData and 
    /// InterpolatedPerformanceData
    /// </summary>
    public abstract class PerformanceData
    {
        private PerformanceData()
        {
        }

        public PerformanceData(
            Scenario scenario,
            decimal pressureAltitude,
            decimal temperature,
            decimal aircraftWeight
            ) : this(scenario, pressureAltitude, temperature, aircraftWeight, null, null)
        {
        }


        public PerformanceData(
            Scenario scenario,
            decimal pressureAltitude,
            decimal temperature,
            decimal aircraftWeight,
            decimal? groundRoll,
            decimal? distanceToClear50Ft)
        {
            Scenario = scenario;
            PressureAltitude = pressureAltitude;
            Temperature = temperature;
            AircraftWeight = aircraftWeight;
            DistanceGroundRoll = groundRoll;
            DistanceToClear50Ft = distanceToClear50Ft;
        }

        public Scenario Scenario;

        public decimal PressureAltitude
        {
            get;
            private set;
        }

        public decimal Temperature
        {
            get;
            private set;
        }

        public decimal? DistanceGroundRoll
        {
            get;
            set;
        }

        public decimal? DistanceToClear50Ft
        {
            get;
            set;
        }

        public decimal AircraftWeight
        {
            get;
            set;
        }
    }
}