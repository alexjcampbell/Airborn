using System;

namespace Airborn.web.Models
{
    public class InterpolatedPerformanceData : PerformanceData
    {

        /// <summary>
        /// Represents an interpolated groundroll distance and distance to clear a 50' obstacle
        /// for a given pressure altitude, temperature, and aircraft weight
        public InterpolatedPerformanceData(
            Scenario scenario,
            decimal pressureAltitude,
            decimal temperature,
            decimal weight)
            : base(
                scenario,
                pressureAltitude,
                temperature,
                weight)
        {
        }

    }
}