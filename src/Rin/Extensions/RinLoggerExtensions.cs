using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rin.Core.Diagnostics;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging
{
    public static class RinLoggerExtensions
    {
        public static void UseRinLogger(this ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddProvider(new RinLoggerProvider(serviceProvider));
        }

        public static void UseRinLogger(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.AddSingleton<ILoggerProvider, RinLoggerProvider>();
        }
    }
}
