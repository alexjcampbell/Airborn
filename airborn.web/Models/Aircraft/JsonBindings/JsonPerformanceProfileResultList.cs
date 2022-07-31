using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class JsonPerformanceProfileResultList : List<JsonPerformanceProfileResult>
    {
        public int FindByTemperature(int temperature)
        {
            JsonPerformanceProfileResult result = this.Find(t => t.Temperature == temperature);

            if (result == null)
            {
                throw new TemperaturePerformanceProfileNotFoundException(temperature);
            }

            return result.Distance;
        }
    }
}