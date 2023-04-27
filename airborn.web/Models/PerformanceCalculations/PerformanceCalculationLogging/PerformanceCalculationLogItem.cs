using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class PerformanceCalculationLogItem
    {
        private PerformanceCalculationLogItem()
        {
        }

        public PerformanceCalculationLogItem(string message)
        {
            Message = message;
        }

        public string Message
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Message;
        }

        public List<PerformanceCalculationLogItem> SubItems
        {
            get;
            set;
        } = new List<PerformanceCalculationLogItem>();

        public void Add(string message)
        {
            SubItems.Add(new PerformanceCalculationLogItem(message));
        }

    }
}