using System;

namespace Airborn.web.Models
{
    /// <summary>
    /// Represented a groundroll distance and a distance to clear a 50' obstacle
    /// from the POH ("the book") for a given pressure altitude, temperature, and aircraft
    /// weight
    /// </summary>
    public class BookPerformanceData : PerformanceData
    {
        public BookPerformanceData(
            Scenario scenario,
            decimal pressureAltitude,
            decimal temperature,
            decimal aircraftWeight,
            decimal groundRoll,
            decimal distanceToClear50Ft)
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