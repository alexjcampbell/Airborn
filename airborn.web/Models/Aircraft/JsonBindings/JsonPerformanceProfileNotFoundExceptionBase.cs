using System;

namespace Airborn.web.Models
{
    public class JsonPerformanceProfileNotFoundExceptionBase : Exception
    {
        private JsonPerformanceProfileNotFoundExceptionBase()
        {

        }

        public int TemperatureCelcius
        {
            get; set;
        }

        public double PressureAltitude
        {
            get; set;
        }

        public JsonPerformanceProfileNotFoundExceptionBase(string message) : base(message)
        {

        }
    }
}