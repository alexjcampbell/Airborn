using System;

namespace Airborn.web.Models
{
    public class NoPerformanceDataFoundException : Exception
    {

        public NoPerformanceDataFoundException(double pressureAltitude, double temperature, double weight)
        : base(string.Format("No performance data found for pressure altitude {0}, temperature {1}, and weight {2}", pressureAltitude, temperature, weight))
        {
            PressureAltitude = pressureAltitude;
            Temperature = temperature;
            Weight = weight;
        }

        public double PressureAltitude
        {
            get;
            private set;
        }
        public double Temperature
        {
            get;
            private set;
        }
        public double Weight
        {
            get;
            private set;
        }
    }
}