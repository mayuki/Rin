using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rin.Core.Record;
using Rin.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Logging
{
    internal class RinLogger : ILogger
    {
        private readonly IRinRequestRecordingFeatureAccessor _accessor;

        public RinLogger(IRinRequestRecordingFeatureAccessor accessor)
        {
            _accessor = accessor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var recording = _accessor?.Feature?.Record;
            if (recording == null) return;

            TimelineStamp.Stamp(logLevel.ToString(), TimelineEventCategory.Trace, formatter(state, exception));
        }

        private class NullDisposable : IDisposable
        {
            public static IDisposable Instance { get; } = new NullDisposable();
            public void Dispose()
            {
            }
        }
    }
}
