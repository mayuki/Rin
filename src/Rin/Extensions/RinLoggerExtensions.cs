using Rin.Logging;
using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging
{
    public static class RinLoggerExtensions
    {
        [Obsolete("Use AddRinLogger instead of UseRinLogger.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void UseRinLogger(this ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddProvider(new RinLoggerProvider(serviceProvider));
        }

        [Obsolete("Use AddRinLogger instead of UseRinLogger.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void UseRinLogger(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.AddSingleton<ILoggerProvider, RinLoggerProvider>();
        }

        public static void AddRinLogger(this ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddProvider(new RinLoggerProvider(serviceProvider));
        }

        public static void AddRinLogger(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.AddSingleton<ILoggerProvider, RinLoggerProvider>();
        }

    }
}
