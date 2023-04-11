using System;

namespace Airborn.web.Models
{
    public class NoPerformanceDataFoundException : Exception
    {

        public NoPerformanceDataFoundException(decimal pressureAltitude, decimal temperature, decimal weight)
        : base(string.Format("No performance data found for pressure altitude {0}, temperature {1}, and weight {2}", pressureAltitude, temperature, weight))
        {
            PressureAltitude = pressureAltitude;
            Temperature = temperature;
            Weight = weight;
        }

        public decimal PressureAltitude
        {
            get;
            private set;
        }
        public decimal Temperature
        {
            get;
            private set;
        }
        public decimal Weight
        {
            get;
            private set;
        }
    }
}