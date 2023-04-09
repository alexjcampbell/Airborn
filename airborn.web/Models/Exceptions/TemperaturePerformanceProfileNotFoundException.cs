using System;

namespace Airborn.web.Models
{
    public class TemperaturePerformanceProfileNotFoundException : JsonPerformanceProfileNotFoundExceptionBase
    {

        public TemperaturePerformanceProfileNotFoundException(int temperature) :
            base(String.Format($"No performance profile found for temperature: {temperature} °C"))
        {
            TemperatureCelcius = temperature;
        }


    }
}