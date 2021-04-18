using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class AircraftPerformanceProfileResultList : List<AircraftPerformanceProfileResult>
    {
        public int FindByTemperature(int temperature)
        {
            AircraftPerformanceProfileResult result = this.Find(t => t.Temperature == temperature);

            if (result == null)
            {
                throw new TemperaturePerformanceProfileNotFoundException(temperature);
            }

            return result.Distance;
        }
    }
}