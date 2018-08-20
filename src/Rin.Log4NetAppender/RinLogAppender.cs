using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net.Core;
using Rin.Core.Record;

namespace Rin.Log4NetAppender
{
    public class RinLogAppender : log4net.Appender.AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            TimelineStamp.Stamp(ToLogLevelName(loggingEvent.Level), TimelineEventCategory.Trace, loggingEvent.RenderedMessage);
        }

        private static string ToLogLevelName(Level level)
        {
            if (level <= Level.Trace) return "Trace"; // LogLevel.Trace
            if (level <= Level.Debug) return "Debug"; // LogLevel.Debug
            if (level <= Level.Notice) return "Information"; // LogLevel.Information
            if (level <= Level.Warn) return "Warning"; // LogLevel.Warning
            if (level <= Level.Error) return "Error"; // LogLevel.Error
            return "Critical"; // LogLevel.Critical
        }
    }
}
