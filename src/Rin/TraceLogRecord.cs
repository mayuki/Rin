using Microsoft.Extensions.Logging;
using System;

namespace Rin.Core.Record
{
    public class TraceLogRecord
    {
        public DateTime DateTime { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public string Message { get; private set; }

        public TraceLogRecord(DateTime dateTime, LogLevel logLevel, string message)
        {
            DateTime = dateTime;
            LogLevel = logLevel;
            Message = message;
        }
    }
}
