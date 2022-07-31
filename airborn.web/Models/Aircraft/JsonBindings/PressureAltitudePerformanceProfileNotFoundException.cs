using System;

namespace Airborn.web.Models
{
    public class PressureAltitudePerformanceProfileNotFoundException : JsonPerformanceProfileNotFoundExceptionBase
    {
        public PressureAltitudePerformanceProfileNotFoundException(int pressureAltitude) :
            base(String.Format($"No performance profile found for pressure altitude: {pressureAltitude} ft"))
        {
            PressureAltitude = pressureAltitude;
        }
    }
}