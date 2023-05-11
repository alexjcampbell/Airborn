using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class PerformanceCalculationLog
    {
        private readonly List<LogItem> logItems = new();

        public IReadOnlyList<LogItem> LogItems => logItems.AsReadOnly();

        public void Add(string message)
        {
            logItems.Add(new LogItem(message));
        }

        public LogItem NewItem(string message)
        {
            var item = new LogItem(message);
            logItems.Add(item);
            return item;
        }

        public class LogItem
        {
            public LogItem(string message)
            {
                Message = message ?? throw new ArgumentNullException(nameof(message));
                SubItems = new List<LogItem>();
            }

            public string Message { get; }

            public List<LogItem> SubItems { get; }

            public void AddSubItem(string message)
            {
                SubItems.Add(new LogItem(message));
            }

            public LogItem NewItem(string message)
            {
                var item = new LogItem(message);
                SubItems.Add(item);
                return item;
            }

            public override string ToString()
            {
                return Message;
            }
        }
    }
}
