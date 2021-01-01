using System;

namespace PowerLogReader.Core
{
    public class PowerLogEntry
    {
        public DateTime Timestamp { get; set; }

        public long Id { get; set; }

        public string Provider { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
