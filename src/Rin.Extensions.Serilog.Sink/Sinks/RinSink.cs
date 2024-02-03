using Serilog.Core;
using Serilog.Events;

namespace Rin.Extensions.Serilog.Sink.Sinks;

public class RinSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        Core.Record.TimelineStamp.Stamp(ToLogLevelName(logEvent.Level), Core.Record.TimelineEventCategory.Trace, logEvent.RenderMessage());
    }

    private static string ToLogLevelName(LogEventLevel level)
    {
        if (level <= LogEventLevel.Verbose) return "Trace";
        if (level <= LogEventLevel.Debug) return "Debug";
        if (level <= LogEventLevel.Information) return "Information";
        if (level <= LogEventLevel.Warning) return "Warning";
        if (level <= LogEventLevel.Error) return "Error";
        return "Critical";
    }
}

