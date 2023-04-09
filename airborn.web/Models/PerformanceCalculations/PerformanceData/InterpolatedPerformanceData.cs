using System;

namespace Airborn.web.Models
{
    public class InterpolatedPerformanceData : PerformanceData
    {
        public InterpolatedPerformanceData(
            Scenario scenario, decimal pressureAltitude, decimal temperature, decimal groundRoll, decimal distanceToClear50Ft)
            : base(scenario, pressureAltitude, temperature, groundRoll, distanceToClear50Ft)
        {
        }
    }

}