using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class PerformanceCalculationLogger : List<PerformanceCalculationLogItem>
    {
        public void Add(string message)
        {
            Add(new PerformanceCalculationLogItem(message));
        }
    }
}