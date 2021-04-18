using System;

namespace Airborn.web.Models
{
    public class AircraftPerformanceProfileNotFoundException : Exception
    {
        private AircraftPerformanceProfileNotFoundException()
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

        public AircraftPerformanceProfileNotFoundException(string message) : base(message)
        {

        }
    }
}