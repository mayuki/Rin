using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Rin.Logging
{
    internal class RinLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public RinLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new RinLogger(_serviceProvider.GetService<IHttpContextAccessor>());
        }

        public void Dispose()
        {
        }
    }
}
