using Rin.Extensions.Serilog.Sink.Sinks;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Rin.Extensions.Serilog.Sink;

public static class RinLoggerConfigurationExtensions
{
    public static LoggerConfiguration Rin(
        this LoggerSinkConfiguration loggerSinkConfiguration,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
    {
        var sink = new RinSink();
        return loggerSinkConfiguration.Sink(sink, restrictedToMinimumLevel);
    }
}
