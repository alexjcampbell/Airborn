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

        public int PressureAltitude
        {
            get; set;
        }

        public JsonPerformanceProfileNotFoundExceptionBase(string message) : base(message)
        {

        }
    }
}