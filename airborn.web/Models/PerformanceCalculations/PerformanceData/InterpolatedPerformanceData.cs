using System;

namespace Airborn.web.Models.PerformanceData
{
    /// <summary>
    /// Represents an interpolated groundroll distance and distance to clear a 50' obstacle
    /// for a given pressure altitude, temperature, and aircraft weight
    /// </summary>
    public class InterpolatedPerformanceData : PerformanceDataBase
    {

        /// <summary>
        /// Represents an interpolated groundroll distance and distance to clear a 50' obstacle
        /// for a given pressure altitude, temperature, and aircraft weight
        /// </summary>
        /// <param name="scenario">The scenario for which this data is being calculated (takeoff or landing</param>
        /// <param name="pressureAltitude">The pressure altitude at the airport</param>
        /// <param name="temperature">The temperature at the airport</param>
        /// <param name="weight">The weight of the aircraft</param>
        public InterpolatedPerformanceData(
            Scenario scenario,
            Distance pressureAltitude,
            double temperature,
            double weight)
            : base(
                scenario,
                pressureAltitude,
                temperature,
                weight)
        {
        }

    }
}