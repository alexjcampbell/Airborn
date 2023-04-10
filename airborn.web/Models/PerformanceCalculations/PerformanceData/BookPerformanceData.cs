using System;

namespace Airborn.web.Models
{
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