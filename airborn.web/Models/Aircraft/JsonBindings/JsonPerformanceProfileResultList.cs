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
                throw new ArgumentOutOfRangeException("temperature", temperature, "No performance profile found for the given temperature");
            }

            return result.Distance;
        }
    }
}