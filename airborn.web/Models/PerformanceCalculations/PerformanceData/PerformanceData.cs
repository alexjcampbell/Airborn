using System;

namespace Airborn.web.Models.PerformanceData
{
    /// <summary>Represents the groundroll distance and the distance to clear a 50' obstacle
    /// for a given pressure altitude and temperature. This is an abstract class because it's only 
    /// intended to provide the foundation for the two concrete classes BookPerformanceData and 
    /// InterpolatedPerformanceData
    /// </summary>
    public abstract class PerformanceDataBase
    {
        private PerformanceDataBase()
        {
        }

        /// <summary>
        /// Represents the groundroll distance and the distance to clear a 50' obstacle
        /// for a given pressure altitude and temperature. This is an abstract class because it's only
        /// intended to provide the foundation for the two concrete classes BookPerformanceData and
        /// InterpolatedPerformanceData
        /// </summary>
        /// <param name="scenario">The scenario for which this data is being calculated (takeoff or landing</param>
        /// <param name="pressureAltitude">The pressure altitude at the airport</param>
        /// <param name="temperature">The temperature at the airport</param>
        /// <param name="aircraftWeight">The weight of the aircraft</param>
        public PerformanceDataBase(
            Scenario scenario,
            Distance pressureAltitude,
            double temperature,
            double aircraftWeight
            ) : this(scenario, pressureAltitude, temperature, aircraftWeight, null, null)
        {
        }

        /// <summary>
        /// Represents the groundroll distance and the distance to clear a 50' obstacle
        /// for a given pressure altitude and temperature. This is an abstract class because it's only
        /// intended to provide the foundation for the two concrete classes BookPerformanceData and
        /// InterpolatedPerformanceData
        /// </summary>
        /// <param name="scenario">The scenario for which this data is being calculated (takeoff or landing</param>
        /// <param name="pressureAltitude">The pressure altitude at the airport</param>
        /// <param name="temperature">The temperature at the airport</param>
        /// <param name="aircraftWeight">The weight of the aircraft</param>
        /// <param name="groundRoll">(optional) The groundroll distance from the POH</param>
        /// <param name="distanceToClear50Ft">(optional) The distance to clear a 50' obstacle from the POH</param>
        public PerformanceDataBase(
            Scenario scenario,
            Distance pressureAltitude,
            double temperature,
            double aircraftWeight,
            Distance? groundRoll,
            Distance? distanceToClear50Ft)
        {
            Scenario = scenario;
            PressureAltitude = pressureAltitude;
            Temperature = temperature;
            AircraftWeight = aircraftWeight;
            DistanceGroundRoll = groundRoll;
            DistanceToClear50Ft = distanceToClear50Ft;
        }

        public Scenario Scenario;

        public Distance PressureAltitude
        {
            get;
            private set;
        }

        public double Temperature
        {
            get;
            private set;
        }

        public Distance? DistanceGroundRoll
        {
            get;
            set;
        }

        public Distance? DistanceToClear50Ft
        {
            get;
            set;
        }

        public double AircraftWeight
        {
            get;
            set;
        }
    }
}