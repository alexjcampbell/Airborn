using System;

namespace Airborn.web.Models
{
    /// <summary>
    /// Represents a groundroll distance and a distance to clear a 50' obstacle
    /// from the POH ("the book") for a given pressure altitude, temperature, and aircraft
    /// weight
    /// </summary>
    public class BookPerformanceData : PerformanceDataBase
    {
        /// <summary>
        /// Represents a groundroll distance and a distance to clear a 50' obstacle
        /// from the POH ("the book") for a given pressure altitude, temperature, and aircraft
        /// weight
        /// </summary>
        /// <param name="scenario">The scenario for which this data is being calculated (takeoff or landing</param>
        /// <param name="pressureAltitude">The pressure altitude at the airport</param>
        /// <param name="temperature">The temperature at the airport</param>
        /// <param name="aircraftWeight">The weight of the aircraft</param>
        /// <param name="groundRoll">The groundroll distance from the POH</param>
        /// <param name="distanceToClear50Ft">The distance to clear a 50' obstacle from the POH</param>
        public BookPerformanceData(
            Scenario scenario,
            Distance pressureAltitude,
            decimal temperature,
            decimal aircraftWeight,
            Distance groundRoll,
            Distance distanceToClear50Ft)
            :
            base(
                scenario,
                pressureAltitude,
                temperature,
                aircraftWeight,
                groundRoll,
                distanceToClear50Ft)
        {
        }
    }

}