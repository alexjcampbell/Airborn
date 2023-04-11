using System;

namespace Airborn.web.Models
{
    public class InterpolatedPerformanceData : PerformanceData
    {

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

        public InterpolatedPerformanceData(
            Scenario scenario,
            decimal pressureAltitude,
            decimal temperature,
            decimal weight,
            decimal groundRoll,
            decimal distanceToClear50Ft)
            : base(
                scenario,
                pressureAltitude,
                temperature,
                weight,
                groundRoll,
                distanceToClear50Ft)
        {
        }
    }

}