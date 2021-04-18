using System;

namespace Airborn.web.Models
{
    public class AircraftPerformanceProfileNotFoundExceptionBase : Exception
    {
        private AircraftPerformanceProfileNotFoundExceptionBase()
        {

        }

        public int TemperatureCelcius
        {
            get; set;
        }

        public int PressureAltitude
        {
            get; set;
        }

        public AircraftPerformanceProfileNotFoundExceptionBase(string message) : base(message)
        {

        }
    }
}